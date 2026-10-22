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
        // Keys 0-67: everything up to and including SignalBars.
        var asOlderServerWroteIt = TruncateArray(current, 68);

        var read = MessagePackSerializer.Deserialize<CarPosition>(asOlderServerWroteIt);

        Assert.AreEqual("297", read.EventId);
        Assert.AreEqual(4, read.SignalBars);
        Assert.IsNull(read.GpsHealth);
    }

    /// <summary>
    /// Mimics a reader built before the pit sequence existed, whose highest key is
    /// <see cref="CarPosition.GpsHealth"/> at 68.
    /// </summary>
    [MessagePackObject]
    public class ReaderWithoutPitPhase
    {
        [Key(0)]
        public string? EventId { get; set; }
        [Key(34)]
        public bool IsInPit { get; set; }
        [Key(68)]
        public int? GpsHealth { get; set; }
    }

    [TestMethod]
    public void Serialize_WithPitPhase_IsDecodableByReaderThatStopsAtGpsHealth()
    {
        var bytes = MessagePackSerializer.Serialize(new CarPosition
        {
            EventId = "297",
            IsInPit = true,
            GpsHealth = 3,
            PitPhase = PitPhase.InPitBox,
            PitStopStartTime = new DateTime(2026, 10, 17, 13, 57, 1, DateTimeKind.Utc),
            LastPitStopDurationMs = 80_000,
        });

        var older = MessagePackSerializer.Deserialize<ReaderWithoutPitPhase>(bytes);

        Assert.AreEqual("297", older.EventId);
        Assert.IsTrue(older.IsInPit);
        Assert.AreEqual(3, older.GpsHealth);
    }

    [TestMethod]
    public void Deserialize_PayloadWrittenBeforePitPhaseExisted_LeavesItUnset()
    {
        var current = MessagePackSerializer.Serialize(new CarPosition
        {
            EventId = "297",
            GpsHealth = 3,
            PitPhase = PitPhase.ExitingPit,
            PitStopStartTime = DateTime.UtcNow,
            LastPitStopDurationMs = 1,
        });
        // Keys 0-68: everything up to and including GpsHealth.
        var asOlderServerWroteIt = TruncateArray(current, 69);

        var read = MessagePackSerializer.Deserialize<CarPosition>(asOlderServerWroteIt);

        Assert.AreEqual(3, read.GpsHealth);
        Assert.AreEqual(PitPhase.None, read.PitPhase);
        Assert.IsNull(read.PitStopStartTime);
        Assert.IsNull(read.LastPitStopDurationMs);
    }

    [TestMethod]
    public void PitPhase_RoundTripsInFullStateAndPatch()
    {
        var start = new DateTime(2026, 10, 17, 13, 57, 1, DateTimeKind.Utc);
        var car = MessagePackSerializer.Deserialize<CarPosition>(MessagePackSerializer.Serialize(new CarPosition
        {
            PitPhase = PitPhase.InPitBox,
            PitStopStartTime = start,
            LastPitStopDurationMs = 42_000,
        }));
        Assert.AreEqual(PitPhase.InPitBox, car.PitPhase);
        Assert.AreEqual(start, car.PitStopStartTime);
        Assert.AreEqual(42_000, car.LastPitStopDurationMs);

        var patch = MessagePackSerializer.Deserialize<CarPositionPatch>(MessagePackSerializer.Serialize(new CarPositionPatch
        {
            Number = "7",
            PitPhase = PitPhase.ExitedPit,
            LastPitStopDurationMs = 42_000,
        }));
        Assert.AreEqual(PitPhase.ExitedPit, patch.PitPhase);
        Assert.AreEqual(42_000, patch.LastPitStopDurationMs);
        Assert.IsNull(patch.PitStopStartTime);
    }

    /// <summary>
    /// Rewrites a keyed payload keeping only its first <paramref name="keep"/> elements, which is
    /// exactly how the same object looked before the later members were added.
    /// </summary>
    private static byte[] TruncateArray(byte[] bytes, int keep)
    {
        var reader = new MessagePackReader(bytes);
        var count = reader.ReadArrayHeader();
        if (keep >= count)
            throw new ArgumentOutOfRangeException(nameof(keep), $"Payload has {count} elements; keeping {keep} would not truncate it.");

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        writer.WriteArrayHeader(keep);
        for (int i = 0; i < keep; i++)
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