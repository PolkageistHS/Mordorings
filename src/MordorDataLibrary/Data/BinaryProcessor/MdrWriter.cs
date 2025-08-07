using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace MordorDataLibrary.Data;

[DebuggerStepThrough]
public class MdrWriter : MdrBase
{
    private readonly IWriteStrategy _writeStrategy;

    public MdrWriter(string filename, int bufferLength) : base(filename, FileMode.Create, FileAccess.Write)
    {
        if (bufferLength > 0)
        {
            _writeStrategy = new BufferedWriteStrategy(_file, bufferLength);
        }
        else
        {
            _writeStrategy = new DirectWriteStrategy(_file);
        }
    }

    public override bool NextRecord()
    {
        ThrowIfDisposed();
        _writeStrategy.Flush();
        return true;
    }

    public void PutString(string value, ushort length)
    {
        ThrowIfDisposed();
        byte[] stringBytes = Encoding.UTF8.GetBytes(value);
        if (length == 0)
        {
            // Variable length string - write length field first
            PutShort((short)stringBytes.Length);
            length = (ushort)stringBytes.Length;
        }
        _writeStrategy.WriteBytes(stringBytes, length);
    }

    public void PutShort(short value)
    {
        ThrowIfDisposed();
        Span<byte> bytes = stackalloc byte[2];
        BinaryPrimitives.WriteInt16LittleEndian(bytes, value);
        _writeStrategy.WriteBytes(bytes);
    }

    public void PutInt(int value) => PutUint((uint)value);

    public void PutIntCurrency(long value) => PutLong(value * 10000);

    public void PutDoubleCurrency(double value) => PutLong((long)(value * 10000));

    public void PutFloat(float value)
    {
        int intBits = BitConverter.SingleToInt32Bits(value);
        PutUint((uint)intBits);
    }

    private void PutUint(uint value)
    {
        ThrowIfDisposed();
        Span<byte> bytes = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(bytes, value);
        _writeStrategy.WriteBytes(bytes);
    }

    private void PutLong(long value)
    {
        ThrowIfDisposed();
        Span<byte> bytes = stackalloc byte[8];
        BinaryPrimitives.WriteInt64LittleEndian(bytes, value);
        _writeStrategy.WriteBytes(bytes);
    }
}
