namespace Mordorings.Modules;

public partial class TileEditor : ObservableObject
{
    public TileEditor()
    {
        MapObjects.ObjectsUpdated += (_, _) => NotifyTileChanged();
    }

    private long? _originalFlags;

    public event EventHandler<TileFlagChangedEventArgs>? FlagChanged;

    public void LoadTile(Tile tile, long flags, Teleporter? teleporter, Chute? chute)
    {
        TileX = tile.X + 1;
        TileY = tile.Y + 1;
        _originalFlags = flags;
        LoadTileFlags((DungeonTileFlag)flags);
        if (teleporter is not null)
        {
            if (teleporter is { X2: 0, Y2: 0 })
            {
                MapObjects.TeleporterRandom = true;
                MapObjects.TeleporterX = null;
                MapObjects.TeleporterY = null;
                MapObjects.TeleporterZ = null;
            }
            else
            {
                MapObjects.TeleporterRandom = false;
                MapObjects.TeleporterX = teleporter.X2;
                MapObjects.TeleporterY = teleporter.Y2;
                MapObjects.TeleporterZ = teleporter.Z2;
            }
        }
        if (chute is not null)
        {
           MapObjects.ChuteDepth = chute.Depth;
        }
    }

    public void Clear()
    {
        TileX = null;
        TileY = null;
        LoadTileFlags(0);
    }

    public void Reset()
    {
        LoadTileFlags((DungeonTileFlag?)_originalFlags ?? 0);
    }

    private void LoadTileFlags(DungeonTileFlag flags)
    {
        WallEast = flags.HasFlag(DungeonTileFlag.WallEast);
        WallNorth = flags.HasFlag(DungeonTileFlag.WallNorth);
        DoorEast = flags.HasFlag(DungeonTileFlag.DoorEast);
        DoorNorth = flags.HasFlag(DungeonTileFlag.DoorNorth);
        SecretDoorEast = flags.HasFlag(DungeonTileFlag.SecretDoorEast);
        SecretDoorNorth = flags.HasFlag(DungeonTileFlag.SecretDoorNorth);
        FaceNorth = flags.HasFlag(DungeonTileFlag.FaceNorth);
        FaceEast = flags.HasFlag(DungeonTileFlag.FaceEast);
        FaceSouth = flags.HasFlag(DungeonTileFlag.FaceSouth);
        FaceWest = flags.HasFlag(DungeonTileFlag.FaceWest);
        Extinguisher = flags.HasFlag(DungeonTileFlag.Extinguisher);
        Pit = flags.HasFlag(DungeonTileFlag.Pit);
        StairsUp = flags.HasFlag(DungeonTileFlag.StairsUp);
        StairsDown = flags.HasFlag(DungeonTileFlag.StairsDown);
        Teleporter = flags.HasFlag(DungeonTileFlag.Teleporter);
        Water = flags.HasFlag(DungeonTileFlag.Water);
        Quicksand = flags.HasFlag(DungeonTileFlag.Quicksand);
        Rotator = flags.HasFlag(DungeonTileFlag.Rotator);
        Antimagic = flags.HasFlag(DungeonTileFlag.Antimagic);
        Rock = flags.HasFlag(DungeonTileFlag.Rock);
        Fog = flags.HasFlag(DungeonTileFlag.Fog);
        Chute = flags.HasFlag(DungeonTileFlag.Chute);
        Stud = flags.HasFlag(DungeonTileFlag.Stud);
    }

    private DungeonTileFlag GetTileFlag()
    {
        DungeonTileFlag flags = 0;
        if (WallEast) { flags |= DungeonTileFlag.WallEast; }
        if (WallNorth) { flags |= DungeonTileFlag.WallNorth; }
        if (DoorEast) { flags |= DungeonTileFlag.DoorEast; }
        if (DoorNorth) { flags |= DungeonTileFlag.DoorNorth; }
        if (SecretDoorEast) { flags |= DungeonTileFlag.SecretDoorEast; }
        if (SecretDoorNorth) { flags |= DungeonTileFlag.SecretDoorNorth; }
        if (FaceNorth) { flags |= DungeonTileFlag.FaceNorth; }
        if (FaceEast) { flags |= DungeonTileFlag.FaceEast; }
        if (FaceSouth) { flags |= DungeonTileFlag.FaceSouth; }
        if (FaceWest) { flags |= DungeonTileFlag.FaceWest; }
        if (Extinguisher) { flags |= DungeonTileFlag.Extinguisher; }
        if (Pit) { flags |= DungeonTileFlag.Pit; }
        if (StairsUp) { flags |= DungeonTileFlag.StairsUp; }
        if (StairsDown) { flags |= DungeonTileFlag.StairsDown; }
        if (Teleporter) { flags |= DungeonTileFlag.Teleporter; }
        if (Water) { flags |= DungeonTileFlag.Water; }
        if (Quicksand) { flags |= DungeonTileFlag.Quicksand; }
        if (Rotator) { flags |= DungeonTileFlag.Rotator; }
        if (Antimagic) { flags |= DungeonTileFlag.Antimagic; }
        if (Rock) { flags |= DungeonTileFlag.Rock; }
        if (Fog) { flags |= DungeonTileFlag.Fog; }
        if (Chute) { flags |= DungeonTileFlag.Chute; }
        if (Stud) { flags |= DungeonTileFlag.Stud; }
        return flags;
    }

    private void NotifyTileChanged()
    {
        if (TileX == null || TileY == null)
            return;
        var args = new TileFlagChangedEventArgs(new Tile(TileX.Value - 1, TileY.Value - 1), GetTileFlag(), MapObjects);
        FlagChanged?.Invoke(this, args);
    }

    [ObservableProperty]
    private int? _tileX;

    [ObservableProperty]
    private int? _tileY;

    [ObservableProperty]
    private bool _wallEast;

    partial void OnWallEastChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _wallNorth;

    partial void OnWallNorthChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _doorEast;

    partial void OnDoorEastChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _doorNorth;

    partial void OnDoorNorthChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _secretDoorEast;

    partial void OnSecretDoorEastChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _secretDoorNorth;

    partial void OnSecretDoorNorthChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _faceNorth;

    partial void OnFaceNorthChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _faceEast;

    partial void OnFaceEastChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _faceSouth;

    partial void OnFaceSouthChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _faceWest;

    partial void OnFaceWestChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _extinguisher;

    partial void OnExtinguisherChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _pit;

    partial void OnPitChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _stairsUp;

    partial void OnStairsUpChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _stairsDown;

    partial void OnStairsDownChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _teleporter;

    partial void OnTeleporterChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _water;

    partial void OnWaterChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _quicksand;

    partial void OnQuicksandChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _rotator;

    partial void OnRotatorChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _antimagic;

    partial void OnAntimagicChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _rock;

    partial void OnRockChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _fog;

    partial void OnFogChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _chute;

    partial void OnChuteChanged(bool value) => NotifyTileChanged();

    [ObservableProperty]
    private bool _stud;

    partial void OnStudChanged(bool value) => NotifyTileChanged();

    public MapObjects MapObjects { get; } = new();
}
