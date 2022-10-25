namespace FileGenerator;

public static class Generator
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
    private static readonly Random Random = new();
    
    public static int GenerateNumber() => Random.Next(100_000);

    public static string GenerateText() =>
        new(Enumerable.Repeat(Chars, Random.Next(10, 20)).Select(s => s[Random.Next(s.Length)]).ToArray());
}