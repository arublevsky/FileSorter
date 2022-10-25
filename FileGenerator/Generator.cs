namespace FileGenerator;

public static class Generator
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private static readonly Random Random = new();
    
    public static int GenerateNumber() => Random.Next(1000);

    public static string GenerateText() =>
        new(Enumerable.Repeat(Chars, 10).Select(s => s[Random.Next(s.Length)]).ToArray());
}