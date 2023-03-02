using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Contexts;
using Files.App.Extensions;
using System.Threading.Tasks;

namespace Files.App.Actions
{
	internal class LayoutPreviousAction : IAction
	{
		private readonly IContentPageContext context = Ioc.Default.GetRequiredService<IContentPageContext>();

		public string Label { get; } = "Previous".GetLocalizedResource();

		public Task ExecuteAsync()
		{
			context.LayoutType = context.LayoutType switch
			{
				ContentLayoutTypes.Details => ContentLayoutTypes.Adaptive,
				ContentLayoutTypes.Tiles =>  ContentLayoutTypes.Details,
				ContentLayoutTypes.GridSmall => ContentLayoutTypes.Tiles,
				ContentLayoutTypes.GridMedium => ContentLayoutTypes.GridSmall,
				ContentLayoutTypes.GridLarge => ContentLayoutTypes.GridMedium,
				ContentLayoutTypes.Columns => ContentLayoutTypes.GridLarge,
				ContentLayoutTypes.Adaptive => ContentLayoutTypes.Columns,
				_ => ContentLayoutTypes.None,
			};

			return Task.CompletedTask;
		}
	}
}
