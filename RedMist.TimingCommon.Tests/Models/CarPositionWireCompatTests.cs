using MessagePack;
using RedMist.TimingCommon.Models;
using System.Buffers;

namespace RedMist.TimingCommon.Tests.Models;

[TestClass]
public class CarPositionWireCompatTests
{
    /// <summary>
    /// Mimics how readers built against 1.8.0-1.12.0 see key 61: a non-nullable bool decoded
    /// positionally. If the member at key 61 is ever removed again, serialization writes nil
    /// in that slot and this reader fails with "Unexpected msgpack code 192 (nil)".
    /// </summary>
    [MessagePackObject]
    public class LegacyBoolKey61Reader
    {
        [Key(0)]
        public string? EventId { get; set; }
        [Key(61)]
        public bool HasGps { get; set; }
    }

    [TestMethod]
    public void Serialize_CurrentContract_IsDecodableByLegacyBoolKey61Reader()
    {
        var bytes = MessagePackSerializer.Serialize(new CarPosition { EventId = "297" });

        var legacy = MessagePackSerializer.Deserialize<LegacyBoolKey61Reader>(bytes);

        Assert.AreEqual("297", legacy.EventId);
        Assert.IsFalse(legacy.HasGps);
    }

    /// <summary>
    /// Mimics a reader built before <see cref="CarPosition.GpsHealth"/> existed, whose highest key
    /// is <see cref="CarPosition.SignalBars"/> at 67.
    /// </summary>
    [MessagePackObject]
    public class ReaderWithoutGpsHealth
    {
        [Key(0)]
        public string? EventId { get; set; }
        [Key(67)]
        public int? SignalBars { get; set; }
    }

    /// <summary>
    /// A member appended past the last key must not disturb readers that stop short of it. This is
    /// the safe direction the key 61 incident established, and the rollout depends on it: clients
    /// run against a server carrying the new members long before they are rebuilt to read them.
    /// </summary>
    [TestMethod]
    public void Serialize_WithGpsHealth_IsDecodableByReaderThatStopsAtSignalBars()
    {
        var bytes = MessagePackSerializer.Serialize(new CarPosition { EventId = "297", SignalBars = 4, GpsHealth = 2 });

        var older = MessagePackSerializer.Deserialize<ReaderWithoutGpsHealth>(bytes);

        Assert.AreEqual("297", older.EventId);
        Assert.AreEqual(4, older.SignalBars);
    }

    /// <summary>
    /// The other direction, which the staggered rollout also passes through: a client built with
    /// the new member reading a server that predates it. An older server writes a shorter array,
    /// so the new member simply is not there - it must read as null, not throw.
    /// </summary>
    [TestMethod]
    public void Deserialize_PayloadWrittenBeforeGpsHealthExisted_LeavesItNull()
    {
        var current = MessagePackSerializer.Serialize(new CarPosition { EventId = "297", SignalBars = 4, GpsHealth = 2 });
        var asOlderServerWroteIt = DropLastArrayElement(current);

        var read = MessagePackSerializer.Deserialize<CarPosition>(asOlderServerWroteIt);

        Assert.AreEqual("297", read.EventId);
        Assert.AreEqual(4, read.SignalBars);
        Assert.IsNull(read.GpsHealth);
    }

    /// <summary>
    /// Rewrites a keyed payload one element shorter, which is exactly how the same object looked
    /// before its last member was added.
    /// </summary>
    private static byte[] DropLastArrayElement(byte[] bytes)
    {
        var reader = new MessagePackReader(bytes);
        var count = reader.ReadArrayHeader();

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteArrayHeader(count - 1);
        for (int i = 0; i < count - 1; i++)
        {
            var raw = reader.ReadRaw();
            foreach (var segment in raw)
                writer.WriteRaw(segment.Span);
        }
        writer.Flush();
        return buffer.WrittenSpan.ToArray();
    }

    [TestMethod]
    public void SessionState_GpsSourceHealth_RoundTrips()
    {
        var bytes = MessagePackSerializer.Serialize(new SessionState { SessionId = 3, GpsSourceHealth = 3 });

        var round = MessagePackSerializer.Deserialize<SessionState>(bytes);

        Assert.AreEqual(3, round.SessionId);
        Assert.AreEqual(3, round.GpsSourceHealth);
    }
}