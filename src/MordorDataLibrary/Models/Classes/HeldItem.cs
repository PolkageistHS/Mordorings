namespace MordorDataLibrary.Models;

[Serializable]
public class HeldItem
{
    public short Atk { get; set; }

    public short Def { get; set; }

    public short ItemIndex { get; set; }

    public short ItemId { get; set; }

    public short Alignment { get; set; }

    public short Charges { get; set; }

    public short Equipped { get; set; }

    public short IdentificationLevel { get; set; }

    public short Cursed { get; set; }
}
