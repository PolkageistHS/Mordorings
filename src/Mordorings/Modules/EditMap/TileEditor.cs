namespace Mordorings.Modules.EditMap;

public partial class TileEditor : ObservableObject
{
    private long? _originalFlags;

    public event EventHandler<TileFlagChangedEventArgs>? FlagChanged;

    public void LoadTile(int x, int y, long flags)
    {
        TileX = x;
        TileY = y;
        _originalFlags = flags;
        LoadTileFlags((DungeonTileFlag)flags);
    }

    public void Clear()
    {
        TileX = null;
        TileY = null;
        LoadTileFlags(0);
    }

    public void Reset()
    {
        if (_originalFlags != null)
        {
            LoadTileFlags((DungeonTileFlag)_originalFlags);
        }
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

    private void NotifyTileChanged(bool isAdded, DungeonTileFlag modifiedFlag)
    {
        if (TileX == null || TileY == null)
            return;
        DungeonTileFlag? addedFlag;
        DungeonTileFlag? removedFlag;
        if (isAdded)
        {
            addedFlag = modifiedFlag;
            removedFlag = null;
        }
        else
        {
            removedFlag = modifiedFlag;
            addedFlag = null;
        }
        FlagChanged?.Invoke(this, new TileFlagChangedEventArgs(TileX.Value, TileY.Value, addedFlag, removedFlag, GetTileFlag()));
    }

    [ObservableProperty]
    private int? _tileX;

    [ObservableProperty]
    private int? _tileY;

    [ObservableProperty]
    private bool _wallEast;

    partial void OnWallEastChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.WallEast);

    [ObservableProperty]
    private bool _wallNorth;

    partial void OnWallNorthChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.WallNorth);

    [ObservableProperty]
    private bool _doorEast;

    partial void OnDoorEastChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.DoorEast);

    [ObservableProperty]
    private bool _doorNorth;

    partial void OnDoorNorthChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.DoorNorth);

    [ObservableProperty]
    private bool _secretDoorEast;

    partial void OnSecretDoorEastChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.SecretDoorEast);

    [ObservableProperty]
    private bool _secretDoorNorth;

    partial void OnSecretDoorNorthChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.SecretDoorNorth);

    [ObservableProperty]
    private bool _faceNorth;

    partial void OnFaceNorthChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.FaceNorth);

    [ObservableProperty]
    private bool _faceEast;

    partial void OnFaceEastChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.FaceEast);

    [ObservableProperty]
    private bool _faceSouth;

    partial void OnFaceSouthChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.FaceSouth);

    [ObservableProperty]
    private bool _faceWest;

    partial void OnFaceWestChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.FaceWest);

    [ObservableProperty]
    private bool _extinguisher;

    partial void OnExtinguisherChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.Extinguisher);

    [ObservableProperty]
    private bool _pit;

    partial void OnPitChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.Pit);

    [ObservableProperty]
    private bool _stairsUp;

    partial void OnStairsUpChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.StairsUp);

    [ObservableProperty]
    private bool _stairsDown;

    partial void OnStairsDownChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.StairsDown);

    [ObservableProperty]
    private bool _teleporter;

    partial void OnTeleporterChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.Teleporter);

    [ObservableProperty]
    private bool _water;

    partial void OnWaterChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.Water);

    [ObservableProperty]
    private bool _quicksand;

    partial void OnQuicksandChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.Quicksand);

    [ObservableProperty]
    private bool _rotator;

    partial void OnRotatorChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.Rotator);

    [ObservableProperty]
    private bool _antimagic;

    partial void OnAntimagicChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.Antimagic);

    [ObservableProperty]
    private bool _rock;

    partial void OnRockChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.Rock);

    [ObservableProperty]
    private bool _fog;

    partial void OnFogChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.Fog);

    [ObservableProperty]
    private bool _chute;

    partial void OnChuteChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.Chute);

    [ObservableProperty]
    private bool _stud;

    partial void OnStudChanged(bool value) => NotifyTileChanged(value, DungeonTileFlag.Stud);
}

public sealed record TileFlagChangedEventArgs(int TileX, int TileY, DungeonTileFlag? AddedTile, DungeonTileFlag? RemovedTile, DungeonTileFlag AllFlags);
