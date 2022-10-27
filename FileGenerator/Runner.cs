using System.Text;

namespace FileGenerator;

public static class Runner
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
    private static readonly Random Random = new();

    public static void Run(string outputFilePath, int sizeInBytes)
    {
        using var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

        var i = 0;
        var textToDuplicate = string.Empty;

        while (fs.Length < sizeInBytes)
        {
            i++;
            var number = GenerateNumber();
            var text = GenerateText();
            WriteLine(fs, number, text);

            if (i % 10 == 0 && textToDuplicate == null)
            {
                textToDuplicate = text;
                continue;
            }

            if (i % 5 == 0 && textToDuplicate != null)
            {
                WriteLine(fs, number, textToDuplicate);
                textToDuplicate = null;
            }
        }
    }
    
    private static int GenerateNumber() => Random.Next(100_000);

    private static string GenerateText() =>
        new(Enumerable.Repeat(Chars, Random.Next(10, 20)).Select(s => s[Random.Next(s.Length)]).ToArray());
    
    private static void WriteLine(Stream stream, int number, string text) =>
        stream.Write(Encoding.UTF8.GetBytes($"{number}. {text}{Environment.NewLine}"));
}