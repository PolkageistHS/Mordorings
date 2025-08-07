namespace MordorDataLibrary.Data;

public class BufferedWriteStrategy(FileStream file, int bufferLength) : IWriteStrategy
{
    private readonly BufferManager _bufferManager = new(bufferLength);

    public void WriteBytes(ReadOnlySpan<byte> bytes)
    {
        Span<byte> span = _bufferManager.GetWriteSpan(bytes.Length);
        bytes.CopyTo(span);
    }

    public void WriteBytes(byte[] bytes, int maxLength)
    {
        Span<byte> span = _bufferManager.GetWriteSpan(maxLength);
        int bytesToCopy = Math.Min(bytes.Length, maxLength);
        bytes.AsSpan(0, bytesToCopy).CopyTo(span);
        if (bytesToCopy < maxLength)
        {
            span[bytesToCopy..].Clear();
        }
    }

    public void Flush()
    {
        _bufferManager.WriteToStream(file);
        _bufferManager.Clear();
    }
}
