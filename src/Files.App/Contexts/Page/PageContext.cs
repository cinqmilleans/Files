using Files.App.UserControls.MultitaskingControl;
using Files.App.Views;
using System;

namespace Files.App.Contexts
{
	internal class PageContext : IPageContext
	{
		public event EventHandler? Changing;
		public event EventHandler? Changed;

		private PaneHolderPage? page;

		private IShellPage? pane;
		public IShellPage? Pane => pane;

		private IShellPage? paneOrColumn;
		public IShellPage? PaneOrColumn => paneOrColumn;

		public PageContext()
		{
			PaneHolderPage.CurrentInstanceChanged += Page_CurrentInstanceChanged;
		}

		private void Page_CurrentInstanceChanged(object? sender, PaneHolderPage? modifiedPage)
		{
			if (page is not null && !page.IsCurrentInstance)
				UpdatePage(null);
			else if (modifiedPage is not null && modifiedPage.IsCurrentInstance)
				UpdatePage(modifiedPage);
		}

		private void Page_ContentChanged(object? sender, TabItemArguments e)
		{
			UpdateContent();
		}

		private void UpdatePage(PaneHolderPage? newPage)
		{
			if (Equals(page, newPage))
				return;

			if (page is not null)
			{
				page.ContentChanged -= Page_ContentChanged;
			}

			page = newPage;

			if (page is not null)
			{
				page.ContentChanged += Page_ContentChanged;
			}

			UpdateContent();
		}

		private void UpdateContent()
		{
			Changing?.Invoke(this, EventArgs.Empty);

			pane = page?.ActivePane;
			paneOrColumn = page?.ActivePaneOrColumn;

			Changed?.Invoke(this, EventArgs.Empty);
		}
	}
}
