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
    Purple60,
    /// <summary>
    /// Faster car approaching. Value: 9
    /// </summary>
    Blue,
    /// <summary>
    /// Black with orange ball, mechanical problem. Value: 10
    /// </summary>
    MeatBall,
    /// <summary>
    /// Debris on track. Value: 11
    /// </summary>
    Debris,
    /// <summary>
    /// Waving yellow, local caution. Yellow is used for standing yellow. Value: 12
    /// </summary>
    WavingYellow
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

    /// <summary>
    /// Maps a Flagtronics flag name (carFlag, localFlag, or fullCourseFlag) to a flag.
    /// Combination flags (e.g. StYellowWhite) map to their dominant flag. Unrecognized
    /// names map to Unknown per the Flagtronics spec, which allows the list to grow.
    /// </summary>
    public static Flags FlagtronicsToFlag(this string flagtronicsFlag)
    {
        return flagtronicsFlag switch
        {
            "Green" or "LocalGreen" => Flags.Green,
            "FCYellow" or "Yellow" or "StYellow" or "StYellowDB" or "StYellowWhite" or "WhiteStYellowDB" => Flags.Yellow,
            "WavingYel" or "WavingYelDB" or "WavingYelWhite" or "WhiteWavingYelDB" => Flags.WavingYellow,
            "Red" => Flags.Red,
            "Black" => Flags.Black,
            "MeatBall" => Flags.MeatBall,
            "Blue" => Flags.Blue,
            "Debris" => Flags.Debris,
            "White" or "WhiteDB" => Flags.White,
            "Checkered" or "LocalCheckered" => Flags.Checkered,
            "Purple" => Flags.Purple35,
            _ => Flags.Unknown,
        };
    }
}