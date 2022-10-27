# FileSorter
Large files sorter

Prerequisites: .NET 6 SDK 

How to use:

1. Navigate to `FileSorter` folder
2. Run `dotnet build -c release`
3. Run `dotnet ./build/FileGenerator.dll <file_path> <file_size_in_bytes>` to generate test file.
4. Run `dotnet ./build/FileSorter.dll <source_file_path> <chunk_size_in_bytes>`.

`chunk_size` represents a file size to which source file will be split. For 1Gb chunk size of 2MB (2097152) showed better performace. For 100GB test run I would probably use 5MB or 10MB file size.

Sorted file will apper next to the source file with a name `tmp_<GUID>.txt`.

Best score for 1GB test file:
```
Split finished: 23.4119435
Merge finished: 66.5086453
```
