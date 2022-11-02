using CommunityToolkit.Mvvm.ComponentModel;

namespace Files.App.ViewModels.Key
{
	internal class ShortKeyManager : ObservableObject, IItemShortKeyManager
	{
		private ShortKey selectAll = ShortKey.None;
		public ShortKey SelectAll
		{
			get => selectAll;
			set => SetProperty(ref selectAll, value);
		}
	}
}
