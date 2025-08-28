using Microsoft.Extensions.DependencyInjection;

namespace Mordorings.Factories;

public class MordorIoFactory(IServiceProvider services) : IMordorIoFactory
{
    public MordorRecordReader GetReader()
    {
        var settings = services.GetRequiredService<IMordoringSettings>();
        return new MordorRecordReader(settings.DataFileFolder);
    }

    public MordorRecordWriter GetWriter()
    {
        var settings = services.GetRequiredService<IMordoringSettings>();
        return new MordorRecordWriter(settings.DataFileFolder);
    }
}
