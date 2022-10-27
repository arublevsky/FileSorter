using System.Text;

namespace FileSorter;

public class FileSplitter
{
    private readonly string _sourceFilePath;
    private readonly int _fileChunkSizeInBytes;

    public FileSplitter(string sourceFilePath, int fileChunkSizeInBytes)
    {
        _sourceFilePath = sourceFilePath;
        _fileChunkSizeInBytes = fileChunkSizeInBytes;
    }

    public async Task SplitIntoFileChunks()
    {
        using var fs = new FileStream(_sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.None);
        using var bs = new BufferedStream(fs);
        var reader = new StreamReader(fs, Encoding.UTF8, false);
        var bucketOfBuckets = new List<List<StringLine>>(100);
        var bucket = new List<StringLine>(_fileChunkSizeInBytes / 20);
        var tasks = new List<Task>(100);
        var batchSize = 0;
        long memoryLimit = 100 * 1024 * 1024;
    
        while (reader.ReadLine() is { } line)
        {
            bucket.Add(new StringLine(line));
            batchSize += Encoding.UTF8.GetByteCount(line);
            if (batchSize > _fileChunkSizeInBytes)
            {
                bucketOfBuckets.Add(bucket);
                batchSize = 0;
                bucket = new List<StringLine>();
            }

            if (bucketOfBuckets.Count * _fileChunkSizeInBytes > memoryLimit)
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

    private async Task SortAndWriteFileAsync(List<StringLine> lines)
    {
        await Task.Yield();
        var chunkFilePath = Path.Combine(Path.GetDirectoryName(_sourceFilePath)!, $"tmp_{Guid.NewGuid()}.txt");
        await File.AppendAllLinesAsync(
            chunkFilePath,
            lines.OrderBy(x => x, StringLine.Comparer).Select(x => x.OriginalLine),
            Encoding.UTF8);
    }
}