
using System.Text;

namespace FileSorter;

public class FileMerger
{
    private const long WriteMemoryBuffer = 100 * 1024 * 1024; // 100 MB
    
    private readonly string _workingDir;

    public FileMerger(string sourceFilePath)
    {
        _workingDir = Path.GetDirectoryName(sourceFilePath)!;
    }
    
    public async Task MergeFileChunksAsync()
    {
        var chunks = CurrentFiles.Chunk(2).ToArray();
        while (chunks.Length > 0)
        {
            await Task.WhenAll(
                chunks.Where(chunk => chunk.Length != 1)
                    .Select(chunk => Task.Run(() => MergeFilesInOrder(chunk[1], chunk[0]))));

            if (CurrentFiles.Length == 1)
            {
                return;
            }

            chunks = CurrentFiles.Chunk(2).ToArray();
        }
    }
    
    private void MergeFilesInOrder(string leftFile, string rightFile)
    {
        var mergedFilePath = Path.Combine(_workingDir, $"tmp_{Guid.NewGuid()}.txt");
        MergeFilesInternal(leftFile, rightFile, mergedFilePath);
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
            
            var left = new StringLine(leftLine!);
            var right = new StringLine(rightLine!);
            
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

    private string[] CurrentFiles => Directory.GetFiles(_workingDir, "tmp_*.txt");

    private static void WriteLineAsync(Stream stream, string line) => stream.Write(Encoding.UTF8.GetBytes(line));

    private static FileStream GetCreateFileStream(string path) => new(path, FileMode.Create, FileAccess.Write, FileShare.None);
    
    private static FileStream GetReadFileStream(string path) => new(path, FileMode.Open, FileAccess.Read, FileShare.None);
    
    private static StreamReader GetStreamReader(Stream s) => new(s, Encoding.UTF8, false);
}