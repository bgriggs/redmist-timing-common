using MessagePack;
using RedMist.TimingCommon.Attributes;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.NativeGen;

/// <summary>
/// A wire property as the apps see it: its MessagePack array slot (when the type is keyed),
/// its JSON name, and the C# default an empty instance carries.
/// </summary>
internal sealed record PropModel(
    PropertyInfo Info,
    string Name,
    Type ClrType,
    bool IsNullable,
    int? Key,
    string JsonKey,
    string MapKey,
    object? DefaultValue,
    bool ElementNullable = false);

internal sealed record ConstModel(string Name, Type ClrType, object? Value);

/// <summary>
/// A class to emit. <see cref="IsKeyed"/> means MessagePack writes it as an array indexed by
/// <c>[Key(n)]</c>; otherwise it is contractless and written as a map of property names.
/// </summary>
internal sealed record TypeModel(
    Type Clr,
    string Name,
    bool IsKeyed,
    IReadOnlyList<PropModel> Props,
    IReadOnlyList<ConstModel> Consts,
    TypeModel? PatchOf)
{
    public TypeModel? PatchOf { get; set; } = PatchOf;
}

internal sealed record EnumModel(Type Clr, string Name, IReadOnlyList<(string Name, long Value)> Members);

/// <summary>
/// Walks the wire types the mobile apps need, starting from <see cref="Roots"/> and following
/// every property, so a model added to a root's graph is picked up without touching this list.
/// </summary>
internal sealed class TypeCatalog
{
    /// <summary>
    /// Everything the apps send or receive: REST results, hub payloads, and the patch types.
    /// </summary>
    internal static readonly Type[] Roots =
    [
        typeof(Models.SessionState),
        typeof(Models.SessionStatePatch),
        typeof(Models.CarPosition),
        typeof(Models.CarPositionPatch),
        typeof(Models.EventListSummary),
        typeof(Models.Event),
        typeof(Models.Session),
        typeof(Models.CompetitorMetadata),
        typeof(Models.ControlLogEntry),
        typeof(Models.CarControlLogs),
        typeof(Models.FlagDuration),
        typeof(Models.SponsorInfo),
        typeof(Models.InCarDriverMode.InCarPayload),
        typeof(Models.InCarDriverMode.CarStatus),
        typeof(Models.InCarVideo.VideoMetadata),
        typeof(Models.InCarVideo.VideoStatus),
        typeof(UIVersionInfo),
    ];

    private readonly Assembly assembly = typeof(Models.SessionState).Assembly;
    private readonly List<Type> classOrder = [];
    private readonly HashSet<Type> enumSet = [];

    public List<TypeModel> Classes { get; } = [];
    public List<EnumModel> Enums { get; } = [];
    public Dictionary<Type, string> Names { get; } = [];

    public static TypeCatalog Build()
    {
        var catalog = new TypeCatalog();
        foreach (var root in Roots)
            catalog.Visit(root);
        catalog.AssignNames();
        catalog.BuildModels();
        return catalog;
    }

    public bool IsClass(Type t) => Names.ContainsKey(t) && !t.IsEnum;

    private void Visit(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        if (type.IsArray)
        {
            Visit(type.GetElementType()!);
            return;
        }
        if (type.IsGenericType)
        {
            foreach (var arg in type.GetGenericArguments())
                Visit(arg);
            return;
        }
        if (type.Assembly != assembly)
            return;
        if (type.IsEnum)
        {
            enumSet.Add(type);
            return;
        }
        if (!type.IsClass || classOrder.Contains(type))
            return;

        classOrder.Add(type);
        foreach (var prop in WireProperties(type))
            Visit(prop.PropertyType);
    }

    /// <summary>
    /// Names are the C# simple names. Where two types share one (Models.Event and
    /// Models.Configuration.Event), the less central one is prefixed with its namespace leaf.
    /// </summary>
    private void AssignNames()
    {
        var all = classOrder.Concat(enumSet).ToList();
        foreach (var group in all.GroupBy(t => t.Name))
        {
            var types = group.ToList();
            if (types.Count == 1)
            {
                Names[types[0]] = Naming.TypeRenames.GetValueOrDefault(types[0].Name, types[0].Name);
                continue;
            }
            foreach (var t in types)
            {
                var leaf = t.Namespace!.Split('.').Last();
                Names[t] = leaf == "Models" ? t.Name : leaf + t.Name;
            }
        }
    }

    private void BuildModels()
    {
        foreach (var e in enumSet.OrderBy(t => Names[t], StringComparer.Ordinal))
        {
            var members = Enum.GetNames(e)
                .Select(n => (n, Convert.ToInt64(Enum.Parse(e, n))))
                .ToList();
            Enums.Add(new EnumModel(e, Names[e], members));
        }

        var byType = new Dictionary<Type, TypeModel>();
        foreach (var t in classOrder.OrderBy(t => Names[t], StringComparer.Ordinal))
        {
            var model = BuildClass(t);
            byType[t] = model;
            Classes.Add(model);
        }

        // A patch class is SomethingPatch generated from a [GeneratePatch] Something.
        foreach (var model in Classes)
        {
            if (!model.Clr.Name.EndsWith("Patch", StringComparison.Ordinal))
                continue;
            var source = byType.Keys.FirstOrDefault(t =>
                t.Name + "Patch" == model.Clr.Name && t.GetCustomAttribute<GeneratePatchAttribute>() != null);
            if (source != null)
                model.PatchOf = byType[source];
        }
    }

