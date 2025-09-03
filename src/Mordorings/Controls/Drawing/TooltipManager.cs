using System.Windows.Input;
using Timer = System.Timers.Timer;

namespace Mordorings.Controls;

public partial class TooltipManager : ObservableObject
{
    private readonly Timer _timer = new(100) { AutoReset = false };
    private Tile _lastMousePosition = new(0, 0);

    public event EventHandler<TooltipChangedEventArgs>? TooltipLocationChanged;

    public bool IsVisible => !string.IsNullOrWhiteSpace(TooltipText);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsVisible))]
    private string _tooltipText = "";

    [RelayCommand]
    private void ShowTooltipForTile(object? args)
    {
        if (args is not MouseEventArgs mouseArgs)
            return;
        if (_timer.Enabled)
            return;
        _timer.Start();
        Tile mousePosition = AutomapEventConversion.GetMapCoordinatesFromEvent(mouseArgs);
        if (mousePosition == _lastMousePosition)
            return;
        var eventArgs = new TooltipChangedEventArgs(mousePosition);
        _lastMousePosition = mousePosition;
        TooltipLocationChanged?.Invoke(this, eventArgs);
        if (!string.IsNullOrWhiteSpace(eventArgs.TooltipText))
        {
            TooltipText = eventArgs.TooltipText;
        }
    }
}

public sealed class TooltipChangedEventArgs(Tile tile)
{
    /// <summary>
    /// Array-based (0-29)
    /// </summary>
    public Tile Tile { get; } = tile;

    public string? TooltipText { get; set; }
}
