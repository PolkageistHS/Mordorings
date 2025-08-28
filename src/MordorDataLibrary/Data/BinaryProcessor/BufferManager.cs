namespace MordorDataLibrary.Data;

public class BufferManager(int bufferLength)
{
    private readonly byte[] _buffer = new byte[bufferLength];

    public int Length => _buffer.Length;

    private long Cursor { get; set; }

    public byte[] GetBuffer() => _buffer;

    public void ResetCursor()
    {
        Cursor = 0;
    }

    public void Clear()
    {
        Array.Clear(_buffer, 0, _buffer.Length);
        Cursor = 0;
    }

    public ReadOnlySpan<byte> ReadBytes(int length)
    {
        CheckLength(length);
        Span<byte> span = _buffer.AsSpan((int)Cursor, length);
        Cursor += length;
        return span;
    }

    public Span<byte> GetWriteSpan(int length)
    {
        CheckLength(length);
        Span<byte> span = _buffer.AsSpan((int)Cursor, length);
        Cursor += length;
        return span;
    }

    public void WriteToStream(Stream stream)
    {
        stream.Write(_buffer, 0, _buffer.Length);
    }

    private void CheckLength(int length)
    {
        Debug.Assert(Cursor + length <= _buffer.Length, $"Buffer overflow: cursor={Cursor}, length={length}, buffer size={_buffer.Length}");
    }
}
