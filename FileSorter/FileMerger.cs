
using System.Text;

namespace FileSorter;

public static class FileMerger
{
    private const long WriteMemoryBuffer = 100 * 1024 * 1024; // 100 MB

    public static void MergeInOrder()
    {
        var chunks = CurrentFiles.Chunk(2).ToArray();

        while (chunks.Length > 0)
        {
            Parallel.ForEach(
                chunks.Where(chunk => chunk.Length != 1),
                (chunk, _) => MergeFilesInOrder(chunk[1], chunk[0]));

            if (CurrentFiles.Length == 1)
            {
                return;
            }

            chunks = CurrentFiles.Chunk(2).ToArray();
        }
    }
    
    private static void MergeFilesInOrder(string leftFile, string rightFile)
    {
        var resultPath = Directory.GetCurrentDirectory() + "\\tmp_" + Guid.NewGuid() + ".txt";
        MergeFilesInternal(leftFile, rightFile, resultPath);
        File.Delete(rightFile);
        File.Delete(leftFile);
    }

    private static void MergeFilesInternal(string leftFile, string rightFile, string resultPath)
    {
        var sb = new StringBuilder();
        using var resultStream = GetCreateFileStream(resultPath);
        using var leftStream = GetReadFileStream(leftFile);
        using var rightStream = GetReadFileStream(rightFile); 
        using var leftReader = GetStreamReader(leftStream);
        using var rightReader = GetStreamReader(rightStream);

        var leftLine = leftReader.ReadLine();
        var rightLine = rightReader.ReadLine();

        while (true)
        {
            if (sb.Length > WriteMemoryBuffer)
            {
                WriteLineAsync(resultStream, sb.ToString());
                sb.Clear();
            }
            
            var left = new StringLine(leftLine);
            var right = new StringLine(rightLine);
            
            var compareResult = StringLine.Comparer.Compare(left, right);
            if (compareResult < 0)
            {
                sb.AppendLine(leftLine);
                leftLine = leftReader.ReadLine();
            }
            else if (compareResult > 0)
            {
                sb.AppendLine(rightLine);
                rightLine = rightReader.ReadLine();
            }
            else
            {
                sb.AppendLine(rightLine);
                sb.AppendLine(leftLine);
                
                leftLine = leftReader.ReadLine();
                rightLine = rightReader.ReadLine();
            }

            if (leftLine == null)
            {
                if (rightLine != null)
                {
                    sb.AppendLine(rightLine);
                }
                
                while (rightReader.ReadLine() is { } line)
                {
                    sb.AppendLine(line);
                }
                
                WriteLineAsync(resultStream, sb.ToString());
                return;
            }

            if (rightLine == null)
            {
                sb.AppendLine(leftLine);
                while (leftReader.ReadLine() is { } line)
                {
                    sb.AppendLine(line);
                }
                
                WriteLineAsync(resultStream, sb.ToString());
                return;
            }
        }
    }

    private static string[] CurrentFiles => Directory.GetFiles(Directory.GetCurrentDirectory(), "tmp_*.txt");

    private static void WriteLineAsync(Stream stream, string line) => stream.Write(Encoding.UTF8.GetBytes(line));

    private static FileStream GetCreateFileStream(string path) => new(path, FileMode.Create, FileAccess.Write, FileShare.None);
    
    private static FileStream GetReadFileStream(string path) => new(path, FileMode.Open, FileAccess.Read, FileShare.None);
    
    private static StreamReader GetStreamReader(Stream s) => new(s, Encoding.UTF8, false);
}