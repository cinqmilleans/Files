using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Contexts;
using Files.App.Extensions;
using Files.Shared.Enums;
using System.Threading.Tasks;

namespace Files.App.Actions
{
	internal class ToggleSortDirectionAction : IAction
	{
		protected IDisplayPageContext Context { get; } = Ioc.Default.GetRequiredService<IDisplayPageContext>();

		public string Label { get; } = "ToggleSortDirection".GetLocalizedResource();

		public Task ExecuteAsync()
		{
			Context.SortDirection = Context.SortDirection is SortDirection.Descending ? SortDirection.Ascending : SortDirection.Descending;
			return Task.CompletedTask;
		}
	}
}
