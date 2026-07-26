using MessagePack;
using RedMist.TimingCommon.Models;

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
}