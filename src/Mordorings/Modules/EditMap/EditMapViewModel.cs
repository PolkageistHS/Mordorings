namespace Mordorings.Modules.EditMap;

public partial class EditMapViewModel : MapViewModelBase
{
    private readonly IDialogFactory _dialogFactory;

    public EditMapViewModel(IMordorIoFactory ioFactory, IMapRendererFactory mapRendererFactory, IDialogFactory dialogFactory) : base(ioFactory, mapRendererFactory)
    {
        _dialogFactory = dialogFactory;
        SelectedFloorNum = 1;
    }

    public TileEditor TileEditor { get; } = new();

    public bool IsTileSelected => TileEditor.TileX is not null && TileEditor.TileY is not null;

    [RelayCommand]
    private void Save()
    {
        if (!_dialogFactory.ShowYesNoQuestion("Do you want to write your changes to the dungeon file?", "Save all"))
            return;
        for (int i = 0; i < MaxFloor; i++)
        {
            Map.Floors[i] = CachedDungeonFloors[i].Floor;
        }
        IoFactory.GetWriter().WriteMordorRecord(Map);
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
        SelectedFloor = CachedDungeonFloors[SelectedFloorNum - 1];
        TileEditor.Clear();
    }

    [RelayCommand]
    private void ResetTile()
    {
        TileEditor.Reset();
    }

    [RelayCommand]
    private void GetImageMouseClick(Tile coords)
    {
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
        var dungeonFloor = new DungeonFloor(Map.Floors[floorNum - 1]);
        CachedDungeonFloors[floorNum - 1] = dungeonFloor;
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

    protected override void HandleOnSelectedFloorNumChanged(int oldValue, int newValue)
    {
        base.HandleOnSelectedFloorNumChanged(oldValue, newValue);
        CurrentRenderer?.RemoveHighlight();
        TileEditor.Clear();
        OnPropertyChanged(nameof(IsTileSelected));
    }

    public override string Instructions => "Edit the dungeon map. Click on a tile to edit its properties.";
}
