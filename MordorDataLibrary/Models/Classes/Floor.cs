namespace MordorDataLibrary.Models;

public class Floor
{
    [NewRecord]
    public short Width { get; set; }

    public short Height { get; set; }

    public short LevelNum { get; set; }

    public short AreaCount { get; set; }

    private short _chuteCount;

    public short ChuteCount
    {
        get => _chuteCount;
        set
        {
            _chuteCount = value;
            if (Chutes.Length == 0)
            {
                Chutes = new Chute[value];
            }
        }
    }

    private short _teleporterCount;

    public short TeleporterCount
    {
        get => _teleporterCount;
        set
        {
            _teleporterCount = value;
            if (Teleporters.Length == 0)
            {
                Teleporters = new Teleporter[value];
            }
        }
    }

    public DungeonMapTile[] Tiles { get; set; } = new DungeonMapTile[900];

    [NewRecord]
    public short Unused1 { get; set; }

    public Area[] Areas { get; set; } = new Area[201];

    [NewRecord]
    public short Unused2 { get; set; }

    public Teleporter[] Teleporters { get; set; } = [];

    [NewRecord]
    public short Unused3 { get; set; }

    public Chute[] Chutes { get; set; } = [];
}
