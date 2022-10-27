using System.Runtime;

namespace FileSorter;

public static class Runner
{
    public static async Task RunAsync(string sourceFilePath, int fileChunkSizeInBytes)
    {
        GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        await FileSplitter.SplitIntoFileChunks(sourceFilePath, fileChunkSizeInBytes);
        FileMerger.MergeInOrder();
    }
}