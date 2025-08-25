namespace MordorDataLibrary.Data;

public class PrimitiveTypeHandler<T>(Func<MdrReader, T> reader, Action<MdrWriter, T> writer) : ITypeHandler where T : notnull
{
    public object ReadValue(MdrReader reader1, PropertyInfo propertyInfo) => reader(reader1);

    public void WriteValue(MdrWriter writer1, object value, PropertyInfo propertyInfo)
    {
        writer(writer1, (T)value);
    }
}
