using MordorDataLibrary.Data;
using MordorDataLibrary.Models;

namespace Examples;

public class DungeonManipulation
{
    private readonly MordorRecordReader _reader;
    private readonly List<Action<AreaSpawn>> _actions = [];
    private bool _doNotSpawnMonster;
    private bool _primaryMonsterSet;

    public DungeonManipulation(string folder)
    {
        _reader = new MordorRecordReader(folder);
        _actions.Add(areaSpawn =>
        {
            ClearMonsterSpawn(areaSpawn.MonsterSpawnGroup2);
            ClearMonsterSpawn(areaSpawn.MonsterSpawnGroup3);
            ClearMonsterSpawn(areaSpawn.MonsterSpawnGroup4);
        });
    }

    /// <summary>
    /// Sets the provided monster as the primary spawn monster and applies the monster's default stats and settings.
    /// This should be the first method called in the builder. Calling it after other methods will overwrite the settings
    /// applied by earlier methods.
    /// </summary>
    /// <returns></returns>
    /// <remarks>Other defaults applied:
    /// <list type="bullet">
    /// <item>NumberWhoWantToJoin: 0</item>
    /// <item>Hostility: Hostile</item>
    /// <item>ChestType: None</item>
    /// <item>Gold: 0</item>
    /// <item>Chest Locked: -1 (Unknown)</item>
    /// <item>Trap: None</item>
    /// </list>
    /// </remarks>
    public DungeonManipulation SetPrimaryMonster(string monsterName, short idLevel = CreatureIdentified.Completely)
    {
        Monster? monster = _reader.GetMordorRecord<DATA05Monsters>().MonstersList.FirstOrDefault(monster => string.Equals(monster.Name, monsterName, StringComparison.OrdinalIgnoreCase));
        if (monster == null)
        {
            throw new ArgumentException($"Could not find monster \"{monsterName}\".", nameof(monsterName));
        }
        _actions.Add(areaSpawn =>
        {
            MonsterSpawn spawn = areaSpawn.MonsterSpawnGroup1;
            TreasureSpawn treasure = areaSpawn.Treasure;
            spawn.MonsterID = monster.ID;
            spawn.OtherMonsterID = monster.ID;
            spawn.IdentificationLevel = idLevel;
            spawn.Alignment = monster.Alignment;
            spawn.Atk = monster.Attack;
            spawn.Def = monster.Defense;
            spawn.CurrentHP = monster.Hits;
            spawn.MaxHP = monster.Hits;
            spawn.GroupSize = monster.GroupSize;
            spawn.NumberWhoWantToJoin = 0;
            spawn.Hostility = Hostility.Hostile;
            treasure.MonsterID = monster.ID;
            treasure.ChestType = ChestTypes.None;
            treasure.Gold = 0;
            treasure.Locked = LockedStates.Unknown;
            treasure.TrapID = 0;
        });
        _primaryMonsterSet = true;
        return this;
    }

    public DungeonManipulation SetGroupSize(short groupSize)
    {
        _actions.Add(areaSpawn =>
        {
            if (groupSize > -1)
            {
                areaSpawn.MonsterSpawnGroup1.GroupSize = groupSize;
            }
        });
        return this;
    }

    public DungeonManipulation SetStats(short atk = 0, short def = 0, short currentHits = 0, short maxHits = 0)
    {
        _actions.Add(areaSpawn =>
        {
            MonsterSpawn spawn = areaSpawn.MonsterSpawnGroup1;
            if (atk > 0)
            {
                spawn.Atk = atk;
            }
            if (def > 0)
            {
                spawn.Def = def;
            }
            if (currentHits > 0)
            {
                spawn.CurrentHP = currentHits;
            }
            if (maxHits > 0)
            {
                spawn.MaxHP = maxHits;
            }
        });
        return this;
    }

