using Mordorings.Controls;

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
    protected readonly IAutomapRenderer[] Renderers;

    protected MapViewModelBase(IMordorIoFactory ioFactory, IMapRenderFactory mapRenderFactory)
    {
        IoFactory = ioFactory;
        _mapRenderFactory = mapRenderFactory;
        Map = IoFactory.GetReader().GetMordorRecord<DATA11DungeonMap>();
        Floor[] floors = Map.Floors;
        CachedDungeonFloors = new DungeonFloor[floors.Length];
        Renderers = new IAutomapRenderer[floors.Length];
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
                    Image = iRenderer.GetMapSnapshot()?.ToBitmapSource();
                }
            };
            renderer.DrawDungeonFloorMap();
            Renderers[i] = renderer;
        }
    }

    protected IAutomapRenderer? CurrentRenderer
    {
        get
        {
            if (SelectedFloorNum is >= MinFloor and <= MaxFloor)
                return Renderers[SelectedFloorNum - 1];
            return null;
        }
    }

    [ObservableProperty]
    private DungeonFloor? _selectedFloor;

    [ObservableProperty]
    private object? _image;

    public TooltipManager Tooltips { get; } = new();

    private int _selectedFloorNum;

    public int SelectedFloorNum
    {
        get => _selectedFloorNum;
        set
        {
            int oldValue = _selectedFloorNum;
            SetProperty(ref _selectedFloorNum, value);
            HandleOnSelectedFloorNumChanged(oldValue, value);
            IncreaseFloorCommand.NotifyCanExecuteChanged();
            DecreaseFloorCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand(CanExecute = nameof(CanIncreaseFloor))]
    private void IncreaseFloor()
    {
        SelectedFloorNum = NormalizeFloorNum(SelectedFloorNum, SelectedFloorNum + 1);
    }

    [RelayCommand(CanExecute = nameof(CanDecreaseFloor))]
    private void DecreaseFloor()
    {
        SelectedFloorNum = NormalizeFloorNum(SelectedFloorNum, SelectedFloorNum - 1);
    }

    protected virtual bool CanIncreaseFloor => SelectedFloorNum < MaxFloor;

    protected virtual bool CanDecreaseFloor => SelectedFloorNum > MinFloor;

    protected virtual void HandleOnSelectedFloorNumChanged(int oldValue, int newValue)
    {
        if (newValue is < MinFloor or > MaxFloor)
        {
            int actualValue;
            if (oldValue is >= MinFloor and <= MaxFloor)
            {
                actualValue = oldValue;
            }
            else
            {
                actualValue = NormalizeFloorNum(oldValue, newValue);
            }
            _selectedFloorNum = actualValue;
        }
        SelectedFloor = CachedDungeonFloors[SelectedFloorNum - 1];
    }

    protected virtual int NormalizeFloorNum(int oldValue, int newValue) => Math.Clamp(newValue, MinFloor, MaxFloor);
}
