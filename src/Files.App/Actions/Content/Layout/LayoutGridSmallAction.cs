﻿using Files.App.Commands;
using Files.App.Contexts;
using Files.App.Extensions;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutGridSmallAction : ToggleLayoutAction
	{
		public override string Label { get; } = "SmallIcons".GetLocalizedResource();

		public override RichGlyph Glyph { get; } = new("\uE80A");
		public override HotKey HotKey { get; } = new(VirtualKey.Number3, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		protected override ContentLayoutTypes LayoutType => ContentLayoutTypes.GridSmall;
	}
}
