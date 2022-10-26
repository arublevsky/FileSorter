using System.Text;

namespace FileSorter;

public static class FileSplitter
{
    public static void SplitIntoFileChunks(string filePath, int chunkSizeInBytes)
    {
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
        using var bs = new BufferedStream(fs);
        var reader = new StreamReader(bs, Encoding.UTF8, true);
        var bucketOfBuckets = new List<List<StringLine>>(100);
        var bucket = new List<StringLine>(chunkSizeInBytes / 20);
        var batchSize = 0;
        long memoryLimit = 50 * 1024 * 1024;
    
        while (reader.ReadLine() is { } line)
        {
            bucket.Add(new StringLine(line));
            batchSize += Encoding.UTF8.GetByteCount(line);
            if (batchSize > chunkSizeInBytes)
            {
                bucketOfBuckets.Add(bucket);
                batchSize = 0;
                bucket = new List<StringLine>();
            }

            if (bucketOfBuckets.Count * chunkSizeInBytes > memoryLimit)
            {
                foreach (var b in bucketOfBuckets)
                {
                    Task.Run(() => WriteStringLines(b.ToList())).ContinueWith(_ => {});
                }

                bucketOfBuckets = new List<List<StringLine>>();
            }
        }

        if (bucket.Any())
        {
            WriteStringLines(bucket);
        }
    }

    private static void WriteStringLines(List<StringLine> list)
    {
        var sb = new StringBuilder();
        list.Sort(StringLine.Comparer);
        foreach (var kvp in list)
        {
            sb.AppendLine(kvp.ToString());
        }

        var tmpFileName = $"tmp_{Guid.NewGuid()}.txt";
        using var writeStream = new FileStream(tmpFileName, FileMode.Create, FileAccess.Write, FileShare.None);
        writeStream.Write(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}