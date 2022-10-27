using System.Diagnostics;
using FileSorter;

var sw = Stopwatch.StartNew();
await Runner.RunAsync(sourceFilePath: args[0], fileChunkSizeInBytes: int.Parse(args[1]));
Console.WriteLine(sw.Elapsed.TotalSeconds);
Console.ReadLine();