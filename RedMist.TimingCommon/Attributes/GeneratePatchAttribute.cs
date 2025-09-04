namespace RedMist.TimingCommon.Attributes;

/// <summary>
/// Generates a patch variant of the target class where all properties become nullable
/// to support partial updates via delta/patch operations.
/// Also generates a Mapperly mapper for efficient patch application.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class GeneratePatchAttribute : Attribute
{
    /// <summary>
    /// Optional custom name for the generated patch class.
    /// If not specified, defaults to "{ClassName}Patch".
    /// </summary>
    public string? PatchClassName { get; set; }

    /// <summary>
    /// Optional custom namespace for the generated patch class.
    /// If not specified, uses the same namespace as the source class.
    /// </summary>
    public string? PatchNamespace { get; set; }

    /// <summary>
    /// Optional custom name for the generated mapper class.
    /// If not specified, defaults to "{ClassName}Mapper".
    /// </summary>
    public string? MapperClassName { get; set; }

    /// <summary>
    /// Optional custom namespace for the generated mapper class.
    /// If not specified, defaults to "{SourceNamespace}.Mappers".
    /// </summary>
    public string? MapperNamespace { get; set; }

    /// <summary>
    /// Whether to include MessagePack attributes in the generated patch class.
    /// Defaults to true.
    /// </summary>
    public bool IncludeMessagePackAttributes { get; set; } = true;

    /// <summary>
    /// Whether to include JSON attributes in the generated patch class.
    /// Defaults to true.
    /// </summary>
    public bool IncludeJsonAttributes { get; set; } = true;

    /// <summary>
    /// Whether to include validation attributes (like MaxLength) in the generated patch class.
    /// Defaults to true.
    /// </summary>
    public bool IncludeValidationAttributes { get; set; } = true;

    /// <summary>
    /// Whether to generate the Mapperly mapper class.
    /// Defaults to true.
    /// </summary>
    public bool GenerateMapper { get; set; } = true;
}