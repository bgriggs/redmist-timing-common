using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.Extensions;

public static class SessionExtensions
{
    /// <summary>
    /// Converts a SessionState to a Payload with deep copying of CarPositions.
    /// Non-matching fields are ignored, and EventEntryUpdates and CarPositionUpdates are left empty.
    /// </summary>
    /// <param name="sessionState">The SessionState to convert</param>
    /// <returns>A new Payload instance with matching fields copied</returns>
    public static Payload ToPayload(this SessionState sessionState)
    {
        ArgumentNullException.ThrowIfNull(sessionState);

        return new Payload
        {
            EventId = sessionState.EventId,
            EventName = sessionState.EventName,
            EventEntries = sessionState.EventEntries?.ToList() ?? [],
            CarPositions = sessionState.CarPositions?.Select(static c => c.DeepCopy()).ToList() ?? [],
            FlagDurations = sessionState.FlagDurations?.ToList(),
            EventEntryUpdates = [],
            CarPositionUpdates = [],
            EventStatus = new EventStatus 
            { 
                EventId = sessionState.EventId.ToString(),
                LapsToGo = sessionState.LapsToGo,
                TimeToGo = sessionState.TimeToGo,
                LocalTimeOfDay = sessionState.LocalTimeOfDay,
                RunningRaceTime = sessionState.RunningRaceTime,
                IsPracticeQualifying = sessionState.IsPracticeQualifying,
                Flag = sessionState.CurrentFlag
            }
        };
    }
}
