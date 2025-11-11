using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.Extensions;

/// <summary>
/// Provides extension methods for converting a Payload to a SessionState with deep copying of relevant fields.
/// </summary>
/// <remarks>Fields in SessionState that are not present in the Payload are initialized with their default values.
/// EventEntryUpdates and CarPositionUpdates fields are intentionally ignored during conversion. This class is intended
/// for scenarios where a complete SessionState representation is needed based on the available data in a
/// Payload.</remarks>
public static class PayloadExtensions
{
    /// <summary>
    /// Converts a Payload to a SessionState with deep copying of CarPositions.
    /// EventEntryUpdates and CarPositionUpdates fields are ignored.
    /// Fields not available in Payload are left as default values.
    /// </summary>
    /// <param name="payload">The Payload to convert</param>
    /// <returns>A new SessionState instance with available fields copied</returns>
    public static SessionState ToSessionState(this Payload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        var sessionState = new SessionState
        {
            EventId = payload.EventId,
            EventName = payload.EventName ?? string.Empty,
            EventEntries = payload.EventEntries?.ToList() ?? [],
            CarPositions = payload.CarPositions?.Select(static cp => cp.DeepCopy()).ToList() ?? [],
            FlagDurations = payload.FlagDurations?.ToList() ?? [],
            // Fields available from EventStatus if present
            LapsToGo = payload.EventStatus?.LapsToGo ?? 0,
            TimeToGo = payload.EventStatus?.TimeToGo ?? string.Empty,
            LocalTimeOfDay = payload.EventStatus?.LocalTimeOfDay ?? string.Empty,
            RunningRaceTime = payload.EventStatus?.RunningRaceTime ?? string.Empty,
            IsPracticeQualifying = payload.EventStatus?.IsPracticeQualifying ?? false,
            CurrentFlag = payload.EventStatus?.Flag ?? Flags.Unknown,
            // Fields not available in Payload - left as defaults
            SessionId = 0,
            SessionName = string.Empty,
            SessionStartTime = DateTime.MinValue,
            SessionEndTime = null,
            LocalTimeZoneOffset = 0.0,
            IsLive = false,
            GreenTimeMs = null,
            GreenLaps = null,
            YellowTimeMs = null,
            YellowLaps = null,
            NumberOfYellows = null,
            RedTimeMs = null,
            AverageRaceSpeed = null,
            LeadChanges = null,
            Sections = [],
            ClassColors = [],
            Announcements = [],
            LastUpdated = null
        };

        return sessionState;
    }
}
