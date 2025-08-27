using Mordorings.Models;

namespace Mordorings;

public static class DungeonFloorExtensions
{
    public static Teleporter? GetTeleporter(this DungeonFloor floor, int x, int y) =>
        floor.Floor.Teleporters.FirstOrDefault(teleporter => teleporter.x == x + 1 && teleporter.y == y + 1);

    public static void DeleteTeleporter(this DungeonFloor floor, int x, int y)
    {
        foreach (Teleporter teleporter in floor.Floor.Teleporters.Where(teleporter => teleporter.x == x + 1 && teleporter.y == y + 1))
        {
            teleporter.x = 0;
            teleporter.y = 0;
            teleporter.x2 = 0;
            teleporter.y2 = 0;
            teleporter.z2 = 0;
        }
    }

    public static bool SaveTeleporter(this DungeonFloor floor, int x, int y, int x2, int y2, int z2)
    {
        Teleporter? teleporter = floor.GetTeleporter(x, y);
        if (teleporter is not null)
        {
            teleporter.x2 = (short)x2;
            teleporter.y2 = (short)y2;
            teleporter.z2 = (short)z2;
            return true;
        }
        int i = GetFirstFreeTeleporter(floor);
        if (i == -1)
            return false;
        floor.Floor.Teleporters[i] = new Teleporter { x = (short)(x + 1), y = (short)(y + 1), x2 = (short)x2, y2 = (short)y2, z2 = (short)z2 };
        return true;
    }

    private static int GetFirstFreeTeleporter(DungeonFloor floor)
    {
        for (int i = 0; i < 20; i++)
        {
            if (floor.Floor.Teleporters[i].x == 0)
                return i;
        }
        return -1;
    }

    public static Chute? GetChute(this DungeonFloor floor, int x, int y) =>
        floor.Floor.Chutes.FirstOrDefault(chute => chute.x == x + 1 && chute.y == y + 1);

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
        floor.Floor.Chutes[i] = new Chute { x = (short)(x + 1), y = (short)(y + 1), Depth = (short)depth };
        return true;
    }

    public static void DeleteChute(this DungeonFloor floor, int x, int y)
    {
        foreach (Chute chute in floor.Floor.Chutes.Where(chute => chute.x == x + 1 && chute.y == y + 1))
        {
            chute.x = 0;
            chute.y = 0;
            chute.Depth = 0;
        }
    }

    private static int GetFirstFreeChute(this DungeonFloor floor)
    {
        for (int i = 0; i < 10; i++)
        {
            if (floor.Floor.Chutes[i].x == 0)
                return i;
        }
        return -1;
    }
}
