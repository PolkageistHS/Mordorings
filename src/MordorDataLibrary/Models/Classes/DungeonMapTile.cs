namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class DungeonMapTile
{
    [NewRecord]
    public short Area { get; set; }

    public long Tile { get; set; }
}
