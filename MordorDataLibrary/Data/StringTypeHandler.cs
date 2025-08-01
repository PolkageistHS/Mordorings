using System.Reflection;

namespace MordorDataLibrary.Data;

public class StringTypeHandler : ITypeHandler
{
    public object ReadValue(MdrReader reader, PropertyInfo propertyInfo) =>
        reader.GetString(propertyInfo.GetFixedStringLength());

    public void WriteValue(MdrWriter writer, object value, PropertyInfo propertyInfo)
    {
        writer.PutString((string)value, propertyInfo.GetFixedStringLength());
    }
}
