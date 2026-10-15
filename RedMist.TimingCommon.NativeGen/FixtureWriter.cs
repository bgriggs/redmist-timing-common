using MessagePack;
using MessagePack.Resolvers;
using System.Collections;
using System.Text.Json;

namespace RedMist.TimingCommon.NativeGen;

/// <summary>
/// Writes one sample of every catalog type, serialized the way the server does it: MessagePack
/// with the hub's ContractlessStandardResolver, and JSON with SignalR's camelCase defaults. Every
/// member is filled with a distinct non-default value, so an app test that decodes both files and
/// compares them catches a member read from the wrong slot or name, or not read at all.
/// </summary>
internal sealed class FixtureWriter(TypeCatalog catalog)
{
    private static readonly MessagePackSerializerOptions MessagePackOptions =
        MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true, NewLine = "\n" };

    private int counter;

    public void Write(string directory)
    {
        Directory.CreateDirectory(directory);
        foreach (var model in catalog.Classes)
        {
            counter = 0;
            var sample = Populate(model.Clr, depth: 0);
            File.WriteAllBytes(Path.Combine(directory, $"{model.Name}.msgpack"),
                MessagePackSerializer.Serialize(model.Clr, sample, MessagePackOptions));
            File.WriteAllText(Path.Combine(directory, $"{model.Name}.json"),
                JsonSerializer.Serialize(sample, model.Clr, JsonOptions) + "\n");

            // A fresh instance: every nullable member null and the rest at their C# defaults, which
            // the generated models must reproduce.
            var empty = Activator.CreateInstance(model.Clr)!;
            File.WriteAllBytes(Path.Combine(directory, $"{model.Name}.default.msgpack"),
                MessagePackSerializer.Serialize(model.Clr, empty, MessagePackOptions));
            File.WriteAllText(Path.Combine(directory, $"{model.Name}.default.json"),
                JsonSerializer.Serialize(empty, model.Clr, JsonOptions) + "\n");
        }
    }

    private readonly System.Reflection.NullabilityInfoContext nullability = new();

    private object? Populate(Type type, int depth, bool elementNullable = false)
    {
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null) return Populate(underlying, depth, elementNullable);

        var n = ++counter;
        if (type == typeof(int)) return n;
        if (type == typeof(long)) return 10_000_000_000L + n;
        if (type == typeof(uint)) return (uint)(3_000_000_000 + n);
        if (type == typeof(ulong)) return (ulong)n;
        if (type == typeof(short)) return (short)n;
        if (type == typeof(ushort)) return (ushort)n;
        if (type == typeof(byte)) return (byte)n;
        if (type == typeof(sbyte)) return (sbyte)n;
        if (type == typeof(double)) return n + 0.25;
        if (type == typeof(float)) return n + 0.5f;
        if (type == typeof(decimal)) return n + 0.75m;
        if (type == typeof(bool)) return n % 2 == 0;
        if (type == typeof(string)) return $"s{n}";
        // Alternate UTC with millisecond precision ("...Z") and Unspecified with full tick precision
        // (no zone, 7 fraction digits): the server sends both shapes.
        if (type == typeof(DateTime))
            return n % 2 == 0
                ? new DateTime(2026, 5, 17, 14, 30, 0, DateTimeKind.Utc).AddSeconds(n).AddMilliseconds(125)
                : new DateTime(2026, 5, 17, 14, 30, 0, DateTimeKind.Unspecified).AddSeconds(n).AddTicks(1_234_567);
        if (type == typeof(TimeOnly))
            return n % 2 == 0
                ? new TimeOnly(13, 5, 7).Add(TimeSpan.FromSeconds(n))
                : new TimeOnly(13, 5, 7).Add(TimeSpan.FromTicks(n * 10_000_000L + 1_234_500));
        if (type.IsEnum)
        {
            var values = Enum.GetValues(type);
            return values.GetValue(Math.Min(1 + n % (values.Length - 1 == 0 ? 1 : values.Length - 1), values.Length - 1));
        }

        if (KotlinEmitter.ElementType(type) is { } element)
        {
            var listType = typeof(List<>).MakeGenericType(element);
            var list = (IList)Activator.CreateInstance(listType)!;
            if (depth < 3)
            {
                list.Add(Populate(element, depth + 1));
                // Only where the model allows it, as the server would only send it there.
                if (elementNullable) list.Add(null);
            }
            if (type.IsArray)
            {
                var array = Array.CreateInstance(element, list.Count);
                list.CopyTo(array, 0);
                return array;
            }
            return list;
        }

        if (KotlinEmitter.IsDictionary(type, out var key, out var value))
        {
            var dict = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(key, value))!;
            if (depth < 3)
                dict[Populate(key, depth + 1)!] = Populate(value, depth + 1);
            return dict;
        }

        if (catalog.IsClass(type))
        {
            var instance = Activator.CreateInstance(type)!;
            if (depth >= 3) return instance;
            foreach (var p in TypeCatalog.WireProperties(type).Where(p => p.CanWrite))
                p.SetValue(instance, Populate(p.PropertyType, depth + 1, TypeCatalog.ElementNullable(p, nullability)));
            return instance;
        }

        throw new NotSupportedException($"Cannot build a sample {type}");
    }
}
