using TypeGen.Core.SpecGeneration;
using RedMist.TimingCommon.Models;
using RedMist.TimingCommon.Models.InCarVideo;
using RedMist.TimingCommon.Models.Configuration;

namespace RedMist.TimingCommon.TypeScript;

/// <summary>
/// TypeGen generation spec for SessionState and all nested/referenced types.
/// Run "dotnet tool run dotnet-typegen generate" from the project directory to regenerate TypeScript files.
/// </summary>
public class SessionStateGenerationSpec : GenerationSpec
{
    private static readonly Type[] InterfaceTypes =
    [
        typeof(SessionState),
        typeof(EventEntry),
        typeof(CarPosition),
        typeof(FlagDuration),
        typeof(Section),
        typeof(Announcement),
        typeof(CompletedSection),
        typeof(EventListSummary),
        typeof(Models.Event),
        typeof(Session),
        typeof(CompetitorMetadata),
        typeof(ControlLogEntry),
        typeof(CarControlLogs),
        typeof(SessionStatePatch),
        typeof(CarPositionPatch),
        typeof(VideoStatus),
        typeof(VideoDestination),
        typeof(BroadcasterConfig),
        typeof(EventSchedule),
        typeof(EventScheduleEntry),
    ];

    public override void OnBeforeGeneration(OnBeforeGenerationArgs args)
    {
        foreach (var type in InterfaceTypes)
            AddInterface(type);

        // Enums
        AddEnum<Flags>();
        AddEnum<VideoSystemType>();
        AddEnum<VideoDestinationType>();

        DecodeGenerator.Generate(InterfaceTypes, "TypeScript/generated");
    }
}
