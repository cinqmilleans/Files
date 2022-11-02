using System.ComponentModel;

namespace Files.App.ViewModels.Key
{
	public interface IItemShortKeyManager : INotifyPropertyChanged
	{
		ShortKey SelectAll { get; set; }
	}
}
