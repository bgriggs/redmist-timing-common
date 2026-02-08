#pragma warning disable CS0618
using RedMist.TimingCommon.Models;
using Riok.Mapperly.Abstractions;

namespace RedMist.TimingCommon.Extensions;
public static class SessionExtensions
{
    private static readonly SessionStateMapperExt mapper = new();

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

    /// <summary>
    /// Creates a deep copy of the specified <see cref="SessionState"/> instance.
    /// </summary>
    /// <param name="original">The <see cref="SessionState"/> object to copy. Cannot be null.</param>
    /// <returns>A new <see cref="SessionState"/> instance that is a deep copy of <paramref name="original"/>.</returns>
    public static SessionState DeepCopy(this SessionState original)
    {
        return mapper.CloneSessionState(original);
    }
}

/// <summary>
/// Mapper for SessionState objects using Mapperly code generation
/// </summary>
[Mapper(UseDeepCloning = true)]
public partial class SessionStateMapperExt
{
    /// <summary>
    /// Creates a deep copy of a SessionState object
    /// </summary>
    public partial SessionState CloneSessionState(SessionState source);
}

#pragma warning restore CS0618