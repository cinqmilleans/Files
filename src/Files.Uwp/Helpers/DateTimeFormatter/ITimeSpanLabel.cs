namespace Files.Uwp.Helpers
{
    public interface ITimeSpanLabel
    {
        string Text { get; }
        string Range { get; }
        string Glyph { get; }
        int Index { get; }
    }
}
