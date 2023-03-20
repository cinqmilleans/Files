using Files.App.UserControls.MultitaskingControl;
using System.ComponentModel;

namespace Files.App.Contexts
{
	public interface IMultitaskingContext : INotifyPropertyChanged
	{
		IMultitaskingControl? Control { get; }

		TabItem SelectedTabItem { get; }
		ushort SelectedTabIndex { get; }
		ushort TabCount { get; }

	}
}