    public DungeonManipulation SetAlignment(short alignment = -1, short hostility = -1, int numberWhoWantToJoin = 0)
    {
        _actions.Add(areaSpawn =>
        {
            MonsterSpawn spawn = areaSpawn.MonsterSpawnGroup1;
            if (alignment is >= Alignments.Good and <= Alignments.Evil)
            {
                spawn.Alignment = alignment;
            }
            if (hostility is Hostility.Friendly or Hostility.Hostile)
            {
                spawn.Hostility = hostility;
            }
            spawn.NumberWhoWantToJoin = (short)Math.Clamp(numberWhoWantToJoin, 0, 4);
        });
        return this;
    }

    public DungeonManipulation SetTreasure(short chestType = -1, short gold = 0, short locked = -2, short trapID = -1)
    {
        _actions.Add(areaSpawn =>
        {
            TreasureSpawn treasure = areaSpawn.Treasure;
            if (chestType > -1)
            {
                treasure.ChestType = chestType;
            }
            if (gold > 0)
            {
                treasure.Gold = gold;
            }
            if (locked > -2)
            {
                treasure.Locked = locked;
            }
            if (trapID > -1)
            {
                treasure.TrapID = trapID;
            }
        });
        return this;
    }

    /// <summary>
    /// Forces the area to load with no monsters by setting the GroupSize and Hits to 0.
    /// This will override any other setting for the monster's group size or hits.
    /// </summary>
    /// <returns>This builder to allow method chaining.</returns>
    public DungeonManipulation DoNotSpawnMonster()
    {
        _actions.Add(areaSpawn =>
        {
            areaSpawn.MonsterSpawnGroup1.GroupSize = 0;
            areaSpawn.MonsterSpawnGroup1.CurrentHP = 0;
            _doNotSpawnMonster = true;
        });
        return this;
    }

    public void BuildAndSave(string outputFolder)
    {
        if (!_primaryMonsterSet)
            throw new InvalidOperationException("Primary monster must be set before building.");
        DATA10DungeonState dungeonState = _reader.GetMordorRecord<DATA10DungeonState>();
        foreach (AreaSpawn areaSpawn in dungeonState.AreaSpawns)
        {
            foreach (Action<AreaSpawn> action in _actions)
            {
                action(areaSpawn);
            }
            CleanUpSpawn(areaSpawn);
        }
        MordorRecordWriter writer = new(outputFolder);
        writer.WriteMordorRecord(dungeonState);
    }

    private static void ClearMonsterSpawn(MonsterSpawn spawn)
    {
        spawn.Atk = 1;
        spawn.Def = 1;
        spawn.CurrentHP = 0;
        spawn.MaxHP = 1;
        spawn.Alignment = Alignments.Evil;
        spawn.Hostility = 0;
        spawn.MonsterID = -1;
        spawn.GroupSize = 0;
        spawn.IdentificationLevel = CreatureIdentified.Completely;
        spawn.SpawnTime = (float)(DateTime.Now - DateTime.Today).TotalSeconds;
        spawn.NumberWhoWantToJoin = 0;
        spawn.OtherMonsterID = -1;
    }

    private void CleanUpSpawn(AreaSpawn areaSpawn)
    {
        MonsterSpawn spawn = areaSpawn.MonsterSpawnGroup1;
        spawn.SpawnTime = (float)(DateTime.Now - DateTime.Today).TotalSeconds;
        if (_doNotSpawnMonster)
        {
            spawn.CurrentHP = 0;
            spawn.GroupSize = 0;
        }
        if (spawn is { GroupSize: 0, CurrentHP: > 0 })
        {
            spawn.CurrentHP = 0;
        }
        if (spawn is { CurrentHP: 0, GroupSize: > 0 })
        {
            spawn.GroupSize = 0;
        }
        if (spawn is { Hostility: Hostility.Hostile, NumberWhoWantToJoin: > 0 })
        {
            spawn.NumberWhoWantToJoin = 0;
        }
        if (spawn.CurrentHP > spawn.MaxHP)
        {
            spawn.CurrentHP = spawn.MaxHP;
        }
    }
}
