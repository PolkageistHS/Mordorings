namespace MordorDataLibrary.Data;

public class MordorRecordWriter : RecordProcessor
{
    public MordorRecordWriter(string outputFolder) : base(outputFolder) { }

    public MordorRecordWriter(IFilePathStrategy filePathStrategy) : base(filePathStrategy) { }

    public void WriteMordorRecord<TDataFile>(TDataFile data) where TDataFile : IMordorDataFile
    {
        using MdrWriter writer = GetWriter<TDataFile>();
        ProcessRecursive(typeof(TDataFile), data, writer, true);
        // Write final buffer if using buffered mode
        if (typeof(TDataFile).GetDataRecordLength() > 0)
        {
            writer.NextRecord();
        }
    }

    private MdrWriter GetWriter<TDataFile>() where TDataFile : IMordorDataFile
    {
        Type dataClassType = typeof(TDataFile);
        string filePath = _filePathStrategy.GetFilePath<TDataFile>();
        return new MdrWriter(filePath, dataClassType.GetDataRecordLength());
    }

    protected override void ProcessPropertyByType(object instance, MdrBase readerOrWriter, PropertyAccessor propertyAccessor)
    {
        Type propertyType = propertyAccessor.PropertyType;
        object value = propertyAccessor.GetValue(instance);
        MdrWriter writer = (MdrWriter)readerOrWriter;
        if (TypeHandlers.TryGetHandler(propertyType, out ITypeHandler? handler))
        {
            handler.WriteValue(writer, value, propertyAccessor.PropertyInfo);
        }
        else if (value is Array array)
        {
            ProcessArray(array, writer, propertyAccessor);
        }
        else if (propertyType.IsGenericList() && value is IList list)
        {
            ProcessList(list, writer, propertyAccessor);
        }
        else
        {
            ProcessComplexObject(value, writer, propertyAccessor);
        }
    }

    private void ProcessArray(Array array, MdrWriter writer, PropertyAccessor propertyAccessor)
    {
        Type elementType = GetArrayElementType(propertyAccessor);
        bool hasNewRecord = HasNewRecordAttribute(propertyAccessor);
        for (int i = 0; i < array.Length; i++)
        {
            if (hasNewRecord)
            {
                writer.NextRecord();
            }
            object arrayElement = array.GetValue(i)!;
            if (TypeHandlers.TryGetHandler(arrayElement.GetType(), out ITypeHandler? handler))
            {
                handler.WriteValue(writer, arrayElement, propertyAccessor.PropertyInfo);
            }
            else
            {
                ProcessRecursive(elementType, arrayElement, writer, false);
            }
        }
    }

    private void ProcessList(IList list, MdrWriter writer, PropertyAccessor propertyAccessor)
    {
        Type listType = GetListElementType(propertyAccessor);
        foreach (object listItem in list.OfType<object>())
        {
            ProcessRecursive(listType, listItem, writer, false);
        }
    }

    private void ProcessComplexObject(object instance, MdrWriter writer, PropertyAccessor propertyAccessor)
    {
        ProcessRecursive(propertyAccessor.PropertyType, instance, writer, false);
    }
}
