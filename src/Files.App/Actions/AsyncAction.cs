using Files.App.Actions.HotKeys;
using System.Threading.Tasks;

namespace Files.App.Actions
{
	public abstract class AsyncAction : IAction
	{
		public abstract ActionCodes Code { get; }
		public abstract string Label { get; }

		public virtual IGlyph Glyph => Actions.Glyph.None;
		public virtual HotKey HotKey => HotKey.None;

		public virtual bool CanExecute() => true;
		public abstract Task ExecuteAsync();
	}
}
