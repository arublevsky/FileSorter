using FileSorter;

await Runner.RunAsync(sourceFilePath: args[0], fileChunkSizeInBytes: int.Parse(args[1]));
