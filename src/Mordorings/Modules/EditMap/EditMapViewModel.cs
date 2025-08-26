using Mordorings.Controls;

namespace Mordorings.Modules.EditMap;

public partial class EditMapViewModel : ViewModelBase
{
    private const string SpriteSheetFile = "Assets/DungeonSprites.bmp";
    private readonly IMordorIoFactory _ioFactory;
    private readonly IMapRenderFactory _mapRenderFactory;
    private readonly IDialogFactory _dialogFactory;
    private readonly DATA11DungeonMap _map;
    private readonly DungeonFloor[] _cachedDungeonFloors;

    public EditMapViewModel(IMordorIoFactory ioFactory, IMapRenderFactory mapRenderFactory, IDialogFactory dialogFactory)
    {
        _ioFactory = ioFactory;
        _mapRenderFactory = mapRenderFactory;
        _dialogFactory = dialogFactory;
        _map = _ioFactory.GetReader().GetMordorRecord<DATA11DungeonMap>();
        _cachedDungeonFloors = CacheDungeonFloors().ToArray();
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

    private DungeonFloor ResetFloor(int floorNum)
    {
        IAutomapRenderer renderer = _mapRenderFactory.CreateRenderer();
        renderer.LoadSpriteSheet(SpriteSheetFile);
        var dungeonFloor = new DungeonFloor(_map.Floors[floorNum - 1]);
        dungeonFloor.Initialize(renderer);
        _cachedDungeonFloors[floorNum - 1] = dungeonFloor;
        return dungeonFloor;
    }

    public TileEditor TileEditor { get; } = new();

    public bool IsTileSelected => TileEditor.TileX is not null && TileEditor.TileY is not null;

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
    private void Save()
    {
        if (!_dialogFactory.ShowYesNoQuestion("Do you want to write your changes to the dungeon file?", "Save all"))
            return;
        for (int i = 0; i < 15; i++)
        {
            _map.Floors[i] = _cachedDungeonFloors[i].Floor;
        }
        _ioFactory.GetWriter().WriteMordorRecord(_map);
    }

    [RelayCommand]
    private void ResetFloor()
    {
        if (!_dialogFactory.ShowYesNoQuestion("Do you want to reset the floor?", "Reset floor"))
            return;
        SelectedFloor = ResetFloor(SelectedFloorNum);
        TileEditor.Clear();
    }

    [RelayCommand]
    private void ResetAllFloors()
    {
        if (!_dialogFactory.ShowYesNoQuestion("Do you want to reset all floors?", "Reset all"))
            return;
        for (int i = 1; i <= 15; i++)
        {
            ResetFloor(i);
        }
        SelectedFloor = _cachedDungeonFloors[SelectedFloorNum - 1];
        TileEditor.Clear();
    }

    [RelayCommand]
    private void ResetTile()
    {
        TileEditor.Reset();
    }

    [RelayCommand]
    private void GetImageMouseClick(object? parameter)
    {
        (int x, int y) = AutomapEventConverters.GetCoordinatesFromEvent(parameter);
        if (x is < 0 or >= 30 || y is < 0 or >= 30)
            return;
        TileEditor.FlagChanged -= UpdateTile;
        if (SelectedFloor != null)
        {
            SelectedFloor.Renderer?.DrawDungeonFloorMap();
            TileEditor.LoadTile(x, y, SelectedFloor.Tiles[x, y], SelectedFloor.GetTeleporter(x, y), SelectedFloor.GetChute(x, y));
            SelectedFloor.Renderer?.HighlightTile(x, y);
        }
        TileEditor.FlagChanged += UpdateTile;
        OnPropertyChanged(nameof(IsTileSelected));
    }

    private void UpdateTile(object? sender, TileFlagChangedEventArgs e)
    {
        if (SelectedFloor is null)
            return;
        DungeonTileFlag flags = e.AllFlags;
        MapObjects mapObjs = e.MapObjects;
        int x = e.TileX;
        int y = e.TileY;
        ProcessTeleporterChange(flags.HasFlag(DungeonTileFlag.Teleporter), x, y, mapObjs);
        ProcessChuteChange(flags.HasFlag(DungeonTileFlag.Chute), x, y, mapObjs.ChuteDepth);
        SelectedFloor.Renderer?.UpdateTile(x, y, flags);
    }

    private void ProcessTeleporterChange(bool hasTeleporter, int tileX, int tileY, MapObjects mapObjects)
    {
        if (SelectedFloor is null)
            return;
        if (hasTeleporter)
        {
            int x;
            int y;
            int z;
            if (mapObjects.TeleporterRandom)
            {
                x = 0;
                y = 0;
                z = 0;
            }
            else
            {
                if (mapObjects is not { TeleporterX: not null, TeleporterY: not null, TeleporterZ: not null })
                    return;
                z = mapObjects.TeleporterZ.Value;
                x = mapObjects.TeleporterX.Value;
                y = mapObjects.TeleporterY.Value;
            }
            if (!SelectedFloor.SaveTeleporter(tileX, tileY, x, y, z))
            {
                _dialogFactory.ShowErrorMessage("Unable to save teleporter. Only 20 teleporters are allowed per floor.", "Error");
                TileEditor.Teleporter = false;
            }
        }
        else
        {
            SelectedFloor.DeleteTeleporter(tileX, tileY);
        }
    }

    private void ProcessChuteChange(bool hasChute, int tileX, int tileY, int chuteDepth)
    {
        if (SelectedFloor is null)
            return;
        if (hasChute)
        {
            if (!SelectedFloor.SaveChute(tileX, tileY, chuteDepth))
            {
                _dialogFactory.ShowErrorMessage("Unable to save chute. Only 10 chutes are allowed per floor.", "Error");
                TileEditor.Chute = false;
            }
        }
        else
        {
            SelectedFloor.DeleteChute(tileX, tileY);
        }
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
        SelectedFloor = _cachedDungeonFloors[SelectedFloorNum - 1];
        if (SelectedFloorNum == oldValue)
            return;
        SelectedFloor.Renderer?.RemoveHighlight();
        TileEditor.Clear();
        OnPropertyChanged(nameof(IsTileSelected));
    }

    public override string Instructions => "Edit the dungeon map. Click on a tile to edit its properties.";
}
