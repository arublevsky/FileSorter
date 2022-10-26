
using System.Text;

namespace FileSorter;

public static class FileMerger
{
    public static async Task MergeInOrderAsync()
    {
        var chunks = CurrentFiles.Chunk(2).ToArray();

        while (chunks.Length > 0)
        {
            await Parallel.ForEachAsync(
                chunks.Where(chunk => chunk.Length != 1),
                (chunk, _) => MergeFilesInOrderAsync(chunk[1], chunk[0]));

            if (CurrentFiles.Length == 1)
            {
                return;
            }

            chunks = CurrentFiles.Chunk(2).ToArray();
        }
    }
    
    private static async ValueTask MergeFilesInOrderAsync(string leftFile, string rightFile)
    {
        var resultPath = Directory.GetCurrentDirectory() + "\\tmp_" + Guid.NewGuid() + ".txt";
        await MergeFilesInternal(leftFile, rightFile, resultPath);
        File.Delete(rightFile);
        File.Delete(leftFile);
    }

    private static async Task MergeFilesInternal(string leftFile, string rightFile, string resultPath)
    {
        var fileBatchSize = 1 * 1024 * 1024;
        var sb = new StringBuilder();
        await using var resultStream = new FileStream(resultPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await using var fs1 = new FileStream(leftFile, FileMode.Open, FileAccess.Read, FileShare.None);
        await using var fs2 = new FileStream(rightFile, FileMode.Open, FileAccess.Read, FileShare.None); 
        using var leftReader = new StreamReader(fs1, Encoding.UTF8, true, 4096);
        using var rightReader = new StreamReader(fs2, Encoding.UTF8, true, 4096);

        var leftLine = await leftReader.ReadLineAsync();
        var rightLine = await rightReader.ReadLineAsync();

        while (true)
        {
            if (sb.Length > fileBatchSize)
            {
                await WriteLineAsync(resultStream, sb.ToString());
                sb.Clear();
            }

            var stringLine1 = new StringLine(leftLine);
            var stringLine2 = new StringLine(rightLine);

            if (StringLine.Comparer.Compare(stringLine1, stringLine2) < 0)
            {
                sb.AppendLine(leftLine);
                // await WriteLineAsync(resultStream, leftLine);
                leftLine = await leftReader.ReadLineAsync()!;
            }
            else if (StringLine.Comparer.Compare(stringLine1, stringLine2) > 0)
            {
                sb.AppendLine(rightLine);
                //await WriteLineAsync(resultStream, rightLine);
                rightLine = await rightReader.ReadLineAsync()!;
            }
            else
            {
                // await WriteLineAsync(resultStream, leftLine);
                // await WriteLineAsync(resultStream, rightLine);
                
                sb.AppendLine(rightLine);
                sb.AppendLine(leftLine);
                
                leftLine = await leftReader.ReadLineAsync();
                rightLine = await rightReader.ReadLineAsync();
            }

            if (leftLine == null)
            {
                if (rightLine != null)
                {
                    //await WriteLineAsync(resultStream, rightLine);
                    sb.AppendLine(rightLine);
                }
                
                while (await rightReader.ReadLineAsync() is { } line)
                {
                    // await WriteLineAsync(resultStream, line);
                    sb.AppendLine(line);
                }
                
                await WriteLineAsync(resultStream, sb.ToString());
                return;
            }

            if (rightLine == null)
            {
                //await WriteLineAsync(resultStream, leftLine);
                sb.AppendLine(leftLine);
                while (await leftReader.ReadLineAsync() is { } line)
                {
                    // await WriteLineAsync(resultStream, line);
                    sb.AppendLine(line);
                }
                
                await WriteLineAsync(resultStream, sb.ToString());
                return;
            }
        }
    }

    private static string[] CurrentFiles => Directory.GetFiles(Directory.GetCurrentDirectory(), "tmp_*.txt");

    private static ValueTask WriteLineAsync(Stream stream, string line) => stream.WriteAsync(Encoding.UTF8.GetBytes(line));
}