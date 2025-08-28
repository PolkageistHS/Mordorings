using System.Buffers.Binary;

namespace MordorDataLibrary.Data;

[DebuggerStepThrough]
public class MdrReader : MdrBase
{
    private readonly BufferManager _bufferManager;

    public MdrReader(string filename, int bufferLength) : base(filename, FileMode.Open, FileAccess.Read)
    {
        int length;
        if (bufferLength > 0) // fixed-length records
        {
            length = bufferLength;
        }
        else
        {
            length = Convert.ToInt32(File.Length);
        }
        _bufferManager = new BufferManager(length);
    }

    public override bool NextRecord()
    {
        ThrowIfDisposed();
        _bufferManager.ResetCursor();
        return File.Read(_bufferManager.GetBuffer(), 0, _bufferManager.Length) > 0;
    }

    public string GetString(ushort length = 0)
    {
        ThrowIfDisposed();
        if (length == 0)
        {
            length = (ushort)GetShort();
        }
        ReadOnlySpan<byte> bytes = _bufferManager.ReadBytes(length);
        return Encoding.UTF8.GetString(bytes);
    }

    public short GetShort()
    {
        ThrowIfDisposed();
        ReadOnlySpan<byte> bytes = _bufferManager.ReadBytes(2);
        return BinaryPrimitives.ReadInt16LittleEndian(bytes);
    }

    public int GetInt() => (int)GetUint();

    public long GetIntCurrency() => GetLong() / 10000;

    public double GetDoubleCurrency() => GetLong() / 10000.0;

    public float GetFloat()
    {
        uint u = GetUint();
        return BitConverter.Int32BitsToSingle((int)u);
    }

    private uint GetUint()
    {
        ThrowIfDisposed();
        ReadOnlySpan<byte> bytes = _bufferManager.ReadBytes(4);
        return BinaryPrimitives.ReadUInt32LittleEndian(bytes);
    }

    private long GetLong()
    {
        ThrowIfDisposed();
        ReadOnlySpan<byte> bytes = _bufferManager.ReadBytes(8);
        return BinaryPrimitives.ReadInt64LittleEndian(bytes);
    }
}
