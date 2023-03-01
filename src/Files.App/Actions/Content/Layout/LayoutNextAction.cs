using Files.App.Commands;
using Files.App.Contexts;
using System.ComponentModel;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutNextAction : LayoutAction
	{
		public override string Label { get; } = "Next";

		public override HotKey HotKey { get; } = new HotKey(VirtualKey.Up, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		private bool isExecutable;
		public override bool IsExecutable => isExecutable;

		public LayoutNextAction()
		{
			isExecutable = GetIsExecutable();
			Context.PropertyChanged += Context_PropertyChanged;
		}

		protected override void Execute()
		{
			switch (Context.LayoutType)
			{
				case ContentLayoutTypes.Details:
					SetLayout(ContentLayoutTypes.Tiles);
					break;
				case ContentLayoutTypes.Tiles:
					SetLayout(ContentLayoutTypes.GridSmall);
					break;
				case ContentLayoutTypes.GridSmall:
					SetLayout(ContentLayoutTypes.GridMedium);
					break;
				case ContentLayoutTypes.GridMedium:
					SetLayout(ContentLayoutTypes.GridLarge);
					break;
				case ContentLayoutTypes.GridLarge:
					SetLayout(ContentLayoutTypes.Columns);
					break;
				case ContentLayoutTypes.Columns:
					SetLayout(ContentLayoutTypes.Adaptive);
					break;
			}
		}

		private bool GetIsExecutable() => Context.LayoutType is not ContentLayoutTypes.None and not ContentLayoutTypes.Adaptive;

		private void Context_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(Context.LayoutType))
				SetProperty(ref isExecutable, GetIsExecutable(), nameof(IsExecutable));
		}
	}
}
