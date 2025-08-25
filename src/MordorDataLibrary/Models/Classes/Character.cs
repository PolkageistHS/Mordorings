namespace MordorDataLibrary.Models;

[Serializable]
public class Character
{
    [NewRecord]
    [FixedLengthString(30)]
    public string Name { get; set; } = null!;

    public short Race { get; set; }

    public short Alignment { get; set; }

    public short Sex { get; set; }

    public float DaysOld { get; set; }

    public short CurrentMP { get; set; }

    public short DungeonLevel { get; set; }

    public short CurrentX { get; set; }

    public short CurrentY { get; set; }

    public float Atk { get; set; }

    public float Def { get; set; }

    public short[] BaseStats { get; set; } = new short[7];

    public short[] ModifiedStats { get; set; } = new short[7];

    public HeldItem[] InventoryItems { get; set; } = new HeldItem[41];

    public HeldItem[] BankItems { get; set; } = new HeldItem[41];

    public short[] EquippedItems { get; set; } = new short[36];

    public long Unused0 { get; set; }

    public short Unused1 { get; set; }

    public short Direction { get; set; } // 1 = North, 2 = East, 3 = South, 4 = West

    public short Unused2 { get; set; }

    public long TotalExperience { get; set; }

    public long GoldOnHand { get; set; }

    public short CurrentHp { get; set; }

    public short MaxHp { get; set; }

    public long GoldInBank { get; set; }

    public short ExtraSwings { get; set; }

    public short ActiveGuild { get; set; }

    public GuildStatus[] GuildStatuses { get; set; } = new GuildStatus[16];

    public Companion[] Companions { get; set; } = new Companion[5];

    public short HandsOccupied { get; set; }

    public short RezCastType { get; set; }

    public short RezChance { get; set; }

    [FixedLengthString(30)]
    public string CharacterPerformingRez { get; set; } = "";

    public long[] MiscCharacterInfo { get; set; } = new long[9];

    public short[] CharacterOptions { get; set; } = new short[6];

    public short[] StatusEffects { get; set; } = new short[8];

    public short[] Resistances { get; set; } = new short[12];

    public int TempBuffs { get; set; }

    public short[] TempResistances { get; set; } = new short[12];

    public short DeadType { get; set; }

    public short CarriedCorpseID { get; set; }

    [FixedLengthString(10)]
    public string Password { get; set; } = null!;

    public SavedWindowState[] SavedWindowStates { get; set; } = new SavedWindowState[21];

    public short RecordLineNumber { get; set; }

    public int XpNeededToPin { get; set; }

    public int AbilitiesFromEquippedItems { get; set; }

    public short[] ResistsFromItems { get; set; } = new short[12];

    public short[] EquippedItemsInHands { get; set; } = new short[2];

    public short SomeADPlaceholderMaybe { get; set; }

    public short[] BufferSlots { get; set; } = new short[11];

    public short CurrentAreaNum { get; set; }

    public short Unused3 { get; set; }

    public short Unused4 { get; set; }

    public MapCoordinate SanctuaryCoordinates { get; set; } = new();

    public short[] LocationAwareness { get; set; } = new short[3];
}
