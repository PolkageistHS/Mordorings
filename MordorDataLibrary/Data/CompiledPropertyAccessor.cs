using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace MordorDataLibrary.Data;

public static class CompiledPropertyAccessor
{
    private static readonly ConcurrentDictionary<Type, PropertyAccessor[]> _accessorCache = new();

    public static PropertyAccessor[] GetAccessors(Type type) => _accessorCache.GetOrAdd(type, CreateAccessors);

    private static PropertyAccessor[] CreateAccessors(Type type) => type.GetRelevantProperties()
                                                                        .Select(info => new PropertyAccessor(info, CreateGetter(info), CreateSetter(info)))
                                                                        .ToArray();

    private static Func<object, object> CreateGetter(PropertyInfo property)
    {
        ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
        UnaryExpression convert = Expression.Convert(instance, property.DeclaringType!);
        MemberExpression getProperty = Expression.Property(convert, property);
        UnaryExpression convertResult = Expression.Convert(getProperty, typeof(object));
        return Expression.Lambda<Func<object, object>>(convertResult, instance).Compile();
    }

    private static Action<object, object> CreateSetter(PropertyInfo property)
    {
        ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
        ParameterExpression value = Expression.Parameter(typeof(object), "value");
        UnaryExpression convert = Expression.Convert(instance, property.DeclaringType!);
        UnaryExpression convertValue = Expression.Convert(value, property.PropertyType);
        MethodCallExpression setProperty = Expression.Call(convert, property.SetMethod!, convertValue);
        return Expression.Lambda<Action<object, object>>(setProperty, instance, value).Compile();
    }
}

public class PropertyAccessor(PropertyInfo propertyInfo, Func<object, object> getter, Action<object, object> setter)
{
    public PropertyInfo PropertyInfo { get; } = propertyInfo;

    public Func<object, object> Getter { get; } = getter;

    public Action<object, object> Setter { get; } = setter;

    public Type PropertyType => PropertyInfo.PropertyType;

    public object GetValue(object instance) => Getter(instance);

    public void SetValue(object instance, object value) => Setter(instance, value);
}
