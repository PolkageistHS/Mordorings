namespace MordorDataLibrary.Models;

public static class ChestTypes
{
    public const short None = 0;
    public const short Box = 2;
    public const short Chest = 3;
}

public static class Alignments
{
    public const short Good = 0;
    public const short Neutral = 1;
    public const short Evil = 2;
}

public static class Hostility
{
    public const short Friendly = 0;
    public const short Hostile = 1;
}

public static class CreatureIdentified
{
    public const short Completely = 0;
    public const short Mostly = 1;
    public const short Partially = 2;
    public const short Barely = 3;
}

public static class LockedState
{
    public const short Unknown = -1;
    public const short NotLocked = 0;
    public const short Unlocked = 1;
    public const short Locked = 2;
}
