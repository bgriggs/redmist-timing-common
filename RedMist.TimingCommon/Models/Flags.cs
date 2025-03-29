namespace RedMist.TimingCommon.Models;

public enum Flags
{
    Unknown,
    Green,
    Yellow,
    Red,
    White,
    Checkered,
    Black,
    Purple35
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
            _ => Flags.Unknown,
        };
    }
}