using System.Diagnostics;
using System.Text;
using FileSorter;

var fileName = args[0];
int memoryCapacityInBytes = int.Parse(args[1]);
var filePath = Path.Combine("C:/temp", fileName);

var sw = new Stopwatch();
sw.Start();
await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
var reader = new StreamReader(fs, Encoding.UTF8, true, 128);

var dict = new Dictionary<string, StringLine>();

var batchSize = 0;
var batchNo = 0;
while (await reader.ReadLineAsync() is { } line)
{
    var parts = line.Split(". ");
    dict.Add(line, new StringLine(int.Parse(parts[0]), parts[1]));
    batchSize += Encoding.UTF8.GetByteCount(line);
    if (batchSize > memoryCapacityInBytes)
    {
        await using var writeStream = new FileStream($"tmp_{batchNo}.txt", FileMode.Create, FileAccess.Write, FileShare.None);
        foreach (var kvp in dict.OrderBy(x => x.Value, StringLine.Comparer))
        {
            await writeStream.WriteAsync(Encoding.UTF8.GetBytes(kvp.Key));
        }

        batchNo++;
        batchSize = 0;
        dict.Clear();
    }
}

sw.Stop();
Console.WriteLine("Elapsed: " + sw.Elapsed.TotalSeconds);