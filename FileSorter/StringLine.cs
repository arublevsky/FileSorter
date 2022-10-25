namespace FileSorter;

public readonly struct StringLine
{
    public static IComparer<StringLine> Comparer { get; } = new StringLineComparer();

    public StringLine(string line)
    {
        var parts = line.Split(". ");
        Text = parts[1];
        Number = int.Parse(parts[0]);
    }

    private int Number { get; }
    private string Text { get; }

    public override string ToString()
    {
        return $"{Number}. {Text}";
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