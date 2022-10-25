namespace FileSorter;

public class StringLine
{
    public static IComparer<StringLine> Comparer { get; } = new StringLineComparer();

    public StringLine(int number, string text)
    {
        Number = number;
        Text = text;
    }

    private int Number { get; set; }
    private string Text { get; set; }

    public override string ToString()
    {
        return $"{Number}. {Text}{Environment.NewLine}";
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