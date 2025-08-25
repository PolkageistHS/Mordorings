using Mordorings.Controls;

namespace Mordorings.Modules.EditMap;

public partial class EditMapViewModel : ViewModelBase
{
    private readonly IMapRenderFactory _mapRenderFactory;
    private const string SpriteSheetFile = "Assets/DungeonSprites.bmp";

    private readonly DATA11DungeonMap _map;
    private readonly DungeonFloor[] _cachedFloors;

    public EditMapViewModel(IMordorIoFactory ioFactory, IMapRenderFactory mapRenderFactory)
    {
        _mapRenderFactory = mapRenderFactory;
        MordorRecordReader reader = ioFactory.GetReader();
        _map = reader.GetMordorRecord<DATA11DungeonMap>();
        _cachedFloors = CacheDungeonFloors().ToArray();
        SelectedFloorNum = 1;
    }

    private IEnumerable<DungeonFloor> CacheDungeonFloors()
    {
        foreach (Floor floor in _map.Floors)
        {
            IAutomapRenderer renderer = _mapRenderFactory.CreateRenderer();
            renderer.LoadSpriteSheet(SpriteSheetFile);
            var dungeonFloor = new DungeonFloor(floor);
            dungeonFloor.Initialize(renderer);
            yield return dungeonFloor;
        }
    }

    public TileEditor TileEditor { get; } = new();

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

    [RelayCommand]
    private void GetImageMouseClick(object? parameter)
    {
        (int x, int y) = AutomapEventConverters.GetCoordinatesFromEvent(parameter);
        TileEditor.FlagChanged -= UpdateSingleFlag;
        TileEditor.LoadTile(x, y, SelectedFloor!.Tiles[x,y]);
        TileEditor.FlagChanged += UpdateSingleFlag;
    }

    private void UpdateSingleFlag(object? sender, TileFlagChangedEventArgs e)
    {
        
        SelectedFloor?.Renderer?.UpdateTile(e.TileX, e.TileY, e.AllFlags);
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
        SelectedFloor = _cachedFloors[_selectedFloorNum - 1];
    }

    public override string Instructions => "Edit Map";
}
