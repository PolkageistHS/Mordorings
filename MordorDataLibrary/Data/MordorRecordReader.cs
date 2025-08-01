using System.Collections;
using System.Reflection;
using MordorDataLibrary.Models;

namespace MordorDataLibrary.Data;

public class MordorRecordReader : RecordProcessor
{
    public MordorRecordReader(string outputFolder) : base(outputFolder) { }

    public MordorRecordReader(IFilePathStrategy filePathStrategy) : base(filePathStrategy) { }

    public TDataFile GetMordorRecord<TDataFile>() where TDataFile : IMordorDataFile
    {
        using MdrReader reader = GetReader<TDataFile>();
        TDataFile instance = Activator.CreateInstance<TDataFile>();
        ProcessRecursive(typeof(TDataFile), instance, reader, false);
        return instance;
    }

    private MdrReader GetReader<TDataFile>() where TDataFile : IMordorDataFile
    {
        Type dataClassType = typeof(TDataFile);
        string filePath = _filePathStrategy.GetFilePath<TDataFile>();
        return new MdrReader(filePath, dataClassType.GetDataRecordLength());
    }

    protected override void ProcessPropertyByType(object instance, MdrBase readerOrWriter, PropertyAccessor propertyAccessor)
    {
        Type propertyType = propertyAccessor.PropertyType;
        MdrReader reader = (MdrReader)readerOrWriter;
        if (TypeHandlers.TryGetHandler(propertyType, out ITypeHandler? handler))
        {
            object value = handler.ReadValue(reader, propertyAccessor.PropertyInfo);
            propertyAccessor.SetValue(instance, value);
        }
        else if (propertyType.IsArray)
        {
            ProcessArray(instance, reader, propertyAccessor);
        }
        else if (propertyType.IsGenericList())
        {
            ProcessList(instance, reader, propertyAccessor);
        }
        else
        {
            ProcessComplexObject(instance, reader, propertyAccessor);
        }
    }

    private void ProcessArray(object instance, MdrReader reader, PropertyAccessor propertyAccessor)
    {
        Array array = (Array)propertyAccessor.GetValue(instance);
        Type elementType = GetArrayElementType(propertyAccessor);
        bool hasNewRecord = HasNewRecordAttribute(propertyAccessor);
        for (int i = 0; i < array.Length; i++)
        {
            if (hasNewRecord)
            {
                reader.NextRecord();
            }
            object subInstance = CreateAndPopulateInstance(elementType, reader);
            array.SetValue(subInstance, i);
        }
        propertyAccessor.SetValue(instance, array);
    }

    private void ProcessList(object instance, MdrReader reader, PropertyAccessor propertyAccessor)
    {
        Type listType = GetListElementType(propertyAccessor);
        Type genericListType = typeof(List<>).MakeGenericType(listType);
        object listInstance = Activator.CreateInstance(genericListType)!;
        MethodInfo addMethod = genericListType.GetMethod(nameof(IList.Add))!;
        bool hasNewRecordProperty = CompiledPropertyAccessor.GetAccessors(listType).Any(accessor => accessor.PropertyInfo.HasNewRecordAttribute());
        while (reader.NextRecord())
        {
            object subInstance = Activator.CreateInstance(listType)!;
            ProcessRecursive(listType, subInstance, reader, hasNewRecordProperty);
            addMethod.Invoke(listInstance, [subInstance]);
        }
        propertyAccessor.SetValue(instance, listInstance);
    }

    private void ProcessComplexObject(object instance, MdrReader reader, PropertyAccessor propertyAccessor)
    {
        object subInstance = Activator.CreateInstance(propertyAccessor.PropertyType)!;
        ProcessRecursive(propertyAccessor.PropertyType, subInstance, reader, false);
        propertyAccessor.SetValue(instance, subInstance);
    }

    private object CreateAndPopulateInstance(Type elementType, MdrReader reader)
    {
        if (TypeHandlers.TryGetHandler(elementType, out ITypeHandler? handler))
        {
            return handler.ReadValue(reader, null!);
        }
        object subInstance = Activator.CreateInstance(elementType)!;
        ProcessRecursive(elementType, subInstance, reader, false);
        return subInstance;
    }
}
