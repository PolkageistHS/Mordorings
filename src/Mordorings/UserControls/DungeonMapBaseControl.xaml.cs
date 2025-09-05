using System.Windows;
using System.Windows.Input;

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
        Tile tile = AutomapEventConversion.GetMapCoordinatesFromEvent(e);
        if (MapClickCommand is not null && MapClickCommand.CanExecute(tile))
        {
            MapClickCommand.Execute(tile);
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
