using TypeGen.Core.SpecGeneration;
using RedMist.TimingCommon.Models;
using RedMist.TimingCommon.Models.InCarVideo;

namespace RedMist.TimingCommon.TypeScript;

/// <summary>
/// TypeGen generation spec for SessionState and all nested/referenced types.
/// Run \redmist-timing-common\RedMist.TimingCommon\dotnet-typegen generate
/// </summary>
public class SessionStateGenerationSpec : GenerationSpec
{
    public override void OnBeforeGeneration(OnBeforeGenerationArgs args)
    {
        // Core session types
        AddInterface<SessionState>();
        AddInterface<EventEntry>();
        AddInterface<CarPosition>();
        AddInterface<FlagDuration>();
        AddInterface<Section>();
        AddInterface<Announcement>();
        AddInterface<CompletedSection>();
        AddInterface<EventListSummary>();
        AddInterface<Event>();
        AddInterface<Session>();
        AddInterface<CompetitorMetadata>();
        AddInterface<ControlLogEntry>();
        AddInterface<CarControlLogs>();

        // Patches
        AddInterface<SessionStatePatch>();
        AddInterface<CarPositionPatch>();

        // Enums
        AddEnum<Flags>();

        // In-car video types
        AddInterface<VideoStatus>();
        AddInterface<VideoDestination>();
        AddEnum<VideoSystemType>();
        AddEnum<VideoDestinationType>();
    }
}
