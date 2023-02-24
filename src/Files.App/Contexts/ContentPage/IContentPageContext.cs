using Files.App.Filesystem;
using System.Collections.Generic;
using System.ComponentModel;

namespace Files.App.Contexts
{
	public interface IContentPageContext : INotifyPropertyChanged
	{
		IShellPage ShellPage { get; }

		ContentPageTypes PageType { get; }

		string Folder { get; }

		bool HasItem { get; }
		int ItemCount { get; }

		IReadOnlyList<ListedItem> SelectedItems { get; }

		void SelectAll();
		void ClearSelection();
		void InvertSelection();

		void StartRename();
	}
}
