namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
[Flags]
public enum DungeonTileFlag
{
    None = 0,
    WallEast = 1 << 0,
    WallNorth = 1 << 1,
    DoorEast = 1 << 2,
    DoorNorth = 1 << 3,
    SecretDoorEast = 1 << 4,
    SecretDoorNorth = 1 << 5,
    FaceNorth = 1 << 6,
    FaceEast = 1 << 7,
    FaceSouth = 1 << 8,
    FaceWest = 1 << 9,
    Extinguisher = 1 << 10,
    Pit = 1 << 11,
    StairsUp = 1 << 12,
    StairsDown = 1 << 13,
    Teleporter = 1 << 14,
    Water = 1 << 15,
    Quicksand = 1 << 16,
    Rotator = 1 << 17,
    Antimagic = 1 << 18,
    Rock = 1 << 19,
    Fog = 1 << 20,
    Chute = 1 << 21,
    Stud = 1 << 22,
    Explored = 1 << 23
}
