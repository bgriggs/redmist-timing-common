using MessagePack;
using System.Reflection;
using System.Text;

namespace RedMist.TimingCommon.TypeScript;

/// <summary>
/// Generates TypeScript decode functions that map MessagePack array positions to typed interface properties.
/// Called from <see cref="SessionStateGenerationSpec"/> during TypeGen code generation.
/// </summary>
internal static class DecodeGenerator
{
    private const string OutputFileName = "decode-functions.ts";

    internal static void Generate(IEnumerable<Type> types, string outputDirectory)
    {
        var classTypes = types.Where(t => t.IsClass && !t.IsAbstract).ToList();
        if (classTypes.Count == 0) return;

        var decodableTypes = new HashSet<Type>(classTypes);

        var sb = new StringBuilder();
        sb.AppendLine("/**");
        sb.AppendLine(" * This is a TypeGen auto-generated file.");
        sb.AppendLine(" * Any changes made to this file can be lost when this file is regenerated.");
        sb.AppendLine(" */");
        sb.AppendLine();

        // Imports for the top-level interface types
        foreach (var type in classTypes)
            sb.AppendLine($"import {{ {type.Name} }} from \"./{ToKebabCase(type.Name)}\";");

        // Imports for any enum or named type referenced in property types but not already imported
        var referencedTypes = classTypes
            .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => (Prop: p, Key: p.GetCustomAttribute<KeyAttribute>()))
                .Where(x => x.Key?.IntKey != null)
                .SelectMany(x => CollectReferencedTypes(x.Prop.PropertyType)))
            .Distinct()
            .Where(t => !classTypes.Contains(t))
            .OrderBy(t => t.Name);

        foreach (var type in referencedTypes)
            sb.AppendLine($"import {{ {type.Name} }} from \"./{ToKebabCase(type.Name)}\";");

        sb.AppendLine();

        foreach (var type in classTypes)
            GenerateDecodeFunction(type, decodableTypes, sb);

        Directory.CreateDirectory(outputDirectory);
        File.WriteAllText(Path.Combine(outputDirectory, OutputFileName), sb.ToString());
    }

    private static IEnumerable<Type> CollectReferencedTypes(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null) return CollectReferencedTypes(underlying);

        if (type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong)
            || type == typeof(short) || type == typeof(ushort) || type == typeof(byte) || type == typeof(sbyte)
            || type == typeof(double) || type == typeof(float) || type == typeof(decimal)
            || type == typeof(string) || type == typeof(bool)
            || type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeOnly))
            return [];

        if (type.IsArray)
            return CollectReferencedTypes(type.GetElementType()!);

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return CollectReferencedTypes(type.GetGenericArguments()[0]);

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            return CollectReferencedTypes(type.GetGenericArguments()[0])
                .Concat(CollectReferencedTypes(type.GetGenericArguments()[1]));

        return [type];
    }

    private static void GenerateDecodeFunction(Type type, HashSet<Type> decodableTypes, StringBuilder sb)
    {
        var keyedProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => (Prop: p, Key: p.GetCustomAttribute<KeyAttribute>()))
            .Where(x => x.Key?.IntKey != null)
            .OrderBy(x => x.Key!.IntKey!.Value)
            .ToList();

        if (keyedProps.Count == 0) return;

        var constFields = type.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .ToList();

        var nullabilityContext = new NullabilityInfoContext();

        sb.AppendLine($"export function decode{type.Name}(a: unknown[]): {type.Name} {{");
        sb.AppendLine("    return {");
        foreach (var field in constFields)
            sb.AppendLine($"        {ToCamelCase(field.Name)}: {FormatConstValue(field.GetRawConstantValue())},");
        foreach (var (prop, key) in keyedProps)
        {
            bool isNullable = Nullable.GetUnderlyingType(prop.PropertyType) != null
                              || nullabilityContext.Create(prop).WriteState == NullabilityState.Nullable;
            sb.AppendLine($"        {ToCamelCase(prop.Name)}: {GetDecodeExpression(prop.PropertyType, $"a[{key!.IntKey}]", decodableTypes, isNullable)},");
        }
        sb.AppendLine("    };");
        sb.AppendLine("}");
        sb.AppendLine();
    }

    /// <summary>
    /// Generates the TypeScript expression to decode a value from a MessagePack array element.
    /// For decodable types, generates recursive decode calls; for primitives, generates simple casts.
    /// </summary>
    private static string GetDecodeExpression(Type type, string accessor, HashSet<Type> decodableTypes, bool isNullable)
    {
        // Nullable<T> value types (int?, bool?, etc.)
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null)
        {
            if (decodableTypes.Contains(underlying))
                return $"{accessor} != null ? decode{underlying.Name}({accessor} as unknown[]) : null";
            return $"{accessor} as {GetTsType(type)}";
        }

        // List<T> or T[] — map with decode function if element type is decodable
        Type? elementType = null;
        if (type.IsArray)
            elementType = type.GetElementType();
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            elementType = type.GetGenericArguments()[0];

        if (elementType != null)
        {
            if (decodableTypes.Contains(elementType))
            {
                var mapExpr = $"({accessor} as unknown[][]).map(decode{elementType.Name})";
                return isNullable ? $"{accessor} != null ? {mapExpr} : null" : mapExpr;
            }
            return $"{accessor} as {GetTsType(type)}";
        }

        // Single decodable class reference
        if (decodableTypes.Contains(type))
        {
            if (isNullable)
                return $"{accessor} != null ? decode{type.Name}({accessor} as unknown[]) : null";
            return $"decode{type.Name}({accessor} as unknown[])";
        }

        // Primitives, enums, dictionaries — simple cast
        return $"{accessor} as {GetTsType(type)}";
    }

    private static string FormatConstValue(object? value) => value switch
    {
        string s => $"\"{s}\"",
        bool b   => b ? "true" : "false",
        _        => value?.ToString() ?? "null"
    };

    private static string GetTsType(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null) return $"{GetTsType(underlying)} | null";

        if (type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong)
            || type == typeof(short) || type == typeof(ushort) || type == typeof(byte) || type == typeof(sbyte)
            || type == typeof(double) || type == typeof(float) || type == typeof(decimal))
            return "number";

        if (type == typeof(string)) return "string";
        if (type == typeof(bool)) return "boolean";
        if (type == typeof(DateTime) || type == typeof(DateTimeOffset)) return "Date";
        if (type == typeof(TimeOnly)) return "string";

        if (type.IsArray)
            return $"{GetTsType(type.GetElementType()!)}[]";

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return $"{GetTsType(type.GetGenericArguments()[0])}[]";

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var args = type.GetGenericArguments();
            return $"{{ [key: {GetTsType(args[0])}]: {GetTsType(args[1])}; }}";
        }

        if (type.IsEnum) return type.Name;

        return type.Name;
    }

    private static string ToCamelCase(string name) =>
        string.IsNullOrEmpty(name) ? name : char.ToLowerInvariant(name[0]) + name[1..];

    private static string ToKebabCase(string name) =>
        string.Concat(name.Select((c, i) => i > 0 && char.IsUpper(c) ? $"-{char.ToLower(c)}" : $"{char.ToLower(c)}"));
}
