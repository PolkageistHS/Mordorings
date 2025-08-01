namespace MordorDataLibrary.Models;

public class DungeonMapTile
{
    [NewRecord]
    public short Area { get; set; }

    public long Tile { get; set; }
}
