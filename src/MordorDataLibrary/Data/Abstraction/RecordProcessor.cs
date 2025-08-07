namespace MordorDataLibrary.Data;

public abstract class RecordProcessor
{
    protected readonly IFilePathStrategy _filePathStrategy;

    protected RecordProcessor(string folder)
    {
        _filePathStrategy = new FolderBasedFilePathStrategy(folder);
    }

    protected RecordProcessor(IFilePathStrategy filePathStrategy)
    {
        _filePathStrategy = filePathStrategy;
    }

    protected void ProcessRecursive(Type dataClass, object instance, MdrBase readerOrWriter, bool isFirstObjectInListItem)
    {
        bool isFirstProperty = true;
        foreach (PropertyAccessor accessor in CompiledPropertyAccessor.GetAccessors(dataClass))
        {
            ProcessProperty(instance, readerOrWriter, accessor, isFirstObjectInListItem && isFirstProperty);
            isFirstProperty = false;
        }
    }

    private void ProcessProperty(object instance, MdrBase readerOrWriter, PropertyAccessor propertyAccessor, bool isFirstObjectInListItem)
    {
        HandleNewRecordAttribute(readerOrWriter, propertyAccessor, isFirstObjectInListItem);
        ProcessPropertyByType(instance, readerOrWriter, propertyAccessor);
    }

    private static void HandleNewRecordAttribute(MdrBase readerOrWriter, PropertyAccessor propertyAccessor, bool isFirstObjectInListItem)
    {
        if (propertyAccessor.PropertyInfo.HasNewRecordAttribute() && !propertyAccessor.PropertyType.IsArray && !isFirstObjectInListItem)
        {
            readerOrWriter.NextRecord();
        }
    }

    protected abstract void ProcessPropertyByType(object instance, MdrBase readerOrWriter, PropertyAccessor propertyAccessor);

    // Shared helper methods for derived classes
    protected static Type GetArrayElementType(PropertyAccessor propertyAccessor) => propertyAccessor.PropertyType.GetElementType()!;

    protected static Type GetListElementType(PropertyAccessor propertyAccessor) => propertyAccessor.PropertyType.GetGenericArguments()[0];

    protected static bool HasNewRecordAttribute(PropertyAccessor propertyAccessor) => propertyAccessor.PropertyInfo.HasNewRecordAttribute();
}
