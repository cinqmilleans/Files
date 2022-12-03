using System.Threading.Tasks;

namespace Files.App.Actions
{
	public abstract class SyncAction : AsyncAction
	{
		public override Task ExecuteAsync(IActionContext context)
		{
			Execute(context);
			return Task.CompletedTask;
		}

		public abstract void Execute(IActionContext context);
	}
}
