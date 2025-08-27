namespace Mordorings.Modules.GuildSpells;

public partial class GuildSpellsViewModel : ViewModelBase
{
    public GuildSpellsViewModel(IMordoringSettings settings)
    {
        var reader = new MordorRecordReader(settings.DataFileFolder);
        var gameData = reader.GetMordorRecord<DATA01GameData>();
        Spell[] spells = reader.GetMordorRecord<DATA02Spells>().Spells;
        Guilds = new ObservableCollection<Guild>(gameData.Guilds);
        SelectedGuild = Guilds.First();
        _ = SpellCost.GetSpellCost(spells[SpellIndexes.WordofDeath], Guilds[GuildIndexes.Healer], 47);
    }

    [ObservableProperty]
    private Guild _selectedGuild;

    [ObservableProperty]
    private int _guildLevel;

    [ObservableProperty]
    private int _spellLevel;

    public ObservableCollection<Guild> Guilds { get; }

    public ObservableCollection<GuildSpellsModel> SpellsForGuild { get; } = [];

    partial void OnGuildLevelChanged(int value)
    {
        SpellLevel = Math.Clamp(value / 2, 1, 255);
    }

    partial void OnSelectedGuildChanged(Guild value)
    {
        Dictionary<SpellCategory, float> _ = value.GetGuildSpellCategories();
    }

    public override string Instructions => "Calculates spell cost for guild and spell level";
}
