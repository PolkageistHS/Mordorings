namespace MordorDataLibrary.Models;

[Serializable]
public class Area
{
    [NewRecord]
    public int SpawnMask { get; set; }

    /// <summary>
    /// If 0 or greater, the MonsterID for the laired monster
    /// </summary>
    public short LairID { get; set; }
}
