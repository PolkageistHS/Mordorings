namespace Mordorings.Modules;

public partial class RaceGuildGraphViewModel : ObservableObject
{
    [ObservableProperty]
    private Race? _raceOne;

    [ObservableProperty]
    private Race? _raceTwo;

    [ObservableProperty]
    private Guild? _guildOne;

    [ObservableProperty]
    private Guild? _guildTwo;

    [ObservableProperty]
    private bool _showXpChart = true;

    [ObservableProperty]
    private bool _showGoldChart;

    public ObservableCollection<Race> Races { get; set; } = [];

    public ObservableCollection<Guild> Guilds { get; set; } = [];

    public RaceGuildGraphChartModel Chart { get; } = new();

    public bool IsChartEnabled => (RaceOne != null && GuildOne != null) || (RaceTwo != null && GuildTwo != null);

    [RelayCommand]
    private void ToggleView()
    {
        ShowXpChart = !ShowXpChart;
        ShowGoldChart = !ShowGoldChart;
        if (ShowXpChart == ShowGoldChart)
        {
            ShowGoldChart = !ShowXpChart;
        }
        Calculate();
    }

    partial void OnRaceOneChanged(Race? value)
    {
        Calculate();
    }

    partial void OnRaceTwoChanged(Race? value)
    {
        Calculate();
    }

    partial void OnGuildOneChanged(Guild? value)
    {
        Calculate();
    }

    partial void OnGuildTwoChanged(Guild? value)
    {
        Calculate();
    }

    private void Calculate()
    {
        Chart.Recalculate(RaceOne, GuildOne, RaceTwo, GuildTwo);
        OnPropertyChanged(nameof(IsChartEnabled));
    }
}
