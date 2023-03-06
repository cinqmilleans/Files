using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Contexts;
using Files.App.Extensions;
using Files.Shared.Enums;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Files.App.Actions.Content.Sort
{
	internal class SortDescendingAction : ObservableObject, IToggleAction
	{
		protected IDisplayPageContext Context { get; } = Ioc.Default.GetRequiredService<IDisplayPageContext>();

		public string Label { get; } = "Descending".GetLocalizedResource();

		public bool IsOn => Context.SortDirection is SortDirection.Descending;

		public SortDescendingAction()
		{
			Context.PropertyChanged += Context_PropertyChanged;
		}

		public Task ExecuteAsync()
		{
			Context.SortDirection = SortDirection.Descending;
			return Task.CompletedTask;
		}

		private void Context_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(IDisplayPageContext.SortDirection))
				OnPropertyChanged(nameof(IsOn));
		}
	}
}
