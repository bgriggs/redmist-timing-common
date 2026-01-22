namespace RedMist.TimingCommon.Models;

public enum Flags
{
    /// <summary>
    /// Value: 0
    /// </summary>
    Unknown,
    /// <summary>
    /// Value: 1
    /// </summary>
    Green,
    /// <summary>
    /// Value: 2
    /// </summary>
    Yellow,
    /// <summary>
    /// Value: 3
    /// </summary>
    Red,
    /// <summary>
    /// Value: 4
    /// </summary>
    White,
    /// <summary>
    /// Value: 5
    /// </summary>
    Checkered,
    /// <summary>
    /// Value: 6
    /// </summary>
    Black,
    /// <summary>
    /// Purple 35mph.Value: 7
    /// </summary>
    Purple35,
    /// <summary>
    /// Purple 60kph. Value: 8
    /// </summary>
    Purple60
}

public static class FlagsExtensions
{
    public static Flags ToFlag(this string rmonitorFlag)
    {
        return rmonitorFlag switch
        {
            "Green" => Flags.Green,
            "Yellow" => Flags.Yellow,
            "Red" => Flags.Red,
            "White" => Flags.White,
            "Finish" => Flags.Checkered,
            "Black" => Flags.Black,
            "Purple" => Flags.Purple35,
            "Purple60" => Flags.Purple60,
            _ => Flags.Unknown,
        };
    }
}