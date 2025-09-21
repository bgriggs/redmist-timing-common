using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace RedMist.PatchGenerator
{
    /// <summary>
    /// Source generator that creates patch class variants from classes marked with [GeneratePatch].
    /// Patch classes have all properties made nullable to support partial updates.
    /// Also generates Mapperly mappers for efficient patch application.
    /// </summary>
    [Generator]
    public class PatchClassGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Find all classes marked with [GeneratePatch] attribute
            var provider = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    "RedMist.TimingCommon.Attributes.GeneratePatchAttribute",
                    static (node, _) => node is ClassDeclarationSyntax,
                    static (context, _) => GetClassToGenerate(context))
                .Where(static x => x is not null);

            // Generate patch classes and mappers for each marked class
            context.RegisterSourceOutput(provider, static (context, classInfo) =>
            {
                if (classInfo is not null)
                {
                    GeneratePatchClass(context, classInfo);

                    if (classInfo.GenerateMapper)
                    {
                        GenerateMapper(context, classInfo);
                    }
                }
            });
        }

        private static ClassToGenerate? GetClassToGenerate(GeneratorAttributeSyntaxContext context)
        {
            if (context.TargetNode is not ClassDeclarationSyntax classDeclaration)
                return null;

            var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
            if (classSymbol is null)
                return null;

            // Get the GeneratePatch attribute
            var generatePatchAttribute = context.Attributes[0];

            // Extract attribute properties with proper null handling
            var patchClassName = GetAttributeValue<string>(generatePatchAttribute, "PatchClassName");
            var patchNamespace = GetAttributeValue<string>(generatePatchAttribute, "PatchNamespace");
            var mapperClassName = GetAttributeValue<string>(generatePatchAttribute, "MapperClassName");
            var mapperNamespace = GetAttributeValue<string>(generatePatchAttribute, "MapperNamespace");
            var includeMessagePack = GetAttributeValue<bool?>(generatePatchAttribute, "IncludeMessagePackAttributes") ?? true;
            var includeJson = GetAttributeValue<bool?>(generatePatchAttribute, "IncludeJsonAttributes") ?? true;
            var includeValidation = GetAttributeValue<bool?>(generatePatchAttribute, "IncludeValidationAttributes") ?? true;
            var generateMapper = GetAttributeValue<bool?>(generatePatchAttribute, "GenerateMapper") ?? true;

            var sourceNamespace = classSymbol.ContainingNamespace?.ToDisplayString() ?? "";

            // Get all public properties
            var properties = new List<PropertyInfo>();
            foreach (var member in classSymbol.GetMembers().OfType<IPropertySymbol>())
            {
                if (member.IsStatic || member.DeclaredAccessibility != Accessibility.Public)
                    continue;

                // Skip indexers
                if (member.IsIndexer)
                    continue;

                // Get a clean type name without global prefix
                var typeName = GetCleanTypeName(member.Type);

                // Determine if this is already a nullable type
                bool isAlreadyNullable = member.Type.CanBeReferencedByName && 
                    (member.Type.SpecialType == SpecialType.None && member.Type.IsValueType && 
                     member.Type is INamedTypeSymbol namedType && namedType.IsGenericType && 
                     namedType.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T) ||
                    (!member.Type.IsValueType && member.NullableAnnotation == NullableAnnotation.Annotated);

                var propertyInfo = new PropertyInfo(
                    member.Name,
                    typeName,
                    GetPropertyAttributes(member, includeMessagePack, includeJson, includeValidation),
                    member.Type.CanBeReferencedByName && !member.Type.IsValueType,
                    IsCollectionType(member.Type),
                    isAlreadyNullable
                );

                properties.Add(propertyInfo);
            }

            return new ClassToGenerate(
                classSymbol.Name,
                sourceNamespace,
                patchClassName ?? $"{classSymbol.Name}Patch",
                patchNamespace ?? sourceNamespace,
                mapperClassName ?? $"{classSymbol.Name}Mapper",
                mapperNamespace ?? $"{sourceNamespace}.Mappers",
                properties.ToImmutableArray(),
                includeMessagePack,
                includeJson,
                includeValidation,
                generateMapper
            );
        }

        private static string GetCleanTypeName(ITypeSymbol type)
        {
            // Handle special cases first
            if (type.SpecialType != SpecialType.None)
            {
                return type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            }

            // For generic types like List<T>, Dictionary<K,V>, etc.
            if (type is INamedTypeSymbol namedType && namedType.IsGenericType)
            {
                var typeDefinition = namedType.ConstructedFrom.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                var typeArguments = namedType.TypeArguments.Select(ta => GetCleanTypeName(ta));
                return $"{typeDefinition.Split('<')[0]}<{string.Join(", ", typeArguments)}>";
            }

            // For simple types, use the minimal format but ensure we get the type name only
            var displayString = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            
            // Clean up any problematic characters
            displayString = displayString
                .Replace("\r\n", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", " ")
                .Trim();
            
            // Replace multiple spaces with single space
            while (displayString.Contains("  "))
            {
                displayString = displayString.Replace("  ", " ");
            }
            
            // If the result is empty or contains null characters, use a fallback
            if (string.IsNullOrWhiteSpace(displayString) || displayString.Contains('\0'))
            {
                return "object";
            }
            
            return displayString;
        }

        private static bool IsCollectionType(ITypeSymbol type)
        {
            // Check if the type implements IEnumerable (but not string)
            if (type.SpecialType == SpecialType.System_String)
                return false;

            // Check for common collection interfaces and types
            var typeName = type.ToDisplayString();
            return typeName.Contains("System.Collections.Generic.List") ||
                   typeName.Contains("System.Collections.Generic.Dictionary") ||
                   typeName.Contains("System.Collections.Generic.IList") ||
                   typeName.Contains("System.Collections.Generic.ICollection") ||
                   typeName.Contains("System.Collections.Generic.IEnumerable") ||
                   type.AllInterfaces.Any(i => i.ToDisplayString().Contains("System.Collections.Generic.IEnumerable"));
        }

        private static T? GetAttributeValue<T>(AttributeData attribute, string propertyName)
        {
            var namedArg = attribute.NamedArguments.FirstOrDefault(kvp => kvp.Key == propertyName);
            if (namedArg.Value.Value is T value)
                return value;
            return default;
        }

        private static ImmutableArray<AttributeInfo> GetPropertyAttributes(IPropertySymbol property, bool includeMessagePack, bool includeJson, bool includeValidation)
        {
            var attributes = new List<AttributeInfo>();

            foreach (var attr in property.GetAttributes())
            {
                var attrName = attr.AttributeClass?.Name;
                if (attrName is null) continue;

                // Include MessagePack attributes
                if (includeMessagePack && attrName == "KeyAttribute" && attr.AttributeClass?.ContainingNamespace?.ToDisplayString() == "MessagePack")
                {
                    var keyValue = attr.ConstructorArguments.FirstOrDefault().Value?.ToString();
                    if (!string.IsNullOrEmpty(keyValue))
                    {
                        attributes.Add(new AttributeInfo("MessagePack.Key", new[] { keyValue }));
                    }
                }

                // Include JSON attributes
                if (includeJson && attrName == "JsonPropertyNameAttribute")
                {
                    var nameValue = attr.ConstructorArguments.FirstOrDefault().Value?.ToString();
                    if (!string.IsNullOrEmpty(nameValue))
                    {
                        attributes.Add(new AttributeInfo("JsonPropertyName", new[] { $"\"{nameValue}\"" }));
                    }
                }

                // Include validation attributes
                if (includeValidation)
                {
                    if (attrName == "MaxLengthAttribute")
                    {
                        var lengthValue = attr.ConstructorArguments.FirstOrDefault().Value?.ToString();
                        if (!string.IsNullOrEmpty(lengthValue))
                        {
                            attributes.Add(new AttributeInfo("MaxLength", new[] { lengthValue }));
                        }
                    }
                    else if (attrName == "RequiredAttribute")
                    {
                        attributes.Add(new AttributeInfo("Required", System.Array.Empty<string>()));
                    }
                    else if (attrName == "RangeAttribute")
                    {
                        var min = attr.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString();
                        var max = attr.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString();
                        if (!string.IsNullOrEmpty(min) && !string.IsNullOrEmpty(max))
                        {
                            attributes.Add(new AttributeInfo("Range", new[] { min, max }));
                        }
                    }
                }
            }

            return attributes.ToImmutableArray();
        }

        private static void GeneratePatchClass(SourceProductionContext context, ClassToGenerate classInfo)
        {
            var sourceBuilder = new StringBuilder();

            // Add using statements
            sourceBuilder.AppendLine("// <auto-generated />");
            sourceBuilder.AppendLine("#nullable enable");
            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine("using System;");
            sourceBuilder.AppendLine("using System.Collections.Generic;");
            sourceBuilder.AppendLine("using MessagePack;");
            sourceBuilder.AppendLine("using System.ComponentModel.DataAnnotations;");
            
            // Add the source namespace to resolve types in the same namespace
            sourceBuilder.AppendLine($"using {classInfo.SourceNamespace};");
            
            // Add additional namespaces that might be needed for property types
            var additionalNamespaces = new HashSet<string>();
            foreach (var prop in classInfo.Properties)
            {
                var typeName = prop.TypeName;
                
                // Check for common sub-namespaces that might be needed
                if (typeName.Contains("VideoStatus") || typeName.Contains("VideoDestination") || typeName.Contains("VideoSystemType"))
                {
                    additionalNamespaces.Add($"{classInfo.SourceNamespace}.InCarVideo");
                }
                if (typeName.Contains("CompletedSection"))
                {
                    // CompletedSection might be in the same namespace, but ensure it's covered
                    additionalNamespaces.Add(classInfo.SourceNamespace);
                }
            }
            
            foreach (var ns in additionalNamespaces.Where(ns => ns != classInfo.SourceNamespace))
            {
                sourceBuilder.AppendLine($"using {ns};");
            }
            
            sourceBuilder.AppendLine();
            
            // Use traditional namespace syntax for compatibility
            sourceBuilder.AppendLine($"namespace {classInfo.PatchNamespace}");
            sourceBuilder.AppendLine("{");

            // Add class documentation
            sourceBuilder.AppendLine("    /// <summary>");
            sourceBuilder.AppendLine($"    /// Patch variant of {classInfo.SourceClassName} with nullable properties for partial updates.");
            sourceBuilder.AppendLine("    /// Generated automatically by PatchClassGenerator.");
            sourceBuilder.AppendLine("    /// </summary>");

            // Add MessagePack attribute if enabled
            if (classInfo.IncludeMessagePack)
            {
                sourceBuilder.AppendLine("    [MessagePackObject]");
            }

            sourceBuilder.AppendLine($"    public class {classInfo.PatchClassName}");
            sourceBuilder.AppendLine("    {");

            // Generate ALL properties
            foreach (var prop in classInfo.Properties)
            {
                // Skip any properties that might cause issues
                if (string.IsNullOrWhiteSpace(prop.Name) || string.IsNullOrWhiteSpace(prop.TypeName))
                {
                    sourceBuilder.AppendLine($"        // Skipped property with invalid name or type");
                    continue;
                }

                // Add property documentation
                sourceBuilder.AppendLine("        /// <summary>");
                sourceBuilder.AppendLine($"        /// Patch field for {prop.Name}. Null indicates no change.");
                sourceBuilder.AppendLine("        /// </summary>");

                // Add MessagePack key attribute
                foreach (var attr in prop.Attributes.Where(a => a.Name == "MessagePack.Key"))
                {
                    var attributeString = attr.Arguments.Length > 0
                        ? $"[{attr.Name}({string.Join(", ", attr.Arguments)})]"
                        : $"[{attr.Name}]";
                    sourceBuilder.AppendLine($"        {attributeString}");
                }

                // Add MaxLength attributes
                foreach (var attr in prop.Attributes.Where(a => a.Name == "MaxLength"))
                {
                    var attributeString = attr.Arguments.Length > 0
                        ? $"[{attr.Name}({string.Join(", ", attr.Arguments)})]"
                        : $"[{attr.Name}]";
                    sourceBuilder.AppendLine($"        {attributeString}");
                }

                // Generate nullable property type with safer handling
                var typeName = prop.TypeName.Trim();
                
                // Handle nullable types properly - don't make already nullable types double-nullable
                string nullableType;
                if (prop.IsAlreadyNullable)
                {
                    // Type is already nullable, use as-is
                    nullableType = typeName;
                }
                else if (prop.IsReferenceType)
                {
                    // Reference types get ?
                    nullableType = $"{typeName}?";
                }
                else
                {
                    // Value types get ?
                    nullableType = $"{typeName}?";
                }
                
                sourceBuilder.AppendLine($"        public {nullableType} {prop.Name} {{ get; set; }}");
                sourceBuilder.AppendLine();
            }

            sourceBuilder.AppendLine("    }");
            sourceBuilder.AppendLine("}");

            // Create the source text with explicit encoding
            var sourceCode = sourceBuilder.ToString();
            var sourceText = SourceText.From(sourceCode, Encoding.UTF8);
            context.AddSource($"{classInfo.PatchClassName}.g.cs", sourceText);
        }

        private static void GenerateMapper(SourceProductionContext context, ClassToGenerate classInfo)
        {
            var sourceBuilder = new StringBuilder();

            // Add using statements
            sourceBuilder.AppendLine("// <auto-generated />");
            sourceBuilder.AppendLine("#nullable enable");
            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine("using System;");
            sourceBuilder.AppendLine("using System.Linq;");
            sourceBuilder.AppendLine($"using {classInfo.SourceNamespace};");
            if (classInfo.PatchNamespace != classInfo.SourceNamespace)
            {
                sourceBuilder.AppendLine($"using {classInfo.PatchNamespace};");
            }
            
            // Add additional namespaces that might be needed for property types
            var additionalNamespaces = new HashSet<string>();
            foreach (var prop in classInfo.Properties)
            {
                var typeName = prop.TypeName;
                
                // Check for common sub-namespaces that might be needed
                if (typeName.Contains("VideoStatus") || typeName.Contains("VideoDestination") || typeName.Contains("VideoSystemType"))
                {
                    additionalNamespaces.Add($"{classInfo.SourceNamespace}.InCarVideo");
                }
                if (typeName.Contains("CompletedSection"))
                {
                    // CompletedSection might be in the same namespace, but ensure it's covered
                    additionalNamespaces.Add(classInfo.SourceNamespace);
                }
            }
            
            foreach (var ns in additionalNamespaces.Where(ns => ns != classInfo.SourceNamespace && ns != classInfo.PatchNamespace))
            {
                sourceBuilder.AppendLine($"using {ns};");
            }
            
            sourceBuilder.AppendLine();

            // Use traditional namespace syntax for compatibility
            sourceBuilder.AppendLine($"namespace {classInfo.MapperNamespace}");
            sourceBuilder.AppendLine("{");

            // Add class documentation
            sourceBuilder.AppendLine("    /// <summary>");
            sourceBuilder.AppendLine($"    /// Mapper for applying {classInfo.PatchClassName} to {classInfo.SourceClassName}.");
            sourceBuilder.AppendLine("    /// Generated automatically by PatchClassGenerator.");
            sourceBuilder.AppendLine("    /// </summary>");
            sourceBuilder.AppendLine($"    public static class {classInfo.MapperClassName}");
            sourceBuilder.AppendLine("    {");

            // Use ALL properties, not just the simplified subset
            var allProperties = classInfo.Properties.Where(p => 
                !string.IsNullOrWhiteSpace(p.Name) && 
                !string.IsNullOrWhiteSpace(p.TypeName)).ToList();

            // ApplyPatch method - manual implementation
            sourceBuilder.AppendLine("        /// <summary>");
            sourceBuilder.AppendLine($"        /// Applies a {classInfo.PatchClassName} to an existing {classInfo.SourceClassName} instance.");
            sourceBuilder.AppendLine("        /// Only non-null properties in the patch will be applied.");
            sourceBuilder.AppendLine("        /// </summary>");
            sourceBuilder.AppendLine($"        /// <param name=\"patch\">The patch containing changes to apply</param>");
            sourceBuilder.AppendLine($"        /// <param name=\"target\">The target {classInfo.SourceClassName} to update</param>");
            sourceBuilder.AppendLine($"        public static void ApplyPatch({classInfo.PatchClassName} patch, {classInfo.SourceClassName} target)");
            sourceBuilder.AppendLine("        {");

            // Generate apply logic for each property
            foreach (var prop in allProperties)
            {
                if (prop.IsReferenceType)
                {
                    sourceBuilder.AppendLine($"            if (patch.{prop.Name} != null) target.{prop.Name} = patch.{prop.Name};");
                }
                else
                {
                    sourceBuilder.AppendLine($"            if (patch.{prop.Name} != null) target.{prop.Name} = patch.{prop.Name}.Value;");
                }
            }

            sourceBuilder.AppendLine("        }");
            sourceBuilder.AppendLine();

            // PatchToEntity method - manual implementation
            sourceBuilder.AppendLine("        /// <summary>");
            sourceBuilder.AppendLine($"        /// Creates a new {classInfo.SourceClassName} from a patch, useful for creating initial state.");
            sourceBuilder.AppendLine("        /// </summary>");
            sourceBuilder.AppendLine($"        /// <param name=\"patch\">The patch to convert to a full {classInfo.SourceClassName}</param>");
            sourceBuilder.AppendLine($"        /// <returns>A new {classInfo.SourceClassName} instance</returns>");
            sourceBuilder.AppendLine($"        public static {classInfo.SourceClassName} PatchToEntity({classInfo.PatchClassName} patch)");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine($"            var entity = new {classInfo.SourceClassName}();");
            sourceBuilder.AppendLine("            ApplyPatch(patch, entity);");
            sourceBuilder.AppendLine("            return entity;");
            sourceBuilder.AppendLine("        }");
            sourceBuilder.AppendLine();

            // ToPatch method - copies all fields from source to patch
            sourceBuilder.AppendLine("        /// <summary>");
            sourceBuilder.AppendLine($"        /// Creates a {classInfo.PatchClassName} from a {classInfo.SourceClassName} by copying all fields.");
            sourceBuilder.AppendLine("        /// This creates a complete patch representation of the source object.");
            sourceBuilder.AppendLine("        /// </summary>");
            sourceBuilder.AppendLine($"        /// <param name=\"source\">The source {classInfo.SourceClassName} to convert</param>");
            sourceBuilder.AppendLine($"        /// <returns>A {classInfo.PatchClassName} with all properties set from the source</returns>");
            sourceBuilder.AppendLine($"        public static {classInfo.PatchClassName} ToPatch({classInfo.SourceClassName} source)");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine($"            var patch = new {classInfo.PatchClassName}();");
            sourceBuilder.AppendLine();

            // Generate copy logic for each property
            foreach (var prop in allProperties)
            {
                sourceBuilder.AppendLine($"            patch.{prop.Name} = source.{prop.Name};");
            }

            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine("            return patch;");
            sourceBuilder.AppendLine("        }");
            sourceBuilder.AppendLine();

            // CreatePatch method (manual implementation)
            sourceBuilder.AppendLine("        /// <summary>");
            sourceBuilder.AppendLine($"        /// Creates a patch from the differences between two {classInfo.SourceClassName} instances.");
            sourceBuilder.AppendLine("        /// </summary>");
            sourceBuilder.AppendLine("        /// <param name=\"original\">The original state</param>");
            sourceBuilder.AppendLine("        /// <param name=\"updated\">The updated state</param>");
            sourceBuilder.AppendLine("        /// <returns>A patch containing only the differences</returns>");
            sourceBuilder.AppendLine($"        public static {classInfo.PatchClassName} CreatePatch({classInfo.SourceClassName} original, {classInfo.SourceClassName} updated)");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine($"            var patch = new {classInfo.PatchClassName}();");
            sourceBuilder.AppendLine();

            // Generate comparison logic for each property
            foreach (var prop in allProperties)
            {
                GeneratePropertyComparison(sourceBuilder, prop);
                sourceBuilder.AppendLine();
            }

            sourceBuilder.AppendLine("            return patch;");
            sourceBuilder.AppendLine("        }");
            sourceBuilder.AppendLine();

            // IsValidPatch method
            sourceBuilder.AppendLine("        /// <summary>");
            sourceBuilder.AppendLine("        /// Determines if a patch contains any changes (has at least one non-null property).");
            sourceBuilder.AppendLine("        /// </summary>");
            sourceBuilder.AppendLine($"        /// <param name=\"patch\">The patch to validate</param>");
            sourceBuilder.AppendLine("        /// <returns>True if the patch contains changes, false otherwise</returns>");
            sourceBuilder.AppendLine($"        public static bool IsValidPatch({classInfo.PatchClassName} patch)");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine("            return ");

            var isFirstProperty = true;
            foreach (var prop in allProperties)
            {
                if (!isFirstProperty)
                {
                    sourceBuilder.AppendLine(" ||");
                }
                sourceBuilder.Append($"                patch.{prop.Name} != null");
                isFirstProperty = false;
            }
            sourceBuilder.AppendLine(";");
            sourceBuilder.AppendLine("        }");
            sourceBuilder.AppendLine();

            // GetChangedProperties method
            sourceBuilder.AppendLine("        /// <summary>");
            sourceBuilder.AppendLine("        /// Gets the names of properties that have changed in the patch.");
            sourceBuilder.AppendLine("        /// </summary>");
            sourceBuilder.AppendLine($"        /// <param name=\"patch\">The patch to analyze</param>");
            sourceBuilder.AppendLine("        /// <returns>Array of property names that have changes</returns>");
            sourceBuilder.AppendLine($"        public static string[] GetChangedProperties({classInfo.PatchClassName} patch)");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine("            var changes = new System.Collections.Generic.List<string>();");
            sourceBuilder.AppendLine();

            foreach (var prop in allProperties)
            {
                sourceBuilder.AppendLine($"            if (patch.{prop.Name} != null)");
                sourceBuilder.AppendLine($"                changes.Add(nameof({classInfo.SourceClassName}.{prop.Name}));");
            }

            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine("            return changes.ToArray();");
            sourceBuilder.AppendLine("        }");

            sourceBuilder.AppendLine();

            // Merge method - combines two patches with second taking priority
            sourceBuilder.AppendLine("        /// <summary>");
            sourceBuilder.AppendLine($"        /// Merges two {classInfo.PatchClassName} instances, with the second patch taking priority.");
            sourceBuilder.AppendLine("        /// Non-null properties in the second patch will override corresponding properties in the first patch.");
            sourceBuilder.AppendLine("        /// </summary>");
            sourceBuilder.AppendLine($"        /// <param name=\"firstPatch\">The base patch</param>");
            sourceBuilder.AppendLine($"        /// <param name=\"secondPatch\">The patch whose non-null properties will override the first</param>");
            sourceBuilder.AppendLine($"        /// <returns>A new merged {classInfo.PatchClassName} instance</returns>");
            sourceBuilder.AppendLine($"        public static {classInfo.PatchClassName} Merge({classInfo.PatchClassName} firstPatch, {classInfo.PatchClassName} secondPatch)");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine($"            var merged = new {classInfo.PatchClassName}();");
            sourceBuilder.AppendLine();

            // Generate merge logic for each property
            foreach (var prop in allProperties)
            {
                    sourceBuilder.AppendLine($"            merged.{prop.Name} = secondPatch.{prop.Name} ?? firstPatch.{prop.Name};");
            }

            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine("            return merged;");
            sourceBuilder.AppendLine("        }");

            sourceBuilder.AppendLine("    }");
            sourceBuilder.AppendLine("}");

            // Add the source to compilation
            var sourceText = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
            context.AddSource($"{classInfo.MapperClassName}.g.cs", sourceText);
        }

        private static void GeneratePropertyComparison(StringBuilder sourceBuilder, PropertyInfo prop)
        {
            if (prop.IsReferenceType)
            {
                if (prop.IsCollectionType)
                {
                    // Collection types - use SequenceEqual with null checks
                    sourceBuilder.AppendLine($"            if (!ReferenceEquals(original.{prop.Name}, updated.{prop.Name}))");
                    sourceBuilder.AppendLine("            {");
                    sourceBuilder.AppendLine($"                if ((original.{prop.Name} == null) != (updated.{prop.Name} == null) ||");
                    sourceBuilder.AppendLine($"                    (original.{prop.Name} != null && updated.{prop.Name} != null && !original.{prop.Name}.SequenceEqual(updated.{prop.Name})))");
                    sourceBuilder.AppendLine($"                    patch.{prop.Name} = updated.{prop.Name};");
                    sourceBuilder.AppendLine("            }");
                }
                else
                {
                    // Regular reference types
                    sourceBuilder.AppendLine($"            if (!ReferenceEquals(original.{prop.Name}, updated.{prop.Name}) && ");
                    sourceBuilder.AppendLine($"                !object.Equals(original.{prop.Name}, updated.{prop.Name}))");
                    sourceBuilder.AppendLine($"                patch.{prop.Name} = updated.{prop.Name};");
                }
            }
            else
            {
                // Value types
                sourceBuilder.AppendLine($"            if (!original.{prop.Name}.Equals(updated.{prop.Name}))");
                sourceBuilder.AppendLine($"                patch.{prop.Name} = updated.{prop.Name};");
            }
        }

        private class ClassToGenerate
        {
            public string SourceClassName { get; }
            public string SourceNamespace { get; }
            public string PatchClassName { get; }
            public string PatchNamespace { get; }
            public string MapperClassName { get; }
            public string MapperNamespace { get; }
            public ImmutableArray<PropertyInfo> Properties { get; }
            public bool IncludeMessagePack { get; }
            public bool IncludeJson { get; }
            public bool IncludeValidation { get; }
            public bool GenerateMapper { get; }

            public ClassToGenerate(
                string sourceClassName,
                string sourceNamespace,
                string patchClassName,
                string patchNamespace,
                string mapperClassName,
                string mapperNamespace,
                ImmutableArray<PropertyInfo> properties,
                bool includeMessagePack,
                bool includeJson,
                bool includeValidation,
                bool generateMapper)
            {
                SourceClassName = sourceClassName;
                SourceNamespace = sourceNamespace;
                PatchClassName = patchClassName;
                PatchNamespace = patchNamespace;
                MapperClassName = mapperClassName;
                MapperNamespace = mapperNamespace;
                Properties = properties;
                IncludeMessagePack = includeMessagePack;
                IncludeJson = includeJson;
                IncludeValidation = includeValidation;
                GenerateMapper = generateMapper;
            }
        }

        private class PropertyInfo
        {
            public string Name { get; }
            public string TypeName { get; }
            public ImmutableArray<AttributeInfo> Attributes { get; }
            public bool IsReferenceType { get; }
            public bool IsCollectionType { get; }
            public bool IsAlreadyNullable { get; }

            public PropertyInfo(
                string name,
                string typeName,
                ImmutableArray<AttributeInfo> attributes,
                bool isReferenceType,
                bool isCollectionType,
                bool isAlreadyNullable)
            {
                Name = name;
                TypeName = typeName;
                Attributes = attributes;
                IsReferenceType = isReferenceType;
                IsCollectionType = isCollectionType;
                IsAlreadyNullable = isAlreadyNullable;
            }
        }

        private class AttributeInfo
        {
            public string Name { get; }
            public string[] Arguments { get; }

            public AttributeInfo(string name, string[] arguments)
            {
                Name = name;
                Arguments = arguments;
            }
        }
    }
}
