using Mordorings.Models;

namespace Mordorings;

public static class DungeonFloorExtensions
{
    public static Teleporter? GetTeleporter(this DungeonFloor floor, int x, int y) =>
        floor.Floor.Teleporters.FirstOrDefault(teleporter => teleporter.X == x + 1 && teleporter.Y == y + 1);

    public static void DeleteTeleporter(this DungeonFloor floor, int x, int y)
    {
        foreach (Teleporter teleporter in floor.Floor.Teleporters.Where(teleporter => teleporter.X == x + 1 && teleporter.Y == y + 1))
        {
            teleporter.X = 0;
            teleporter.Y = 0;
            teleporter.X2 = 0;
            teleporter.Y2 = 0;
            teleporter.Z2 = 0;
        }
    }

    public static bool SaveTeleporter(this DungeonFloor floor, int x, int y, int x2, int y2, int z2)
    {
        Teleporter? teleporter = floor.GetTeleporter(x, y);
        if (teleporter is not null)
        {
            teleporter.X2 = (short)x2;
            teleporter.Y2 = (short)y2;
            teleporter.Z2 = (short)z2;
            return true;
        }
        int i = GetFirstFreeTeleporter(floor);
        if (i == -1)
            return false;
        floor.Floor.Teleporters[i] = new Teleporter { X = (short)(x + 1), Y = (short)(y + 1), X2 = (short)x2, Y2 = (short)y2, Z2 = (short)z2 };
        return true;
    }

    private static int GetFirstFreeTeleporter(DungeonFloor floor)
    {
        for (int i = 0; i < 20; i++)
        {
            if (floor.Floor.Teleporters[i].X == 0)
                return i;
        }
        return -1;
    }

    public static Chute? GetChute(this DungeonFloor floor, int x, int y) =>
        floor.Floor.Chutes.FirstOrDefault(chute => chute.X == x + 1 && chute.Y == y + 1);

    public static bool SaveChute(this DungeonFloor floor, int x, int y, int depth)
    {
        Chute? chute = GetChute(floor, x, y);
        if (chute is not null)
        {
            chute.Depth = (short)depth;
            return true;
        }
        int i = GetFirstFreeChute(floor);
        if (i == -1)
            return false;
        floor.Floor.Chutes[i] = new Chute { X = (short)(x + 1), Y = (short)(y + 1), Depth = (short)depth };
        return true;
    }

    public static void DeleteChute(this DungeonFloor floor, int x, int y)
    {
        foreach (Chute chute in floor.Floor.Chutes.Where(chute => chute.X == x + 1 && chute.Y == y + 1))
        {
            chute.X = 0;
            chute.Y = 0;
            chute.Depth = 0;
        }
    }

    private static int GetFirstFreeChute(this DungeonFloor floor)
    {
        for (int i = 0; i < 10; i++)
        {
            if (floor.Floor.Chutes[i].X == 0)
                return i;
        }
        return -1;
    }
}
