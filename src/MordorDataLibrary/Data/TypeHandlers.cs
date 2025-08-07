using System.Diagnostics.CodeAnalysis;

namespace MordorDataLibrary.Data;

public static class TypeHandlers
{
    private static readonly Dictionary<Type, ITypeHandler> _handlers = new()
    {
        { typeof(short), new PrimitiveTypeHandler<short>(reader => reader.GetShort(), (writer, value) => writer.PutShort(value)) },
        { typeof(int), new PrimitiveTypeHandler<int>(reader => reader.GetInt(), (writer, value) => writer.PutInt(value)) },
        { typeof(long), new PrimitiveTypeHandler<long>(reader => reader.GetIntCurrency(), (writer, value) => writer.PutIntCurrency(value)) },
        { typeof(float), new PrimitiveTypeHandler<float>(reader => reader.GetFloat(), (writer, value) => writer.PutFloat(value)) },
        { typeof(double), new PrimitiveTypeHandler<double>(reader => reader.GetDoubleCurrency(), (writer, value) => writer.PutDoubleCurrency(value)) },
        { typeof(string), new StringTypeHandler() }
    };

    public static bool TryGetHandler(Type type, [NotNullWhen(true)] out ITypeHandler? handler) => _handlers.TryGetValue(type, out handler);
}
