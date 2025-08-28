using System.Windows;
using System.Windows.Input;
using Mordorings.Controls;

namespace Mordorings.UserControls;

public partial class DungeonMapBaseControl
{
    public DungeonMapBaseControl()
    {
        InitializeComponent();
        ImageMap.MouseLeftButtonUp += OnMouseLeftButtonUp;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        (int X, int Y) coords = AutomapEventConverters.GetMapCoordinatesFromEvent(e);
        if (MapClickCommand is not null && coords is { X: >= 0, Y: >= 0 } && MapClickCommand.CanExecute(coords))
        {
            MapClickCommand.Execute(coords);
        }
    }

    public static readonly DependencyProperty MapClickCommandProperty = DependencyProperty.Register(
        nameof(MapClickCommand), typeof(ICommand), typeof(DungeonMapBaseControl), new PropertyMetadata(default(ICommand?)));

    public ICommand? MapClickCommand
    {
        get => (ICommand?)GetValue(MapClickCommandProperty);
        set => SetValue(MapClickCommandProperty, value);
    }
}
