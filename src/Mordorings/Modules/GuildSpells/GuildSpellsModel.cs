namespace Mordorings.Modules.GuildSpells;

public partial class GuildSpellsModel(Spell spell, Guild guild) : ObservableObject
{
    public int InitialSpellLevel { get; } = spell.Level;

    public int InitialSpellCost { get; } = spell.Cost;

    [ObservableProperty]
    private int _spellCost;

    [ObservableProperty]
    private int _casterSpellLevel;

    public void RecalculateCost(int spellLevel)
    {
         
    }
}
