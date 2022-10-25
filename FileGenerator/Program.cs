using System.Text;
using static FileGenerator.Generator;

var fileName = args[0];
var sizeInBytes = int.Parse(args[1]);
var enableDuplicates = bool.Parse(args[2]);
var filePath = Path.Combine("C:/temp", fileName);

await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

var i = 0;
var textToDuplicate = string.Empty;
var numberToDuplicate = 0;

while (fs.Length < sizeInBytes)
{
    i++;
    var number = GenerateNumber();
    var text = GenerateText();
    await WriteLineAsync(fs, number, text);

    if (!enableDuplicates)
    {
        continue;
    }

    if (i % 10 == 0 && string.IsNullOrEmpty(textToDuplicate))
    {
        textToDuplicate = text;
        numberToDuplicate = number;
        continue;
    }

    if (i % 5 == 0 && !string.IsNullOrEmpty(textToDuplicate))
    {
        await WriteLineAsync(fs, number, textToDuplicate);
        textToDuplicate = string.Empty;
        continue;
    }
    
    if (i % 4 == 0 && numberToDuplicate > 0)
    {
        await WriteLineAsync(fs, numberToDuplicate, text);
        numberToDuplicate = 0;
    }
}

ValueTask WriteLineAsync(Stream stream, int number, string text) =>
    stream.WriteAsync(Encoding.UTF8.GetBytes($"{number}. {text}{Environment.NewLine}"));
