namespace MordorDataLibrary.Models;

public enum MonsterAlignment
{
    Good = 0,
    Neutral = 1,
    Evil = 2
}

public enum LockedState
{
    NotLocked = 0,
    Locked = 1,
    Unlocked = 2,
    Three = 3,
    NegativeOne = -1
}

public enum ChestType
{
    Box = 2,
    Chest = 3
}

public enum TrapType
{
    None = -1,
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
