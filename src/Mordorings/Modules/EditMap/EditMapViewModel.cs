namespace Mordorings.Modules;

public partial class EditMapViewModel : ViewModelBase
{
    private readonly IDialogFactory _dialogFactory;
    private readonly IEditMapMediator _mediator;

    public EditMapViewModel(IEditMapMediator mediator, IDialogFactory dialogFactory)
    {
        _mediator = mediator;
        _dialogFactory = dialogFactory;
        _mediator.Initialize();
        InitializeRenderers();
        SelectedFloorNum = 1;
    }

    public TileEditor TileEditor { get; } = new();

    public bool IsTileSelected => TileEditor.TileX is not null && TileEditor.TileY is not null;

    private void InitializeRenderers()
    {
        foreach (IAutomapRenderer renderer in _mediator.Renderers)
        {
            renderer.MapUpdated += (sender, _) =>
            {
                if (sender is IAutomapRenderer iRenderer)
                {
                    Image = iRenderer.GetMapSnapshot()?.ToBitmapSource();
                }
            };
        }
    }

    private IAutomapRenderer? CurrentRenderer
    {
        get
        {
            if (SelectedFloorNum is < Game.MinFloor or > Game.MaxFloor)
                return null;
            return _mediator.Renderers[SelectedFloorNum - 1];
        }
    }

    private DungeonFloor SelectedFloor => _mediator.DungeonFloors[SelectedFloorNum - 1];

    private object? _image;

    public object? Image
    {
        get => _image;
        private set => SetProperty(ref _image, value);
    }

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

    protected bool CanIncreaseFloor => SelectedFloorNum < Game.MaxFloor;

    protected bool CanDecreaseFloor => SelectedFloorNum > Game.MinFloor;

    [RelayCommand]
    private void Save()
    {
        if (_dialogFactory.ShowYesNoQuestion("Do you want to write your changes to the dungeon file?", "Save all"))
        {
            _mediator.SaveAll();
        }
    }

    [RelayCommand]
    private void ResetFloor()
    {
        if (!_dialogFactory.ShowYesNoQuestion("Do you want to reset the floor?", "Reset floor"))
            return;
        ResetFloor(SelectedFloorNum);
        TileEditor.Clear();
    }

    [RelayCommand]
    private void ResetAllFloors()
    {
        if (!_dialogFactory.ShowYesNoQuestion("Do you want to reset all floors?", "Reset all"))
            return;
        for (int floorNum = Game.MinFloor; floorNum <= Game.MaxFloor; floorNum++)
        {
            ResetFloor(floorNum);
        }
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
        if (x is < 0 or >= Game.FloorWidth || y is < 0 or >= Game.FloorHeight)
            return;
        TileEditor.FlagChanged -= UpdateTile;
        CurrentRenderer?.DrawDungeonFloorMap();
        TileEditor.LoadTile(coords, SelectedFloor.Tiles[x, y], SelectedFloor.GetTeleporter(coords), SelectedFloor.GetChute(coords));
        CurrentRenderer?.HighlightTile(coords);
        TileEditor.FlagChanged += UpdateTile;
        OnPropertyChanged(nameof(IsTileSelected));
    }

    private void ResetFloor(int floorNum)
    {
        _mediator.ResetFloor(floorNum);
    }

    private void UpdateTile(object? sender, TileFlagChangedEventArgs e)
    {
        DungeonTileFlag flags = e.AllFlags;
        MapObjects mapObjs = e.MapObjects;
        Tile tile = e.Tile;
        ProcessTeleporterChange(flags.HasFlag(DungeonTileFlag.Teleporter), tile, mapObjs);
        ProcessChuteChange(flags.HasFlag(DungeonTileFlag.Chute), tile, mapObjs.ChuteDepth);
        CurrentRenderer?.UpdateTile(tile, flags);
    }

    private void ProcessTeleporterChange(bool hasTeleporter, Tile tile, MapObjects mapObjects)
    {
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
        if (newValue is < Game.MinFloor or > Game.MaxFloor)
        {
            int actualValue;
            if (oldValue is >= Game.MinFloor and <= Game.MaxFloor)
            {
                actualValue = oldValue;
            }
            else
            {
                actualValue = NormalizeFloorNum(newValue);
            }
            _selectedFloorNum = actualValue;
        }
        CurrentRenderer?.RemoveHighlight();
        TileEditor.Clear();
        OnPropertyChanged(nameof(IsTileSelected));
    }

    private static int NormalizeFloorNum(int newValue) => Math.Clamp(newValue, Game.MinFloor, Game.MaxFloor);

    public override string Instructions => "Edit the dungeon map. Click on a tile to edit its properties.";
}
