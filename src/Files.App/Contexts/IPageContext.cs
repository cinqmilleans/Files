using Files.App.Filesystem;
using System.Collections.Generic;
using System.ComponentModel;

namespace Files.App.Contexts
{
	public interface IPageContext : INotifyPropertyChanged
	{
		PageTypes PageType { get; }

		bool CanCopy { get; }
		bool CanRefresh { get; }

		IReadOnlyList<ListedItem> SelectedItems { get; }
	}
}
