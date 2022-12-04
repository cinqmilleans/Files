using Files.App.CommandManager;
using Microsoft.UI.Windowing;
using Windows.System;

namespace Files.App.Commands
{
	internal class FullScreenCommand : ToggleCommand
	{
		public override CommandCodes Code => CommandCodes.FullScreen;

		public override HotKey HotKey => new(VirtualKey.F11);

		public override bool IsOn
		{
			get
			{
				var window = App.GetAppWindow(App.Window);
				return window.Presenter.Kind is AppWindowPresenterKind.FullScreen;
			}
			set
			{
				var window = App.GetAppWindow(App.Window);
				var kind = value ? AppWindowPresenterKind.FullScreen : AppWindowPresenterKind.Overlapped;

				if (kind == window.Presenter.Kind)
					return;

				window.SetPresenter(kind);
				OnPropertyChanged();
				OnPropertyChanged(nameof(Label));
			}
		}

		protected override string OnLabel => "Full Screen";
		protected override string OffLabel => "Full Screen";

		public FullScreenCommand(ICommandContext context) : base(context) {}
	}
}