    private TypeModel BuildClass(Type type)
    {
        var isKeyed = type.GetCustomAttribute<MessagePackObjectAttribute>() is { KeyAsPropertyName: false };
        var nullability = new NullabilityInfoContext();
        object? empty = null;
        try
        {
            empty = Activator.CreateInstance(type);
        }
        catch (MissingMethodException)
        {
            // No parameterless constructor: defaults fall back to the type's zero value.
        }

        var props = new List<PropModel>();
        foreach (var p in WireProperties(type))
        {
            var keyAttr = p.GetCustomAttribute<KeyAttribute>();
            var key = keyAttr?.IntKey;
            if (isKeyed && key == null)
                continue; // Not on the MessagePack wire; JSON-only members are not used by the apps.
            if (p.PropertyType == typeof(byte[]))
                continue; // Binary blobs (Organization.Logo) are not used by the apps; logos come from the CDN.

            var isNullable = Nullable.GetUnderlyingType(p.PropertyType) != null
                             || (!p.PropertyType.IsValueType && nullability.Create(p).ReadState == NullabilityState.Nullable);
            var jsonKey = p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? JsonNamingPolicy.CamelCase.ConvertName(p.Name);
            props.Add(new PropModel(p, p.Name, p.PropertyType, isNullable, key, jsonKey, keyAttr?.StringKey ?? p.Name, empty != null ? p.GetValue(empty) : null,
                ElementNullable(p, nullability)));
        }

        if (isKeyed)
            props.Sort((a, b) => a.Key!.Value.CompareTo(b.Key!.Value));

        var consts = type.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .OrderBy(f => f.MetadataToken)
            .Select(f => new ConstModel(f.Name, f.FieldType, f.GetRawConstantValue()))
            .ToList();

        return new TypeModel(type, Names[type], isKeyed, props, consts, null);
    }

    /// <summary>
    /// Whether a list's elements, or a dictionary's values, may be null: <c>List&lt;CarStatus?&gt;</c>
    /// holds nulls on the wire, and the generated element type must be optional to decode it.
    /// </summary>
    internal static bool ElementNullable(PropertyInfo p, NullabilityInfoContext context)
    {
        var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
        Type? element = KotlinEmitter.ElementType(type);
        var index = 0;
        if (element == null && KotlinEmitter.IsDictionary(type, out _, out var value))
        {
            element = value;
            index = 1;
        }
        if (element == null) return false;
        if (Nullable.GetUnderlyingType(element) != null) return true;
        var info = context.Create(p);
        var args = type.IsArray ? (info.ElementType != null ? [info.ElementType] : []) : info.GenericTypeArguments;
        return args.Length > index && args[index].ReadState == NullabilityState.Nullable;
    }

    internal static IEnumerable<PropertyInfo> WireProperties(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .Where(p => p.GetCustomAttribute<IgnoreMemberAttribute>() == null)
            .Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() is not { Condition: JsonIgnoreCondition.Always })
            .OrderBy(p => p.MetadataToken);
}

internal static class Naming
{
    public static string LowerFirst(string name) =>
        string.IsNullOrEmpty(name) ? name : char.ToLowerInvariant(name[0]) + name[1..];

    /// <summary>
    /// Lower-camel member name for Swift and Kotlin. Leading acronyms are lowered as a run
    /// (<c>URL</c> becomes <c>url</c>, <c>IOSVersion</c> becomes <c>iosVersion</c>), and a name that
    /// is a keyword in either language gets a readable replacement.
    /// </summary>
    public static string Member(string name)
    {
        if (Renames.TryGetValue(name, out var renamed))
            return renamed;

        var upperRun = 0;
        while (upperRun < name.Length && char.IsUpper(name[upperRun]))
            upperRun++;

        string result;
        if (upperRun <= 1)
            result = LowerFirst(name);
        else if (upperRun == name.Length)
            result = name.ToLowerInvariant();
        else
            result = name[..(upperRun - 1)].ToLowerInvariant() + name[(upperRun - 1)..];

        return Keywords.Contains(result) ? result + "Value" : result;
    }

    /// <summary>
    /// Enum case names. <c>None</c> becomes <c>notSet</c>: in Swift <c>.none</c> on an optional
    /// compares with nil instead of the case.
    /// </summary>
    public static string EnumCase(string name) => name == "None" ? "notSet" : Member(name);

    /// <summary>
    /// Type names that would shadow platform types the apps use (SwiftUI's Section).
    /// </summary>
    public static readonly Dictionary<string, string> TypeRenames = new(StringComparer.Ordinal)
    {
        ["Section"] = "TrackSection",
    };

    /// <summary>Normalizes generated text to LF so output is identical on Windows and macOS.</summary>
    public static string Lf(string text) => text.Replace("\r\n", "\n");

    private static readonly Dictionary<string, string> Renames = new(StringComparer.Ordinal)
    {
        ["Class"] = "className",
    };

    // Hard keywords in Kotlin or Swift that a C# property name could collide with.
    private static readonly HashSet<string> Keywords = new(StringComparer.Ordinal)
    {
        "as", "break", "class", "continue", "do", "else", "false", "for", "fun", "if", "in",
        "interface", "is", "null", "object", "package", "return", "super", "this", "throw",
        "true", "try", "typealias", "typeof", "val", "var", "when", "while",
        "associatedtype", "case", "default", "defer", "deinit", "enum", "extension", "fallthrough",
        "func", "guard", "import", "init", "inout", "internal", "let", "operator", "private",
        "protocol", "public", "repeat", "rethrows", "self", "static", "struct", "subscript",
        "switch", "throws", "where", "nil",
    };
}
