using CommunityToolkit.Mvvm.ComponentModel;
using Files.App.UserControls.MultitaskingControl;
using Files.App.ViewModels;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Files.App.Contexts
{
	internal class MultitaskingContext : ObservableObject, IMultitaskingContext
	{
		private IMultitaskingControl? control;
		public IMultitaskingControl? Control => control;

		public TabItem SelectedTabItem => MainPageViewModel.AppInstances[selectedTabIndex];

		private ushort selectedTabIndex = 0;
		public ushort SelectedTabIndex => selectedTabIndex;

		private ushort tabCount = 0;
		public ushort TabCount => tabCount;

		public MultitaskingContext()
		{
			App.AppModel.PropertyChanged += AppModel_PropertyChanged;
			MainPageViewModel.AppInstances.CollectionChanged += AppInstances_CollectionChanged;
			BaseMultitaskingControl.OnLoaded += BaseMultitaskingControl_OnLoaded;
		}

		private void AppModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			UpdateSelectedTabIndex();
		}
		private void AppInstances_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateTabCount();
		}
		private void BaseMultitaskingControl_OnLoaded(object? sender, IMultitaskingControl control)
		{
			SetProperty(ref this.control, control, nameof(Control));
			UpdateSelectedTabIndex();
			UpdateTabCount();
		}

		private void UpdateSelectedTabIndex()
		{
			if (SetProperty(ref selectedTabIndex, (ushort)App.AppModel.TabStripSelectedIndex, nameof(SelectedTabIndex)))
			{
				OnPropertyChanged(nameof(SelectedTabItem));
			}
		}
		private void UpdateTabCount()
		{
			SetProperty(ref tabCount, (ushort)MainPageViewModel.AppInstances.Count, nameof(TabCount));
		}
	}
}
