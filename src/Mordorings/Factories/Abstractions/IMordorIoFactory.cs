namespace Mordorings.Factories;

public interface IMordorIoFactory
{
    MordorRecordReader GetReader();

    MordorRecordWriter GetWriter();
}
