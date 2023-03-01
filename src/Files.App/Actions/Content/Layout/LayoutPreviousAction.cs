using Files.App.Commands;
using Files.App.Contexts;
using System.ComponentModel;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutPreviousAction : LayoutAction
	{
		public override string Label { get; } = "Previous";

		public override HotKey HotKey { get; } = new HotKey(VirtualKey.Down, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		private bool isExecutable;
		public override bool IsExecutable => isExecutable;

		public LayoutPreviousAction()
		{
			isExecutable = GetIsExecutable();
			Context.PropertyChanged += Context_PropertyChanged;
		}

		protected override void Execute()
		{
			switch (Context.LayoutType)
			{
				case ContentLayoutTypes.Tiles:
					SetLayout(ContentLayoutTypes.Details);
					break;
				case ContentLayoutTypes.GridSmall:
					SetLayout(ContentLayoutTypes.Tiles);
					break;
				case ContentLayoutTypes.GridMedium:
					SetLayout(ContentLayoutTypes.GridSmall);
					break;
				case ContentLayoutTypes.GridLarge:
					SetLayout(ContentLayoutTypes.GridMedium);
					break;
				case ContentLayoutTypes.Columns:
					SetLayout(ContentLayoutTypes.GridLarge);
					break;
				case ContentLayoutTypes.Adaptive:
					SetLayout(ContentLayoutTypes.Columns);
					break;
			}
		}

		private bool GetIsExecutable() => Context.LayoutType is not ContentLayoutTypes.None and not ContentLayoutTypes.Details;

		private void Context_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(Context.LayoutType))
				SetProperty(ref isExecutable, GetIsExecutable(), nameof(IsExecutable));
		}
	}
}
