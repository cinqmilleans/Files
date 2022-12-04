using System.Threading.Tasks;

namespace Files.App.CommandManager
{
	public abstract class ToggleCommand : ObservableCommand, IToggleCommand
	{
		public override string Label => IsOn ? OnLabel : OffLabel;

		protected abstract string OnLabel { get; }
		protected abstract string OffLabel { get; }

		private bool isOn = false;
		public virtual bool IsOn
		{
			get => isOn;
			set
			{
				if (SetProperty(ref isOn, value))
					OnPropertyChanged(nameof(Label));
			}
		}

		public ToggleCommand(ICommandContext context) : base(context) {}

		protected override Task ExecuteAsync()
		{
			IsOn = !IsOn;
			return Task.CompletedTask;
		}
	}
}
