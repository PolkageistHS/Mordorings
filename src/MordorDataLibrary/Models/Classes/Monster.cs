namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Monster
{
    [NewRecord]
    public string Name { get; set; } = null!;

    public short Attack { get; set; }

    public short Defense { get; set; }

    public short Id { get; set; }

    public short Hits { get; set; }

    public short NumGroups { get; set; }

    public short PicId { get; set; }

    public short LockedChance { get; set; }

    public short LevelFound { get; set; }

    public short[] Resistances { get; set; } = new short[12];

    public int SpecialPropertyFlags { get; set; }

    public int SpecialAttackFlags { get; set; }

    public int SpellFlags { get; set; }

    public short EncounterChance { get; set; }

    public short[] BoxChance { get; set; } = new short[4];

    public short Alignment { get; set; }

    public short GroupSize { get; set; }

    public int GoldFactor { get; set; }

    public int TrapFlags { get; set; }

    public short GuildLevel { get; set; }

    public short[] Stats { get; set; } = new short[7];

    public short MonsterSubtype { get; set; }

    public float DamageMod { get; set; }

    public short CompanionType { get; set; }

    public short CompanionSpawnMode { get; set; }

    public short CompanionId { get; set; } //id of specific monster

    public short[] Items { get; set; } = new short[11];

    public short SimilarityId { get; set; }

    public short CompanionGrouping { get; set; }

    public short Size { get; set; }

    public short ItemDropLevel { get; set; }

    public short ItemChance { get; set; }

    public override string ToString() => Name;

    public class MonsterComparable : IEqualityComparer<Monster>
    {
        public bool Equals(Monster? x, Monster? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (x is null)
            {
                return false;
            }
            if (y is null)
            {
                return false;
            }
            if (x.GetType() != y.GetType())
            {
                return false;
            }
            return x.Name == y.Name && x.Id == y.Id;
        }

        public int GetHashCode(Monster obj) => HashCode.Combine(obj.Name, obj.Id);
    }
}
