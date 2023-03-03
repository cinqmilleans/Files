using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Commands;
using Files.App.Contexts;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Files.App.Actions
{
	internal abstract class ToggleLayoutAction : ObservableObject, IToggleAction
	{
		private readonly IDisplayPageContext context = Ioc.Default.GetRequiredService<IDisplayPageContext>();

		protected abstract LayoutTypes LayoutType { get; }

		public abstract string Label { get; }

		public abstract RichGlyph Glyph { get; }
		public abstract HotKey HotKey { get; }

		private bool isOn;
		public bool IsOn => isOn;

		public ToggleLayoutAction()
		{
			isOn = context.LayoutType == LayoutType;
			context.PropertyChanged += Context_PropertyChanged;
		}

		public Task ExecuteAsync()
		{
			context.LayoutType = LayoutType;
			return Task.CompletedTask;
		}

		private void Context_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(IDisplayPageContext.LayoutType))
				SetProperty(ref isOn, context.LayoutType == LayoutType, nameof(IsOn));
		}
	}
}
