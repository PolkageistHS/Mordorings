namespace Mordorings.Modules.DungeonState;

public class DungeonStateProcessor(short monsterId, DungeonStateModel model, DATA10DungeonState data)
{
    public DATA10DungeonState Process()
    {
        foreach (AreaSpawn area in data.AreaSpawns)
        {
            ClearAllMonsters(area);
            ClearTreasureSpawn(area.Treasure);
            ProcessMonster(area);
            ProcessTreasure(area);
        }
        return data;
    }

    private static void ClearAllMonsters(AreaSpawn area)
    {
        ClearMonsterSpawn(area.MonsterSpawnGroup1);
        ClearMonsterSpawn(area.MonsterSpawnGroup2);
        ClearMonsterSpawn(area.MonsterSpawnGroup3);
        ClearMonsterSpawn(area.MonsterSpawnGroup4);
    }

    private static void ClearTreasureSpawn(TreasureSpawn treasure)
    {
        treasure.ChestType = (short)ChestType.None;
        treasure.Gold = 0;
        treasure.Locked = (short)LockedState.NotLocked;
        treasure.TrapID = 0;
    }

    private void ProcessMonster(AreaSpawn area)
    {
        MonsterSpawn spawn = area.MonsterSpawnGroup1;
        spawn.MonsterID = monsterId;
        spawn.OtherMonsterID = monsterId;
        spawn.Alignment = (short)model.Alignment;
        spawn.CurrentHP = model.CurrentHits!.Value;
        spawn.MaxHP = model.MaxHits!.Value;
        spawn.Atk = model.Attack!.Value;
        spawn.Def = model.Defense!.Value;
        spawn.GroupSize = model.GroupSize!.Value;
        spawn.Hostility = (short)(model.Friendly ? 0 : 1);
        spawn.NumberWhoWantToJoin = model.NumToJoin ?? 0;
        spawn.SpawnTime = (float)(DateTime.Now - DateTime.Today).TotalSeconds;
    }

    private void ProcessTreasure(AreaSpawn area)
    {
        area.Treasure.MonsterID = monsterId;
        area.Treasure.ChestType = (short)model.ChestType;
        area.Treasure.Locked = (short)model.LockedType;
        area.Treasure.TrapID = (short)model.TrapType;
        area.Treasure.Gold = model.Gold;
    }

    private static void ClearMonsterSpawn(MonsterSpawn spawn)
    {
        spawn.Atk = 1;
        spawn.Def = 1;
        spawn.CurrentHP = 0;
        spawn.MaxHP = 1;
        spawn.Alignment = (short)MonsterAlignment.Evil;
        spawn.Hostility = 0;
        spawn.MonsterID = -1;
        spawn.GroupSize = 0;
        spawn.IdentificationLevel = (short)CreatureIdentityLevel.Completely;
        spawn.SpawnTime = (float)(DateTime.Now - DateTime.Today).TotalSeconds;
        spawn.NumberWhoWantToJoin = 0;
        spawn.OtherMonsterID = -1;
    }
}
