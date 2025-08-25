namespace MordorDataLibrary.Data;

public interface ITypeHandler
{
    object ReadValue(MdrReader reader, PropertyInfo propertyInfo);

    void WriteValue(MdrWriter writer, object value, PropertyInfo propertyInfo);
}
