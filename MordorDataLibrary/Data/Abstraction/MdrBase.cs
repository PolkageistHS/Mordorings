namespace MordorDataLibrary.Data;

public abstract class MdrBase : IDisposable
{
    protected readonly FileStream _file;
    private bool _disposed;

    protected MdrBase(string filename, FileMode mode, FileAccess access)
    {
        _file = new FileStream(filename, mode, access);
        if (_file == null)
        {
            throw new Exception($"Unable to open file! {filename}");
        }
    }

    public abstract bool NextRecord();

    ~MdrBase()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        if (disposing)
        {
            _file.Dispose();
        }
        _disposed = true;
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
