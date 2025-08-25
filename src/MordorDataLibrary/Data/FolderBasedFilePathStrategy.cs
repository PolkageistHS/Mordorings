namespace MordorDataLibrary.Data;

public class FolderBasedFilePathStrategy(string folder) : IFilePathStrategy
{
    public string GetFilePath<TDataFile>() where TDataFile : IMordorDataFile
    {
        ReadOnlySpan<char> fileNum = typeof(TDataFile).Name.AsSpan().Slice(4, 2);
        string fileName = $"MDATA{int.Parse(fileNum)}.MDR";
        return Path.Combine(folder, fileName);
    }
}
