namespace MordorDataLibrary.Models;

[Serializable]
public class StoreItem
{
    [NewRecord]
    public short RowNumber { get; set; }

    public short ItemID { get; set; }

    public short UnalignedQty { get; set; }

    public short GoodQty { get; set; }

    public short NeutralQty { get; set; }

    public short EvilQty { get; set; }
}
