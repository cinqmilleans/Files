using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Contexts;
using Files.Shared.Enums;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Files.App.Actions
{
	internal abstract class SortByAction : ObservableObject, IToggleAction
	{
		protected IContentPageContext ContentContext { get; } = Ioc.Default.GetRequiredService<IContentPageContext>();
		protected IDisplayPageContext DisplayContext { get; } = Ioc.Default.GetRequiredService<IDisplayPageContext>();

		protected abstract SortOption SortOption { get; }

		public abstract string Label { get; }

		private bool isOn;
		public bool IsOn => isOn;

		private bool isExecutable = false;
		public bool IsExecutable => isExecutable;

		public SortByAction()
		{
			isOn = DisplayContext.SortOption == SortOption;
			isExecutable = GetIsExecutable(ContentContext.PageType);

			ContentContext.PropertyChanged += ContentContext_PropertyChanged;
			DisplayContext.PropertyChanged += DisplayContext_PropertyChanged;
		}

		public Task ExecuteAsync()
		{
			DisplayContext.SortOption = SortOption;
			return Task.CompletedTask;
		}

		protected virtual bool GetIsExecutable(ContentPageTypes pageType) => true;

		private void ContentContext_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(IContentPageContext.PageType))
				SetProperty(ref isExecutable, GetIsExecutable(ContentContext.PageType), nameof(IsExecutable));
		}

		private void DisplayContext_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(IDisplayPageContext.SortOption))
				SetProperty(ref isOn, DisplayContext.SortOption == SortOption, nameof(IsOn));
		}
	}
}
