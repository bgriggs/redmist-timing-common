using MessagePack;
using MessagePack.Resolvers;
using RedMist.TimingCommon.Models;
using System.Collections;
using System.Reflection;

namespace RedMist.TimingCommon.Tests.Models;

/// <summary>
/// Covers the bytes on the wire for the patch types, which now have a hand-emitted formatter.
/// </summary>
/// <remarks>
/// Until PatchClassGenerator started writing formatters, these types were serialized by
/// DynamicObjectResolver, which builds one at runtime from the [Key] attributes. The deployed
/// backend encodes them that way, and so does every shipped Android and desktop build - iOS is the
/// exception, because it never used MessagePack for the hub at all. The generated formatter has to
/// agree with the dynamic one byte for byte or a new client cannot read an old server, and the
/// failure would look like corrupt timing rather than a protocol change.
///
/// So these tests do not assert a hand-written expected encoding. They serialize the same instance
/// through both formatters and compare, which is the actual compatibility question.
/// </remarks>
[TestClass]
public class PatchWireCompatTests
{
    /// <summary>
    /// Forces the old path: DynamicObjectResolver is asked first, so it wins for the patch type even
    /// though the class now carries [MessagePackFormatter] - that attribute is read by
    /// AttributeFormatterResolver, and DynamicObjectResolver only ever looks at [Key].
    /// </summary>
    /// <remarks>
    /// Being asked first, it also wins for any nested [MessagePackObject] - EventEntry and the like
    /// get a runtime formatter here rather than their source-generated one. Only types it cannot
    /// build, such as List and Dictionary, fall through. The two agree, which is part of what these
    /// tests establish, but the chain is not "dynamic for the patch and standard for everything
    /// else".
    /// </remarks>
    private static readonly MessagePackSerializerOptions ViaDynamic =
        MessagePackSerializerOptions.Standard.WithResolver(
            CompositeResolver.Create(DynamicObjectResolver.Instance, StandardResolver.Instance));

    /// <summary>The new path: the standard chain, where AttributeFormatterResolver now answers.</summary>
    private static readonly MessagePackSerializerOptions ViaGenerated = MessagePackSerializerOptions.Standard;

    /// <summary>
    /// Sets every property it knows how to, so the comparison covers each slot's encoding rather
    /// than a run of nils. Anything it cannot build is left null, which is still a slot on the wire.
    /// </summary>
    private static int Populate<T>(T target) where T : notnull
    {
        var filled = 0;
        foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!p.CanWrite)
                continue;

            var value = SampleFor(p.PropertyType, p.Name);
            if (value is null)
                continue;

