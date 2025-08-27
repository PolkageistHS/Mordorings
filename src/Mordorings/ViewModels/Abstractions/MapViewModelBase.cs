using Mordorings.Controls;
using Mordorings.Models;

namespace Mordorings.ViewModels.Abstractions;

public abstract partial class MapViewModelBase : ViewModelBase
{
    private const string SpriteSheetFile = "Assets/DungeonSprites.bmp";

    protected readonly DungeonFloor[] CachedDungeonFloors;
    protected readonly IMordorIoFactory IoFactory;
    protected readonly DATA11DungeonMap Map;
    private readonly IMapRenderFactory _mapRenderFactory;

    protected MapViewModelBase(IMordorIoFactory ioFactory, IMapRenderFactory mapRenderFactory)
    {
        IoFactory = ioFactory;
        _mapRenderFactory = mapRenderFactory;
        Map = IoFactory.GetReader().GetMordorRecord<DATA11DungeonMap>();
        CachedDungeonFloors = Map.Floors.Select(CreateDungeonFloor).ToArray();
    }

    protected DungeonFloor CreateDungeonFloor(Floor floor)
    {
        IAutomapRenderer renderer = _mapRenderFactory.CreateAutomapRenderer();
        renderer.LoadSpriteSheet(SpriteSheetFile);
        var dungeonFloor = new DungeonFloor(floor);
        dungeonFloor.Initialize(renderer);
        return dungeonFloor;
    }

    [ObservableProperty]
    private DungeonFloor? _selectedFloor;

    [ObservableProperty]
    private int _selectedFloorNum;

    [RelayCommand]
    private void IncreaseFloor()
    {
        SelectedFloorNum = Math.Clamp(SelectedFloorNum + 1, 1, 15);
    }

    [RelayCommand]
    private void DecreaseFloor()
    {
        SelectedFloorNum = Math.Clamp(SelectedFloorNum - 1, 1, 15);
    }

    partial void OnSelectedFloorNumChanged(int oldValue, int newValue)
    {
        if (newValue is < 1 or > 15)
        {
            if (oldValue is >= 1 and <= 15)
            {
                _selectedFloorNum = oldValue;
            }
            else
            {
                _selectedFloorNum = Math.Clamp(newValue, 1, 15);
            }
        }
        SelectedFloor = CachedDungeonFloors[SelectedFloorNum - 1];
        if (SelectedFloorNum == oldValue)
            return;
        OnSelectedFloorNumChanged();
    }

    protected abstract void OnSelectedFloorNumChanged();
}
