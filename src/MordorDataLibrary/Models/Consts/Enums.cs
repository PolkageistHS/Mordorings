namespace MordorDataLibrary.Models;

public enum MonsterAlignment
{
    Good = 0,
    Neutral = 1,
    Evil = 2
}

public enum LockedState
{
    NotLocked = -1,
    Unlocked = 0,
    Locked = 1,
}

public enum ChestType
{
    Box = 2,
    Chest = 3
}

public enum TrapType
{
    None = 0,
    Poison,
    Disease,
    Exploding,
    Fire,
    Slime,
    Fate,
    Teleport,
    Blackout,
    Fear,
    Withering
}