            p.SetValue(target, value);
            filled++;
        }

        return filled;
    }

    /// <summary>
    /// A stable per-name number. Not string.GetHashCode, which is salted per process and would make
    /// which slots share a value change from run to run.
    /// </summary>
    private static int Seed(string name)
    {
        var acc = 7;
        foreach (var c in name)
            acc = (acc * 31 + c) % 9973;
        return acc + 1;
    }

    private static object? SampleFor(Type type, string seedName)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        // Vary by name so that transposing two same-typed slots is likely to show up. With 16 bools
        // on CarPositionPatch it cannot be certain, which is why the slot order is generated from
        // the [Key] values rather than trusted to this.
        var seed = Seed(seedName);

        if (underlying.IsEnum)
        {
            var values = Enum.GetValues(underlying);
            return values.Length > 0 ? values.GetValue(seed % values.Length) : null;
        }

        if (underlying == typeof(string)) return "v" + seed;
        if (underlying == typeof(bool)) return seed % 2 == 0;
        if (underlying == typeof(int)) return seed;
        if (underlying == typeof(uint)) return (uint)seed;
        if (underlying == typeof(long)) return (long)seed;
        if (underlying == typeof(ulong)) return (ulong)seed;
        if (underlying == typeof(short)) return (short)seed;
        if (underlying == typeof(ushort)) return (ushort)seed;
        if (underlying == typeof(byte)) return (byte)(seed % 256);
        if (underlying == typeof(sbyte)) return (sbyte)(seed % 128);
        if (underlying == typeof(char)) return (char)('a' + seed % 26);
        if (underlying == typeof(double)) return seed + 0.5d;
        if (underlying == typeof(float)) return seed + 0.5f;
        if (underlying == typeof(decimal)) return seed + 0.5m;
        if (underlying == typeof(Guid)) return new Guid(seed, 0, 0, new byte[8]);
        if (underlying == typeof(DateTime)) return new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMinutes(seed % 1000);
        if (underlying == typeof(DateTimeOffset)) return new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero).AddMinutes(seed % 1000);
        if (underlying == typeof(TimeSpan)) return TimeSpan.FromSeconds(seed % 1000);
        if (underlying == typeof(DateOnly)) return new DateOnly(2026, 1, 1).AddDays(seed % 300);
        if (underlying == typeof(TimeOnly)) return new TimeOnly(0, 0).AddMinutes(seed % 1000);

        // Collections get an element where one can be built, so the element formatter is compared
        // too and not just the empty-array header.
        if (typeof(IEnumerable).IsAssignableFrom(underlying) && underlying != typeof(string))
        {
            return PopulatedCollection(underlying);
        }

        // A nested object, built empty. Its own formatter still has to encode it identically.
        try { return Activator.CreateInstance(underlying); } catch { return null; }
    }

    private static object? PopulatedCollection(Type collectionType)
    {
        object? instance;
        try { instance = Activator.CreateInstance(collectionType); } catch { return null; }
        if (instance is null)
            return null;

        if (instance is IDictionary dictionary && collectionType.IsGenericType)
        {
            var args = collectionType.GetGenericArguments();
            var key = SampleFor(args[0], "dictionaryKey");
            var val = SampleFor(args[1], "dictionaryValue");
            if (key is not null && val is not null)
                dictionary[key] = val;
            return instance;
        }

        if (instance is IList list && collectionType.IsGenericType)
        {
            var element = SampleFor(collectionType.GetGenericArguments()[0], "listElement");
            if (element is not null)
                list.Add(element);
        }

        return instance;
    }

    /// <summary>
    /// A patch big enough that any per-member framing shows up. Compression only engages past
    /// MessagePackSerializerOptions' 64 byte minimum, so a small patch hides the whole class of bug.
    /// </summary>
    private static SessionStatePatch LargeSessionPatch() => new()
    {
        EventId = 3,
        EventName = new string('e', 400),
        EventEntries = [.. Enumerable.Range(1, 60).Select(i => new EventEntry { Number = i.ToString(), Name = "Car " + i })],
    };

    private static void AssertSameBytes<T>(T value, int filled) where T : notnull
    {
        // The count is asserted against what the type actually has, so a filler that quietly stops
        // populating a category cannot pass. Two CarPositionPatch members are deliberately excluded.
        var writable = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Count(p => p.CanWrite);
        Assert.IsGreaterThanOrEqualTo(writable - 2, filled,
            $"Only {filled} of {writable} properties were populated, so this compares more nils than values.");

        var oldWay = MessagePackSerializer.Serialize(value, ViaDynamic);
        var newWay = MessagePackSerializer.Serialize(value, ViaGenerated);

        CollectionAssert.AreEqual(oldWay, newWay,
            $"The generated formatter for {typeof(T).Name} encodes differently from the dynamic one " +
            $"every deployed build uses ({oldWay.Length} bytes vs {newWay.Length}).");
    }

    [TestMethod]
    public void TheSessionPatchEncodesAsItAlwaysHas()
    {
        var patch = new SessionStatePatch();
        AssertSameBytes(patch, Populate(patch));
    }

    [TestMethod]
    public void TheCarPatchEncodesAsItAlwaysHas()
    {
        var patch = new CarPositionPatch();
        AssertSameBytes(patch, Populate(patch));
    }

    [TestMethod]
    public void AnEmptySessionPatchEncodesAsItAlwaysHas()
    {
        // The common case on the wire: a patch touching one field, every other slot nil.
        var patch = new SessionStatePatch { EventId = 12 };

        CollectionAssert.AreEqual(
            MessagePackSerializer.Serialize(patch, ViaDynamic),
            MessagePackSerializer.Serialize(patch, ViaGenerated));
    }

    [DataTestMethod]
    [DataRow(MessagePackCompression.Lz4Block)]
    [DataRow(MessagePackCompression.Lz4BlockArray)]
    public void ACompressedPatchEncodesAsItAlwaysHas(MessagePackCompression compression)
    {
        // A formatter must write only its member, never route back through MessagePackSerializer's
        // top-level entry point. That entry point applies compression, so a member over the 64 byte
        // minimum would get its own LZ4 block nested inside the one the outer call already made -
        // unreadable to any other peer, and silent until a payload gets big enough to trip it.
        var patch = LargeSessionPatch();

        var oldWay = MessagePackSerializer.Serialize(patch, ViaDynamic.WithCompression(compression));
        var newWay = MessagePackSerializer.Serialize(patch, ViaGenerated.WithCompression(compression));

        CollectionAssert.AreEqual(oldWay, newWay,
            $"Under {compression} the generated formatter produced {newWay.Length} bytes against the " +
            $"dynamic formatter's {oldWay.Length}. A member is being framed individually.");
    }

    [DataTestMethod]
    [DataRow(MessagePackCompression.Lz4Block)]
    [DataRow(MessagePackCompression.Lz4BlockArray)]
    public void ACompressedPatchIsReadableByTheOldFormatter(MessagePackCompression compression)
    {
        var patch = LargeSessionPatch();

        var written = MessagePackSerializer.Serialize(patch, ViaGenerated.WithCompression(compression));
        var read = MessagePackSerializer.Deserialize<SessionStatePatch>(written, ViaDynamic.WithCompression(compression));

        Assert.AreEqual(patch.EventName, read.EventName);
        Assert.AreEqual(60, read.EventEntries!.Count);
    }

    [TestMethod]
    public void TheOldFormatterCanStillReadTheNewOnesOutput()
    {
        var patch = new SessionStatePatch();
        Populate(patch);

        var written = MessagePackSerializer.Serialize(patch, ViaGenerated);
        var read = MessagePackSerializer.Deserialize<SessionStatePatch>(written, ViaDynamic);

        Assert.AreEqual(patch.EventId, read.EventId);
        Assert.AreEqual(patch.EventName, read.EventName);
        Assert.AreEqual(patch.SessionId, read.SessionId);
    }

    [TestMethod]
    public void TheNewFormatterCanStillReadTheOldOnesOutput()
    {
        var patch = new CarPositionPatch();
        Populate(patch);

        var written = MessagePackSerializer.Serialize(patch, ViaDynamic);
        var read = MessagePackSerializer.Deserialize<CarPositionPatch>(written, ViaGenerated);

        Assert.AreEqual(patch.EventId, read.EventId);
        Assert.AreEqual(patch.Number, read.Number);
        Assert.AreEqual(patch.LastLapCompleted, read.LastLapCompleted);
    }
}
