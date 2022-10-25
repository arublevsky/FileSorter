namespace FileSorter;

public class StringLine
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
        public int Compare(StringLine? x, StringLine? y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (ReferenceEquals(null, y))
            {
                return 1;
            }

            if (ReferenceEquals(null, x))
            {
                return -1;
            }
           
            var stringComparison = string.Compare(x.Text, y.Text, StringComparison.Ordinal);
            return stringComparison != 0 ? stringComparison : x.Number.CompareTo(y.Number);
        }
    }
}