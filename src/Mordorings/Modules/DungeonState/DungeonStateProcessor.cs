namespace Mordorings.Modules;

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
        treasure.TrapId = 0;
    }

    private void ProcessMonster(AreaSpawn area)
    {
        MonsterSpawn spawn = area.MonsterSpawnGroup1;
        spawn.MonsterId = monsterId;
        spawn.OtherMonsterId = monsterId;
        spawn.Alignment = (short)model.Alignment;
        spawn.CurrentHp = model.CurrentHits!.Value;
        spawn.MaxHp = model.MaxHits!.Value;
        spawn.Atk = model.Attack!.Value;
        spawn.Def = model.Defense!.Value;
        spawn.GroupSize = model.GroupSize!.Value;
        spawn.Hostility = (short)(model.Friendly ? 0 : 1);
        spawn.NumberWhoWantToJoin = model.NumToJoin ?? 0;
        spawn.SpawnTime = (float)(DateTime.Now - DateTime.Today).TotalSeconds;
    }

    private void ProcessTreasure(AreaSpawn area)
    {
        area.Treasure.MonsterId = monsterId;
        area.Treasure.ChestType = (short)model.ChestType;
        area.Treasure.Locked = (short)model.LockedType;
        area.Treasure.TrapId = (short)model.TrapType;
        area.Treasure.Gold = model.Gold;
    }

    private static void ClearMonsterSpawn(MonsterSpawn spawn)
    {
        spawn.Atk = 1;
        spawn.Def = 1;
        spawn.CurrentHp = 0;
        spawn.MaxHp = 1;
        spawn.Alignment = (short)MonsterAlignment.Evil;
        spawn.Hostility = 0;
        spawn.MonsterId = -1;
        spawn.GroupSize = 0;
        spawn.IdentificationLevel = (short)CreatureIdentityLevel.Completely;
        spawn.SpawnTime = (float)(DateTime.Now - DateTime.Today).TotalSeconds;
        spawn.NumberWhoWantToJoin = 0;
        spawn.OtherMonsterId = -1;
    }
}
