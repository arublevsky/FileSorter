using System.Diagnostics;
using System.Runtime;

namespace FileSorter;

public static class Runner
{
    public static async Task RunAsync(string sourceFilePath, int fileChunkSizeInBytes)
    {
        GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        var sw = Stopwatch.StartNew();
        Console.WriteLine("Starting..");
        await new FileSplitter(sourceFilePath, fileChunkSizeInBytes).SplitIntoFileChunks();
        Console.WriteLine($"Split done: {sw.Elapsed.TotalSeconds}");
        await new FileMerger(sourceFilePath).MergeFileChunksAsync();
        Console.WriteLine($"Merge done: {sw.Elapsed.TotalSeconds}");
    }
}