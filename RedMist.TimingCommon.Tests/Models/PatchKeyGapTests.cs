using MessagePack;
using MessagePack.Resolvers;
using RedMist.TimingCommon.Attributes;

namespace RedMist.TimingCommon.Tests.Models;

/// <summary>
/// A model with two removed keys, existing only to give the patch generator a non-contiguous shape
/// to emit. Top level rather than nested, because the generated mapper refers to it by simple name
/// and cannot see into a containing class.
/// </summary>
[GeneratePatch]
[MessagePackObject]
public class GapModel
{
    [Key(0)]
    public int First { get; set; }

    // 1 and 2 are deliberately absent, as if two members had been removed.
    [Key(3)]
    public string? Fourth { get; set; }

    [Key(5)]
    public bool Sixth { get; set; }
}

/// <summary>
/// Covers a patch whose MessagePack keys are not contiguous.
/// </summary>
/// <remarks>
/// Neither shipping model has a gap today - CarPosition runs 0-68 and SessionState 0-35 - so nothing
/// in the library exercises the branch that writes nil into an unclaimed slot. That branch is what
/// keeps slot numbering stable when a member is removed, which has already happened once here: see
/// CarPositionWireCompatTests on key 61. Getting it wrong would shift every later member by one and
/// silently mis-decode against a deployed peer.
/// </remarks>
[TestClass]
public class PatchKeyGapTests
{
    private static readonly MessagePackSerializerOptions ViaDynamic =
        MessagePackSerializerOptions.Standard.WithResolver(
            CompositeResolver.Create(DynamicObjectResolver.Instance, StandardResolver.Instance));

    private static readonly MessagePackSerializerOptions ViaGenerated = MessagePackSerializerOptions.Standard;

    private static GapModelPatch Sample() => new() { First = 9, Fourth = "nine", Sixth = true };

    [TestMethod]
    public void AGapInTheKeysEncodesTheSameAsTheDynamicFormatter()
    {
        CollectionAssert.AreEqual(
            MessagePackSerializer.Serialize(Sample(), ViaDynamic),
            MessagePackSerializer.Serialize(Sample(), ViaGenerated),
            "A skipped key has to leave its slot in place, or every later member shifts.");
    }

    [TestMethod]
    public void AGapInTheKeysStillOccupiesItsSlot()
    {
        // Six slots for three members: the two holes are written, not skipped.
        var reader = new MessagePackReader(MessagePackSerializer.Serialize(Sample(), ViaGenerated));

        Assert.AreEqual(6, reader.ReadArrayHeader());
    }

    [TestMethod]
    public void AGapRoundTripsThroughTheGeneratedFormatter()
    {
        var bytes = MessagePackSerializer.Serialize(Sample(), ViaGenerated);

        var round = MessagePackSerializer.Deserialize<GapModelPatch>(bytes, ViaGenerated);

        Assert.AreEqual(9, round.First);
        Assert.AreEqual("nine", round.Fourth);
        Assert.AreEqual(true, round.Sixth);
    }

    [TestMethod]
    public void ASenderWithMoreSlotsIsReadWithoutFailing()
    {
        // A newer peer adds keys this build has never seen; the reader has to skip them, not throw.
        var newer = MessagePackSerializer.Serialize(
            new object?[] { 9, null, null, "nine", null, true, "added later", 42 }, ViaGenerated);

        var round = MessagePackSerializer.Deserialize<GapModelPatch>(newer, ViaGenerated);

        Assert.AreEqual(9, round.First);
        Assert.AreEqual("nine", round.Fourth);
        Assert.AreEqual(true, round.Sixth);
    }
}
