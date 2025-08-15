using MordorDataLibrary.Data;

namespace Mordorings.Factories;

public interface IMordorIoFactory
{
    MordorRecordReader GetReader();

    MordorRecordWriter GetWriter();
}
