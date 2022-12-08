using Microsoft.UI.Xaml.Media;

namespace Files.App.Commands
{
	public class Glyph : IGlyph
	{
		public static Glyph None { get; } = new();

		public string Base { get; init; } = string.Empty;
		public string Overlay { get; init; } = string.Empty;
		public string Family { get; init; } = string.Empty;

		public Glyph() {}
		public Glyph(string glyphBase) : this() => Base = glyphBase;
		public Glyph(string glyphBase, string glyphOverlay) : this(glyphBase) => Overlay = glyphOverlay;
	}
}
