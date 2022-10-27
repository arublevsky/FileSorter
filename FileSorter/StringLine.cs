namespace FileSorter;

public readonly struct StringLine
{
    public static IComparer<StringLine> Comparer { get; } = new StringLineComparer();

    public StringLine(string line)
    {
        var (number, text) = ParseLine(line);
        Text = text;
        Number = number;
    }

    private int Number { get; }
    private string Text { get; }

    public override string ToString() => $"{Number}. {Text}";

    private static (int, string) ParseLine(string line)
    {
        var span = line.AsSpan();
        var commaIndex = line.IndexOf(".", StringComparison.Ordinal);
        var textStartIndex = commaIndex + 2;
        return (int.Parse(span[..commaIndex]), span.Slice(textStartIndex, span.Length - textStartIndex).ToString());
    }
    
    private sealed class StringLineComparer : IComparer<StringLine>
    {
        public int Compare(StringLine x, StringLine y)
        {
            var stringComparison = string.Compare(x.Text, y.Text, StringComparison.OrdinalIgnoreCase);
            return stringComparison != 0 ? stringComparison : x.Number.CompareTo(y.Number);
        }
    }
}