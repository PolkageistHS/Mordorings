using Microsoft.Extensions.DependencyInjection;

namespace Mordorings.Factories;

public class MordorIoFactory(IServiceProvider services) : IMordorIoFactory
{
    public MordorRecordReader GetReader()
    {
        IMordoringSettings settings = services.GetRequiredService<IMordoringSettings>();
        return new MordorRecordReader(settings.DataFileFolder);
    }

    public MordorRecordWriter GetWriter()
    {
        IMordoringSettings settings = services.GetRequiredService<IMordoringSettings>();
        return new MordorRecordWriter(settings.DataFileFolder);
    }
}
