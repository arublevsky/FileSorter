using System.Diagnostics;
using FileSorter;

var fileName = args[0];
var memoryCapacityInBytes = int.Parse(args[1]);
var sw = new Stopwatch();
sw.Start();
FileSplitter.SplitIntoFileChunks(Path.Combine("C:/temp", fileName), memoryCapacityInBytes);
Console.WriteLine("Split finished: " + sw.Elapsed.TotalSeconds);
await FileMerger.MergeInOrderAsync();
sw.Stop();
Console.WriteLine("Merge finished: " + sw.Elapsed.TotalSeconds);