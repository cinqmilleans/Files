using Files.App.Contexts;
using System.ComponentModel;

namespace Files.App.Actions
{
	internal abstract class ToggleLayoutAction : LayoutAction, IToggleAction
	{
		protected abstract ContentLayoutTypes LayoutType { get; }

		private bool isOn;
		public override bool IsOn => isOn;

		public ToggleLayoutAction()
		{
			isOn = Context.LayoutType == LayoutType;
			Context.PropertyChanged += Context_PropertyChanged;
		}

		protected override void Execute() => SetLayout(LayoutType);

		private void Context_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(IContentPageContext.LayoutType))
				SetProperty(ref isOn, Context.LayoutType == LayoutType, nameof(IsOn));
		}
	}
}
