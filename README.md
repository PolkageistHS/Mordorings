# Mordorings

A growing collection of libraries and utilities for Mordor: The Depths of Dejenol.

## MordorDataLibrary
A C# library for reading and writing MDR files. 

Reader usage:
```csharp
// It should go without saying, but back up your files before trying to edit them. 
MordorRecordReader reader = new(@"C:\Mordor\Data"); // Folder containing the MDR files
DATA04Characters data = reader.GetMordorRecord<DATA04Characters>(); // Read the contents of the MDR file. Data files are found in \Models\DataFiles.
// Read, edit, do whatever you want with the data
MordorRecordWriter writer = new(@"C:\Temp\MordorOutput");
writer.WriteMordorRecord(data); // Writes the entire file back to MDR format. 
```
