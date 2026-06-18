namespace RedMist.TimingCommon.Models;

/// <summary>
/// A batch of pre-formed patches produced by an external timing source and applied by the
/// processing pipeline without source-specific parsing. The transport-neutral envelope that
/// carries already-mapped session and car changes into the pipeline.
///
/// Serialized as JSON (its patch members are source-generated, so it is not a MessagePack object).
/// </summary>
public class ExternalPatchBatch
{
    /// <summary>Gets or sets the event these patches apply to.</summary>
    public int EventId { get; set; }

    /// <summary>Gets or sets the session these patches apply to.</summary>
    public int SessionId { get; set; }

    /// <summary>Gets or sets the session-level patches in this batch.</summary>
    public List<SessionStatePatch> SessionPatches { get; set; } = [];

    /// <summary>Gets or sets the per-car patches in this batch.</summary>
    public List<CarPositionPatch> CarPatches { get; set; } = [];
}
