using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.Actions
{
	public class ActionViewModel : IActionViewModel
	{
		private readonly IAction action;

		public string Label => action.Label;
		public HotKey HotKey => action.HotKey;

		public string Glyph => action.Glyph;
		public string GlyphOverlay => action.GlyphOverlay;
		public string GlyphFamily => action.GlyphFamily;

		public ICommand Command { get; }

		public ActionViewModel(IAction action)
		{
			this.action = action;
			Command = new AsyncRelayCommand(action.ExecuteAsync, action.CanExecute);
		}

		public bool CanExecute() => action.CanExecute();
		public async Task ExecuteAsync() => await action.ExecuteAsync();
	}
}
