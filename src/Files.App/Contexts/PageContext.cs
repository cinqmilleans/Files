using CommunityToolkit.Mvvm.ComponentModel;
using Files.App.UserControls.MultitaskingControl;
using Files.App.Views;

namespace Files.App.Contexts
{
	internal abstract class PageContext : ObservableObject
	{
		private BaseShellPage? page;
		public BaseShellPage? Page => page!;

		public PageContext()
		{
			BaseShellPage.CurrentInstanceChanged += BaseShellPage_CurrentInstanceChanged;
		}

		protected virtual void OnPageChanging() {}
		protected virtual void OnPageChanged() {}
		protected virtual void OnContentChanged() {}

		private void BaseShellPage_CurrentInstanceChanged(object? sender, BaseShellPage? newPage)
		{
			if (Equals(page, newPage))
				return;

			if (page is not null)
			{
				page.ContentChanged -= Page_ContentChanged;
				OnPageChanging();
			}

			page = newPage;

			if (page is not null)
			{
				page.ContentChanged += Page_ContentChanged;
				OnPageChanged();
			}

			OnContentChanged();
		}

		private void Page_ContentChanged(object? sender, TabItemArguments e) => OnContentChanged();
	}
}
