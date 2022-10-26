using System.Text;

namespace FileSorter;

public static class FileSplitter
{
    public static void SplitIntoFileChunks(string filePath, int chunkSizeInBytes)
    {
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
        using var bs = new BufferedStream(fs);
        var reader = new StreamReader(bs, Encoding.UTF8, true);
        var bucket = new List<StringLine>(chunkSizeInBytes / 20);
        var batchSize = 0;
        var batchNo = 0;
    
        while (reader.ReadLine() is { } line)
        {
            bucket.Add(new StringLine(line));
            batchSize += Encoding.UTF8.GetByteCount(line);
            if (batchSize > chunkSizeInBytes)
            {
                WriteStringLines(bucket, batchNo);
                batchNo++;
                batchSize = 0;
                bucket.Clear();
            }
        }

        if (bucket.Any())
        {
            WriteStringLines(bucket, batchNo);
        }
    }

    private static void WriteStringLines(List<StringLine> list, int batchNo)
    {
        var sb = new StringBuilder();
        list.Sort(StringLine.Comparer);
        foreach (var kvp in list)
        {
            sb.AppendLine(kvp.ToString());
        }

        var tmpFileName = $"tmp_{batchNo}.txt";
        using var writeStream = new FileStream(tmpFileName, FileMode.Create, FileAccess.Write, FileShare.None);
        writeStream.Write(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}