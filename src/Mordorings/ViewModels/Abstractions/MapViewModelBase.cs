using Mordorings.Controls;
using Mordorings.Models;

namespace Mordorings.ViewModels.Abstractions;

public abstract partial class MapViewModelBase : ViewModelBase
{
    private const string SpriteSheetFile = "Assets/DungeonSprites.bmp";

    protected const int MinFloor = 1;
    protected const int MaxFloor = 15;
    protected const int FloorWidth = 30;
    protected const int FloorHeight = 30;

    protected readonly DungeonFloor[] CachedDungeonFloors;
    protected readonly IMordorIoFactory IoFactory;
    protected readonly DATA11DungeonMap Map;
    private readonly IMapRenderFactory _mapRenderFactory;
    private readonly IAutomapRenderer[] _renderers;

    protected MapViewModelBase(IMordorIoFactory ioFactory, IMapRenderFactory mapRenderFactory)
    {
        IoFactory = ioFactory;
        _mapRenderFactory = mapRenderFactory;
        Map = IoFactory.GetReader().GetMordorRecord<DATA11DungeonMap>();
        Floor[] floors = Map.Floors;
        CachedDungeonFloors = new DungeonFloor[floors.Length];
        _renderers = new IAutomapRenderer[floors.Length];
        InitializeRenderers(floors);
    }

    private void InitializeRenderers(Floor[] floors)
    {
        for (int i = 0; i < floors.Length; i++)
        {
            var dungeonFloor = new DungeonFloor(floors[i]);
            CachedDungeonFloors[i] = dungeonFloor;
            IAutomapRenderer renderer = _mapRenderFactory.CreateAutomapRenderer();
            renderer.LoadSpriteSheet(SpriteSheetFile);
            renderer.Initialize(dungeonFloor);
            renderer.MapUpdated += (sender, _) =>
            {
                if (sender is IAutomapRenderer iRenderer && SelectedFloor != null)
                {
                    SelectedFloor.Image = iRenderer.GetMapSnapshot()?.ToBitmapSource();
                }
            };
            renderer.DrawDungeonFloorMap();
            _renderers[i] = renderer;
        }
    }

    protected IAutomapRenderer? CurrentRenderer
    {
        get
        {
            if (SelectedFloorNum is >= 1 and <= MaxFloor)
                return _renderers[SelectedFloorNum - 1];
            return null;
        }
    }

    [ObservableProperty]
    private DungeonFloor? _selectedFloor;

    [ObservableProperty]
    private int _selectedFloorNum;

    [RelayCommand]
    private void IncreaseFloor()
    {
        SelectedFloorNum = NormalizeFloorNum(SelectedFloorNum + 1);
    }

    [RelayCommand]
    private void DecreaseFloor()
    {
        SelectedFloorNum = NormalizeFloorNum(SelectedFloorNum - 1);
    }

    partial void OnSelectedFloorNumChanged(int oldValue, int newValue)
    {
        if (newValue is < 1 or > MaxFloor)
        {
            if (oldValue is >= 1 and <= MaxFloor)
            {
                _selectedFloorNum = oldValue;
            }
            else
            {
                _selectedFloorNum = NormalizeFloorNum(newValue);
            }
        }
        SelectedFloor = CachedDungeonFloors[SelectedFloorNum - 1];
        if (SelectedFloorNum != oldValue)
        {
            OnSelectedFloorNumChanged();
        }
    }

    partial void OnSelectedFloorChanged(DungeonFloor? value)
    {
        CurrentRenderer?.DrawDungeonFloorMap();
    }

    protected abstract void OnSelectedFloorNumChanged();

    private static int NormalizeFloorNum(int floorNum) => Math.Clamp(floorNum, MinFloor, MaxFloor);
}
