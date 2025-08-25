namespace MordorDataLibrary.Models;

[Serializable]
public class AutomapFloor
{
    [NewRecord]
    public int Unused { get; set; }

    public AutomapTile[] Tiles { get; set; } =  new AutomapTile[900];
}
