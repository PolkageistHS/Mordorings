namespace Mordorings.Modules.GuildSpells;

public partial class GuildSpellsViewModel : ViewModelBase
{
    private readonly Spell[] _spells;
    private readonly MordorRecordReader _reader;
    private readonly DATA01GameData _gameData;

    public GuildSpellsViewModel(IMordoringSettings settings)
    {
        _reader = new MordorRecordReader(settings.DataFileFolder);
        _gameData = _reader.GetMordorRecord<DATA01GameData>();
        _spells = _reader.GetMordorRecord<DATA02Spells>().Spells;
        Guilds = new ObservableCollection<Guild>(_gameData.Guilds);
        SelectedGuild = Guilds.First();
        int x = SpellCost.GetSpellCost(_spells[SpellIndexes.WordofDeath], Guilds[GuildIndexes.Healer], 47);
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
        Dictionary<SpellCategory, float> x = value.GetGuildSpellCategories();
    }

    public override string Instructions => "Calculates spell cost for guild and spell level";
}
