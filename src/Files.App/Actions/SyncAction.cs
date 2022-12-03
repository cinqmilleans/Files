using System.Threading.Tasks;

namespace Files.App.Actions
{
	public abstract class SyncAction : AsyncAction
	{
		public override Task ExecuteAsync()
		{
			Execute();
			return Task.CompletedTask;
		}

		public abstract void Execute();
	}
}
