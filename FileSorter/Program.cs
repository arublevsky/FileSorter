using System.Diagnostics;
using System.Text;
using FileSorter;

var fileName = args[0];
int memoryCapacityInBytes = int.Parse(args[1]);
var filePath = Path.Combine("C:/temp", fileName);

var sw = new Stopwatch();
sw.Start();
await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
using BufferedStream bs = new BufferedStream(fs);
var reader = new StreamReader(bs, Encoding.UTF8, true, 4096 * 2);

var list = new List<StringLine>(500_000);

var batchSize = 0;
var batchNo = 0;
Console.WriteLine("Stating...");
while (reader.ReadLine() is { } line)
{
    list.Add(new StringLine(line));
    batchSize += Encoding.UTF8.GetByteCount(line);
    if (batchSize > memoryCapacityInBytes)
    {
        Console.WriteLine($"New batch completed. Starting sorting: {batchNo}: " + sw.Elapsed.TotalSeconds);
        
        var sb = new StringBuilder();
        list.Sort(StringLine.Comparer);
        Console.WriteLine($"Sort completed. Starting writing to SB: {batchNo}: " + sw.Elapsed.TotalSeconds);
        foreach (var kvp in list)
        {
            sb.AppendLine(kvp.ToString());
        }
        
        Console.WriteLine($"Writing to SB completed. Starting writing to file: {batchNo}: " + sw.Elapsed.TotalSeconds);
        await using var writeStream = new FileStream($"tmp_{batchNo}.txt", FileMode.Create, FileAccess.Write, FileShare.None);
        writeStream.Write(Encoding.UTF8.GetBytes(sb.ToString()));
        batchNo++;
        batchSize = 0;
        list.Clear();
        Console.WriteLine($"Writing completed for batch {batchNo}: " + sw.Elapsed.TotalSeconds);
    }
}

sw.Stop();
Console.WriteLine("Elapsed: " + sw.Elapsed.TotalSeconds);