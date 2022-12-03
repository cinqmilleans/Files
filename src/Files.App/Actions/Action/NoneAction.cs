using Files.App.Actions.HotKeys;
using System.Threading.Tasks;

namespace Files.App.Actions.Action
{
	internal class NoneAction : IAction
	{
		public ActionCodes Code => ActionCodes.None;
		public string Label => string.Empty;

		public IGlyph Glyph => Actions.Glyph.None;
		public HotKey HotKey => HotKey.None;

		public bool CanExecute(IActionContext _) => true;
		public Task ExecuteAsync(IActionContext _) => Task.CompletedTask;
	}
}
