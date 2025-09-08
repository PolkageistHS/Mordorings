namespace Mordorings.Modules.EditMap;

public partial class EditMapViewModel : ViewModelBase
{
    private readonly IDialogFactory _dialogFactory;
    private const string SpriteSheetFile = "Assets/DungeonSprites.bmp";

    private const int MinFloor = 1;
    private const int MaxFloor = 15;
    private const int FloorWidth = 30;
    private const int FloorHeight = 30;
    private readonly DungeonFloor[] _cachedDungeonFloors;
    private readonly DATA11DungeonMap _map;
    private readonly IMapRendererFactory _mapRendererFactory;
    private readonly IAutomapRenderer[] _renderers;
    private readonly MordorRecordWriter _writer;

    public EditMapViewModel(IMordorIoFactory ioFactory, IMapRendererFactory mapRendererFactory, IDialogFactory dialogFactory)
    {
        _mapRendererFactory = mapRendererFactory;
        _dialogFactory = dialogFactory;
        _writer = ioFactory.GetWriter();
        _map = ioFactory.GetReader().GetMordorRecord<DATA11DungeonMap>();
        int floorCount = _map.Floors.Length;
        _cachedDungeonFloors = new DungeonFloor[floorCount];
        _renderers = new IAutomapRenderer[floorCount];
        InitializeRenderers();
        SelectedFloorNum = 1;
    }

    public TileEditor TileEditor { get; } = new();

    public bool IsTileSelected => TileEditor.TileX is not null && TileEditor.TileY is not null;

    private void InitializeRenderers()
    {
        Floor[] floors = _map.Floors;
        for (int i = 0; i < floors.Length; i++)
        {
            var dungeonFloor = new DungeonFloor(floors[i]);
            _cachedDungeonFloors[i] = dungeonFloor;
            IAutomapRenderer renderer = _mapRendererFactory.CreateAutomapRenderer();
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
            _renderers[i] = renderer;
        }
    }

    private IAutomapRenderer? CurrentRenderer
    {
        get
        {
            if (SelectedFloorNum is >= MinFloor and <= MaxFloor)
                return _renderers[SelectedFloorNum - 1];
            return null;
        }
    }

    [ObservableProperty]
    private DungeonFloor? _selectedFloor;

    [ObservableProperty]
    private object? _image;

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
        SelectedFloorNum = NormalizeFloorNum(SelectedFloorNum + 1);
    }

    [RelayCommand(CanExecute = nameof(CanDecreaseFloor))]
    private void DecreaseFloor()
    {
        SelectedFloorNum = NormalizeFloorNum(SelectedFloorNum - 1);
    }

    protected bool CanIncreaseFloor => SelectedFloorNum < MaxFloor;

    protected bool CanDecreaseFloor => SelectedFloorNum > MinFloor;

    [RelayCommand]
    private void Save()
    {
        if (!_dialogFactory.ShowYesNoQuestion("Do you want to write your changes to the dungeon file?", "Save all"))
            return;
        for (int i = 0; i < MaxFloor; i++)
        {
            _map.Floors[i] = _cachedDungeonFloors[i].Floor;
        }
        _writer.WriteMordorRecord(_map);
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
        for (int floorNum = MinFloor; floorNum <= MaxFloor; floorNum++)
        {
            ResetFloor(floorNum);
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
    private void GetImageMouseClick(object? args)
    {
        Tile coords = AutomapEventConversion.GetMapCoordinatesFromEvent(args);
        int x = coords.X;
        int y = coords.Y;
        if (x is < 0 or >= FloorWidth || y is < 0 or >= FloorHeight)
            return;
        TileEditor.FlagChanged -= UpdateTile;
        if (SelectedFloor != null)
        {
            CurrentRenderer?.DrawDungeonFloorMap();
            TileEditor.LoadTile(coords, SelectedFloor.Tiles[x, y], SelectedFloor.GetTeleporter(coords), SelectedFloor.GetChute(coords));
            CurrentRenderer?.HighlightTile(coords);
        }
        TileEditor.FlagChanged += UpdateTile;
        OnPropertyChanged(nameof(IsTileSelected));
    }

    private DungeonFloor ResetFloor(int floorNum)
    {
        var dungeonFloor = new DungeonFloor(_map.Floors[floorNum - 1]);
        _cachedDungeonFloors[floorNum - 1] = dungeonFloor;
        return dungeonFloor;
    }

    private void UpdateTile(object? sender, TileFlagChangedEventArgs e)
    {
        if (SelectedFloor is null)
            return;
        DungeonTileFlag flags = e.AllFlags;
        MapObjects mapObjs = e.MapObjects;
        ProcessTeleporterChange(flags.HasFlag(DungeonTileFlag.Teleporter), e.Tile, mapObjs);
        ProcessChuteChange(flags.HasFlag(DungeonTileFlag.Chute), e.Tile, mapObjs.ChuteDepth);
        CurrentRenderer?.UpdateTile(e.Tile, flags);
    }

    private void ProcessTeleporterChange(bool hasTeleporter, Tile tile, MapObjects mapObjects)
    {
        if (SelectedFloor is null)
            return;
        if (hasTeleporter)
        {
            int x2;
            int y2;
            int z2;
            if (mapObjects.TeleporterRandom)
            {
                x2 = 0;
                y2 = 0;
                z2 = 0;
            }
            else
            {
                if (mapObjects is not { TeleporterX: not null, TeleporterY: not null, TeleporterZ: not null })
                    return;
                z2 = mapObjects.TeleporterZ.Value;
                x2 = mapObjects.TeleporterX.Value;
                y2 = mapObjects.TeleporterY.Value;
            }
            if (SelectedFloor.SaveTeleporter(tile, x2, y2, z2))
                return;
            _dialogFactory.ShowErrorMessage("Unable to save teleporter. Only 20 teleporters are allowed per floor.", "Error");
            TileEditor.Teleporter = false;
        }
        else
        {
            SelectedFloor.DeleteTeleporter(tile);
        }
    }

    private void ProcessChuteChange(bool hasChute, Tile tile, int chuteDepth)
    {
        if (SelectedFloor is null)
            return;
        if (hasChute)
        {
            if (SelectedFloor.SaveChute(tile, chuteDepth))
                return;
            _dialogFactory.ShowErrorMessage("Unable to save chute. Only 10 chutes are allowed per floor.", "Error");
            TileEditor.Chute = false;
        }
        else
        {
            SelectedFloor.DeleteChute(tile);
        }
    }

    private void HandleOnSelectedFloorNumChanged(int oldValue, int newValue)
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
                actualValue = NormalizeFloorNum(newValue);
            }
            _selectedFloorNum = actualValue;
        }
        SelectedFloor = _cachedDungeonFloors[SelectedFloorNum - 1];
        CurrentRenderer?.RemoveHighlight();
        TileEditor.Clear();
        OnPropertyChanged(nameof(IsTileSelected));
    }

    private static int NormalizeFloorNum(int newValue) => Math.Clamp(newValue, MinFloor, MaxFloor);

    public override string Instructions => "Edit the dungeon map. Click on a tile to edit its properties.";
}
