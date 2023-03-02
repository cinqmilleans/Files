using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Contexts;
using Files.App.Extensions;
using System.Threading.Tasks;

namespace Files.App.Actions
{
	internal class LayoutNextAction : IAction
	{
		private readonly IContentPageContext context = Ioc.Default.GetRequiredService<IContentPageContext>();

		public string Label { get; } = "Next".GetLocalizedResource();

		public Task ExecuteAsync()
		{
			context.LayoutType = context.LayoutType switch
			{
				ContentLayoutTypes.Details => ContentLayoutTypes.Tiles,
				ContentLayoutTypes.Tiles => ContentLayoutTypes.GridSmall,
				ContentLayoutTypes.GridSmall => ContentLayoutTypes.GridMedium,
				ContentLayoutTypes.GridMedium => ContentLayoutTypes.GridLarge,
				ContentLayoutTypes.GridLarge => ContentLayoutTypes.Columns,
				ContentLayoutTypes.Columns => ContentLayoutTypes.Adaptive,
				ContentLayoutTypes.Adaptive => ContentLayoutTypes.Details,
				_ => ContentLayoutTypes.None,
			};

			return Task.CompletedTask;
		}
	}
}
