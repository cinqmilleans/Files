using System.Threading.Tasks;

namespace Files.App.Actions
{
	internal class NoneAction : IAction
	{
		public ActionCodes Code => ActionCodes.None;
		public string Label => string.Empty;

		public Task ExecuteAsync() => Task.CompletedTask;
	}
}
