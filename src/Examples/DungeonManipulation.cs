namespace Examples;

public class DungeonManipulation
{
    private const short Friendly = 0;
    private const short Hostile = 1;

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
    public DungeonManipulation SetPrimaryMonster(string monsterName, CreatureIdentityLevel idLevel = CreatureIdentityLevel.Completely)
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
            spawn.MonsterId = monster.Id;
            spawn.OtherMonsterId = monster.Id;
            spawn.IdentificationLevel = (short)idLevel;
            spawn.Alignment = monster.Alignment;
            spawn.Atk = monster.Attack;
            spawn.Def = monster.Defense;
            spawn.CurrentHp = monster.Hits;
            spawn.MaxHp = monster.Hits;
            spawn.GroupSize = monster.GroupSize;
            spawn.NumberWhoWantToJoin = 0;
            spawn.Hostility = Hostile;
            treasure.MonsterId = monster.Id;
            treasure.ChestType = (short)ChestType.None;
            treasure.Gold = 0;
            treasure.Locked = (short)LockedState.NotLocked;
            treasure.TrapId = 0;
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
                spawn.CurrentHp = currentHits;
            }
            if (maxHits > 0)
            {
                spawn.MaxHp = maxHits;
            }
        });
        return this;
    }

    public DungeonManipulation SetAlignment(short alignment = -1, short hostility = -1, int numberWhoWantToJoin = 0)
    {
        _actions.Add(areaSpawn =>
        {
            MonsterSpawn spawn = areaSpawn.MonsterSpawnGroup1;
            if (alignment is >= (short)MonsterAlignment.Good and <= (short)MonsterAlignment.Evil)
            {
                spawn.Alignment = alignment;
            }
            if (hostility is Friendly or Hostile)
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
                treasure.TrapId = trapID;
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
            areaSpawn.MonsterSpawnGroup1.CurrentHp = 0;
            _doNotSpawnMonster = true;
        });
        return this;
    }

    public void BuildAndSave(string outputFolder)
    {
        if (!_primaryMonsterSet)
            throw new InvalidOperationException("Primary monster must be set before building.");
        var dungeonState = _reader.GetMordorRecord<DATA10DungeonState>();
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

    private void CleanUpSpawn(AreaSpawn areaSpawn)
    {
        MonsterSpawn spawn = areaSpawn.MonsterSpawnGroup1;
        spawn.SpawnTime = (float)(DateTime.Now - DateTime.Today).TotalSeconds;
        if (_doNotSpawnMonster)
        {
            spawn.CurrentHp = 0;
            spawn.GroupSize = 0;
        }
        if (spawn is { GroupSize: 0, CurrentHp: > 0 })
        {
            spawn.CurrentHp = 0;
        }
        if (spawn is { CurrentHp: 0, GroupSize: > 0 })
        {
            spawn.GroupSize = 0;
        }
        if (spawn is { Hostility: Hostile, NumberWhoWantToJoin: > 0 })
        {
            spawn.NumberWhoWantToJoin = 0;
        }
        if (spawn.CurrentHp > spawn.MaxHp)
        {
            spawn.CurrentHp = spawn.MaxHp;
        }
    }
}
