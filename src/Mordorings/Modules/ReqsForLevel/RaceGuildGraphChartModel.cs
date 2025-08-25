using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Mordorings.Modules.ReqsForLevel;

public partial class RaceGuildGraphChartModel : ObservableObject
{
    [ObservableProperty]
    private ISeries[] _series = [];

    public Axis[] XAxes { get; } = [new()
    {
        LabelsPaint = new SolidColorPaint(SKColors.Black),
        CustomSeparators = [ 1, 100, 200, 300, 400, 500, 600, 700, 800, 900, 999 ]
    }];

    public Axis[] YAxes { get; } = GetYAxes();

    public void Recalculate(Race? raceOne, Guild? guildOne, Race? raceTwo, Guild? guildTwo)
    {
        List<ISeries> series = [];
        if (raceOne != null && guildOne != null)
        {
            series.Add(GetXpSeries(raceOne, guildOne, SKColors.Red));
            series.Add(GetGoldSeries(raceOne, guildOne, SKColors.Coral));
        }
        if (raceTwo != null && guildTwo != null)
        {
            series.Add(GetXpSeries(raceTwo, guildTwo, SKColors.LimeGreen));
            series.Add(GetGoldSeries(raceTwo, guildTwo, SKColors.ForestGreen));
        }
        Series = series.ToArray();
    }

    private static LineSeries<int> GetXpSeries(Race race, Guild guild, SKColor color)
    {
        List<int> values = [];
        for (int i = 1; i < 999; i++)
        {
            values.Add(LevelRequirements.GetXpForNextLevel(i, race.ExpFactor, guild.ExpFactor));
        }
        return new LineSeries<int>
        {
            Values = new ReadOnlyCollection<int>(values),
            Fill = null,
            GeometrySize = 0,
            Stroke = new SolidColorPaint(color) { StrokeThickness = 3 },
            GeometryStroke = new SolidColorPaint(color) { StrokeThickness = 3 },
            Name = $"{race.Name} {guild.Name} (XP)",
            YToolTipLabelFormatter = point => $"{point.Model:N0} ({point.Index + 2})",
            ScalesYAt = 0
        };
    }

    private static LineSeries<int> GetGoldSeries(Race race, Guild guild, SKColor color)
    {
        List<int> values = [];
        for (int i = 1; i < 999; i++)
        {
            values.Add(LevelRequirements.GetGoldForNextLevel(i, guild.GoldFactor));
        }
        return new LineSeries<int>
        {
            Values = new ReadOnlyCollection<int>(values),
            Fill = null,
            GeometrySize = 2,
            Stroke = new SolidColorPaint(color) { StrokeThickness = 1 },
            GeometryStroke = new SolidColorPaint(color) { StrokeThickness = 1 },
            Name = $"{race.Name} {guild.Name} (gold)",
            YToolTipLabelFormatter = point => $"{point.Model:N0} ({point.Index + 2})",
            ScalesYAt = 1
        };
    }

    private static Axis[] GetYAxes() =>
    [
        new()
        {
            LabelsPaint = new SolidColorPaint(SKColors.DarkGreen)
        },
        new()
        {
            LabelsPaint = new SolidColorPaint(SKColors.OrangeRed),
            Position = AxisPosition.End,
            ShowSeparatorLines = false
        }
    ];
}
