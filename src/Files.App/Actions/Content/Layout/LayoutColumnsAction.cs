using Files.App.Commands;
using Files.App.Contexts;
using Files.App.Extensions;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutColumnsAction : ToggleLayoutAction
	{
		protected override ContentLayoutTypes LayoutType => ContentLayoutTypes.Columns;

		public override string Label { get; } = "Details".GetLocalizedResource();

		public override RichGlyph Glyph { get; } = new(opacityStyle: "ColorIconColumnsLayout");
		public override HotKey HotKey { get; } = new(VirtualKey.Number6, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);
	}
}
