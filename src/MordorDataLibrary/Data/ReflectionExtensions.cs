using System.Reflection;
using MordorDataLibrary.Models;

namespace MordorDataLibrary.Data;

public static class ReflectionExtensions
{
    public static PropertyInfo[] GetRelevantProperties(this Type type) => type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

    public static bool IsGenericList(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

    public static int GetDataRecordLength(this Type type) => type.GetCustomAttribute<DataRecordLengthAttribute>()!.Length!.Value;

    public static bool HasNewRecordAttribute(this PropertyInfo propertyInfo) => propertyInfo.GetCustomAttribute<NewRecordAttribute>() != null;

    public static ushort GetFixedStringLength(this PropertyInfo propertyInfo) => propertyInfo.GetCustomAttribute<FixedLengthStringAttribute>()?.Length ?? 0;
}
