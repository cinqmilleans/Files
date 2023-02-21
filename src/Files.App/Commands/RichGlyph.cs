namespace Files.App.Commands
{
	public readonly struct RichGlyph
	{
		public static RichGlyph None { get; } = new(string.Empty);

		public bool IsNone => string.IsNullOrEmpty(BaseGlyph);

		public string BaseGlyph { get; }
		public string OverlayGlyph { get; }
		public string FontFamily { get; }

		public RichGlyph(string baseGlyph, string overlayGlyph = "", string fontFamily = "")
			=> (BaseGlyph, OverlayGlyph, FontFamily) = (baseGlyph, overlayGlyph, fontFamily);

		public void Deconstruct(out string baseGlyph, out string overlayGlyph, out string fontFamily)
			=> (baseGlyph, overlayGlyph, fontFamily) = (BaseGlyph, OverlayGlyph, FontFamily);
	}
}
