namespace MordorDataLibrary.Data;

public interface IFilePathStrategy
{
    string GetFilePath<TDataFile>() where TDataFile : IMordorDataFile;
}