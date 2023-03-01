using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Commands;
using Files.App.Contexts;
using System.Threading.Tasks;

namespace Files.App.Actions
{
	internal abstract class LayoutAction : ObservableObject, IAction
	{
		public abstract string Label { get; }

		public virtual RichGlyph Glyph { get; } = RichGlyph.None;
		public virtual HotKey HotKey { get; } = HotKey.None;

		public virtual bool IsOn => false;
		public virtual bool IsExecutable => true;

		protected IContentPageContext Context { get; } = Ioc.Default.GetRequiredService<IContentPageContext>();

		public Task ExecuteAsync()
		{
			Execute();
			return Task.CompletedTask;
		}

		protected abstract void Execute();

		protected void SetLayout(ContentLayoutTypes layoutType)
		{
			var viewModel = Context.ShellPage?.InstanceViewModel?.FolderSettings;
			if (viewModel is null)
				return;

			switch (layoutType)
			{
				case ContentLayoutTypes.Details:
					viewModel.ToggleLayoutModeDetailsView(true);
					break;
				case ContentLayoutTypes.Tiles:
					viewModel.ToggleLayoutModeTiles(true);
					break;
				case ContentLayoutTypes.GridSmall:
					viewModel.ToggleLayoutModeGridViewSmall(true);
					break;
				case ContentLayoutTypes.GridMedium:
					viewModel.ToggleLayoutModeGridViewMedium(true);
					break;
				case ContentLayoutTypes.GridLarge:
					viewModel.ToggleLayoutModeGridViewLarge(true);
					break;
				case ContentLayoutTypes.Columns:
					viewModel.ToggleLayoutModeColumnView(true);
					break;
				case ContentLayoutTypes.Adaptive:
					viewModel.ToggleLayoutModeAdaptive();
					break;
			}
		}
	}
}
