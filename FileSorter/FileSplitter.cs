using System.Text;

namespace FileSorter;

public static class FileSplitter
{
    public static async Task SplitIntoFileChunks(string filePath, int fileChunkSizeInBytes)
    {
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
        using var bs = new BufferedStream(fs);
        var reader = new StreamReader(fs, Encoding.UTF8, false);
        var bucketOfBuckets = new List<List<StringLine>>(100);
        var bucket = new List<StringLine>(fileChunkSizeInBytes / 20);
        var tasks = new List<Task>(100);
        var batchSize = 0;
        long memoryLimit = 100 * 1024 * 1024;
    
        while (reader.ReadLine() is { } line)
        {
            bucket.Add(new StringLine(line));
            batchSize += Encoding.UTF8.GetByteCount(line);
            if (batchSize > fileChunkSizeInBytes)
            {
                bucketOfBuckets.Add(bucket);
                batchSize = 0;
                bucket = new List<StringLine>();
            }

            if (bucketOfBuckets.Count * fileChunkSizeInBytes > memoryLimit)
            {
                tasks.AddRange(bucketOfBuckets.Select(SortAndWriteFileAsync));
                bucketOfBuckets = new List<List<StringLine>>();
            }
        }

        if (bucketOfBuckets.Any())
        {
            tasks.AddRange(bucketOfBuckets.Select(SortAndWriteFileAsync));
        }

        if (bucket.Any())
        {
            tasks.Add(SortAndWriteFileAsync(bucket));
        }

        await Task.WhenAll(tasks);
    }

    private static async Task SortAndWriteFileAsync(List<StringLine> lines)
    {
        await Task.Yield();
        await File.AppendAllLinesAsync(
            $"tmp_{Guid.NewGuid()}.txt",
            lines.OrderBy(x => x, StringLine.Comparer).Select(x => x.ToString()),
            Encoding.UTF8);
    }
}