using System;

namespace Files.Uwp.Helpers
{
    internal class TimeSpanLabel : ITimeSpanLabel
    {
        public string Text { get; init; }
        public string Range { get; init; }
        public string Glyph { get; init; }
        public int Index { get; init; }

        public TimeSpanLabel(string text, string range, string glyph, int index)
            => (Text, Range, Glyph, Index) = (text, range, glyph, index);
        public TimeSpanLabel(string text, DateTime range, string glyph, int index)
            => (Text, Range, Glyph, Index) = (text, range.ToShortDateString(), glyph, index);

        public void Deconstruct(out string text, out string range, out string glyph, out int index)
        {
            text = Text;
            range = Range;
            glyph = Glyph;
            index = Index;
        }
    }
}
