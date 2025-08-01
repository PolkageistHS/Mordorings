namespace MordorDataLibrary.Data;

public interface IWriteStrategy
{
    void WriteBytes(ReadOnlySpan<byte> bytes);

    void WriteBytes(byte[] bytes, int maxLength);

    void Flush();
}
