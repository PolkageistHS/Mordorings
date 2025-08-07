namespace MordorDataLibrary.Data;

public class DirectWriteStrategy(FileStream file) : IWriteStrategy
{
    public void WriteBytes(ReadOnlySpan<byte> bytes)
    {
        file.Write(bytes);
    }

    public void WriteBytes(byte[] bytes, int maxLength)
    {
        file.Write(bytes, 0, Math.Min(bytes.Length, maxLength));
    }

    public void Flush() { }
}
