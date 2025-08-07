namespace MordorDataLibrary.Models;

public class ConfinementMonster
{
    [NewRecord]
    public short RowNumber { get; set; }

    public short MonsterID { get; set; }

    public short Good { get; set; }

    public short Neutral { get; set; }

    public short Evil { get; set; }
}
