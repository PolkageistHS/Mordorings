namespace Mordorings.Modules;

public partial class LevelRequirementsViewModel : ViewModelBase
{
    public LevelRequirementsViewModel(IMordoringSettings settings, RaceGuildGraphViewModel graphViewModel)
    {
        GraphViewModel = graphViewModel;
        var gameData = new MordorRecordReader(settings.DataFileFolder).GetMordorRecord<DATA01GameData>();
        Guilds = new ObservableCollection<Guild>(gameData.Guilds);
        Races = new ObservableCollection<Race>(gameData.Races);
        GraphViewModel.Guilds = new ObservableCollection<Guild>(gameData.Guilds);
        GraphViewModel.Races = new ObservableCollection<Race>(gameData.Races);
        SelectedGuild = Guilds.First();
        SelectedRace = Races.First();
        TargetLevel = 1;
    }

    public RaceGuildGraphViewModel GraphViewModel { get; }

    public ObservableCollection<Guild> Guilds { get; }

    public ObservableCollection<Race> Races { get; }

    [ObservableProperty]
    private int _targetLevel;

    [ObservableProperty]
    private Guild _selectedGuild;

    [ObservableProperty]
    private Race _selectedRace;

    [ObservableProperty]
    private int _experience;

    [ObservableProperty]
    private int _experienceForPin;

    [ObservableProperty]
    private int _gold;

    [ObservableProperty]
    private long _totalGold;

    [RelayCommand]
    private void Calculate()
    {
        if (TargetLevel is > 1 and < 1000)
        {
            int actualLevel = TargetLevel - 1;
            Gold = LevelRequirements.GetGoldForNextLevel(actualLevel, SelectedGuild.GoldFactor);
            TotalGold = LevelRequirements.GetTotalGold(actualLevel, SelectedGuild.GoldFactor);
            Experience = LevelRequirements.GetXpForNextLevel(actualLevel, SelectedRace.ExpFactor, SelectedGuild.ExpFactor);
            ExperienceForPin = LevelRequirements.GetXpForPin(actualLevel, SelectedRace.ExpFactor, SelectedGuild.ExpFactor);
        }
        else
        {
            Gold = 0;
            TotalGold = 0;
            Experience = 0;
            ExperienceForPin = 0;
        }
    }

    partial void OnTargetLevelChanged(int value)
    {
        Calculate();
    }

    partial void OnSelectedGuildChanged(Guild value)
    {
        Calculate();
    }

    partial void OnSelectedRaceChanged(Race value)
    {
        Calculate();
    }

    public override string Instructions => "Calculates the experience and gold required to reach the specified level.";
}
