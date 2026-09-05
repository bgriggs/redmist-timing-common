using MessagePack;
using MessagePack.Formatters;
using MessagePack.ImmutableCollection;
using MessagePack.Resolvers;
using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.Tests.Models;

/// <summary>
/// Covers serialization on a runtime that cannot generate code, which is where iOS runs.
/// </summary>
/// <remarks>
/// iOS is full AOT: Apple forbids a JIT, so <c>RuntimeFeature.IsDynamicCodeSupported</c> is false and
/// MessagePack drops two resolvers from its standard chain - <see cref="DynamicObjectResolver"/>,
/// which emits a formatter for an arbitrary <c>[MessagePackObject]</c> at runtime, and
/// DynamicUnionResolver. DynamicGenericResolver stays, so collections still work. What stops working
/// is any object type with no formatter of its own.
///
/// Every hand-written model here gets one from MessagePack's source generator. The patch types do
/// not: they are emitted by PatchClassGenerator, and Roslyn source generators cannot see each other's
/// output, so the MessagePack generator never sees them. On a JIT they are covered by
/// DynamicObjectResolver and nobody notices.
///
/// The app does use the JSON hub protocol on iOS rather than MessagePack, and these types being
/// unserializable there is sufficient to require that. It is not established to be the original
/// reason: the fallback in BigMission.Shared attributes itself to reflection over InvokeAsync's
/// parameters, which is a different code path, and the hub carries other types besides these two -
/// CarControlLogs and InCarPayload among them, both of which do have generated formatters. Fixing
/// this is necessary for MessagePack on iOS; whether it is sufficient wants a device to answer.
///
/// <see cref="AotChain"/> is that runtime's resolver list reproduced exactly, so this failure is
/// reproducible on a desktop and does not need a device to test.
/// </remarks>
[TestClass]
public class AotFormatterTests
{
    /// <summary>
    /// The resolvers MessagePack composes when it cannot generate code, copied from
    /// StandardResolverHelper.DefaultResolvers - the AvoidDynamicCode branch. That branch is internal
    /// and selected by a static readonly bool, so it cannot be switched on from a test; naming the
    /// same resolvers here is the way to run against it.
    /// </summary>
    private static readonly IFormatterResolver AotChain = CompositeResolver.Create(
        BuiltinResolver.Instance,
        AttributeFormatterResolver.Instance,
        SourceGeneratedFormatterResolver.Instance,
        ImmutableCollectionResolver.Instance,
        CompositeResolver.Create(ExpandoObjectFormatter.Instance),
        DynamicGenericResolver.Instance);

    private static readonly MessagePackSerializerOptions AotOptions =
        MessagePackSerializerOptions.Standard.WithResolver(AotChain);

    [TestMethod]
    public void TheModelsSerializeWithoutGeneratingCode()
    {
        // Sanity check on the harness: these have source-generated formatters, so they are expected
        // to work here. If these fail, the chain above is wrong rather than the types.
        _ = MessagePackSerializer.Serialize(new SessionState { EventId = 1 }, AotOptions);
        _ = MessagePackSerializer.Serialize(new CarPosition { EventId = "1" }, AotOptions);
    }

    [TestMethod]
    public void TheSessionPatchSerializesWithoutGeneratingCode()
    {
        // What the hub sends on "ReceiveSessionPatch".
        var patch = new SessionStatePatch { EventId = 7, EventName = "Test" };

        var bytes = MessagePackSerializer.Serialize(patch, AotOptions);
        var round = MessagePackSerializer.Deserialize<SessionStatePatch>(bytes, AotOptions);

        Assert.AreEqual(7, round.EventId);
        Assert.AreEqual("Test", round.EventName);
    }

    [TestMethod]
    public void TheCarPatchSerializesWithoutGeneratingCode()
    {
        // What the hub sends on "ReceiveCarPatches", as an array.
        var patches = new[] { new CarPositionPatch { EventId = "7", Number = "42" } };

        var bytes = MessagePackSerializer.Serialize(patches, AotOptions);
        var round = MessagePackSerializer.Deserialize<CarPositionPatch[]>(bytes, AotOptions);

        Assert.AreEqual(1, round.Length);
        Assert.AreEqual("7", round[0].EventId);
        Assert.AreEqual("42", round[0].Number);
    }
}
