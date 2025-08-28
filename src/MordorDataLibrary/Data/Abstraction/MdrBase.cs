namespace MordorDataLibrary.Data;

public abstract class MdrBase : IDisposable
{
    protected readonly FileStream File;
    private bool _disposed;

    protected MdrBase(string filename, FileMode mode, FileAccess access)
    {
        File = new FileStream(filename, mode, access);
        if (File == null)
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
            File.Dispose();
        }
        _disposed = true;
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
