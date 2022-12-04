using Files.App.Actions.HotKeys;
using Microsoft.UI.Windowing;
using Windows.ApplicationModel.VoiceCommands;
using Windows.System;

namespace Files.App.Actions.Action
{
	internal class ToggleFullScreenAction : SyncAction
	{
		public override ActionCodes Code => ActionCodes.FullScreen;
		public override string Label => "FullScreen";

		public override HotKey HotKey => new(VirtualKey.F11);

		public bool IsFullScreen
		{
			get => App.GetAppWindow(App.Window).Presenter.Kind is AppWindowPresenterKind.FullScreen;
			set
			{
				var kind = value ? AppWindowPresenterKind.FullScreen : AppWindowPresenterKind.Overlapped;
				App.GetAppWindow(App.Window).SetPresenter(kind);
			}
		}

		public override void Execute(IActionContext _)
		{
			var view = App.GetAppWindow(App.Window);

			var kind = view.Presenter.Kind is AppWindowPresenterKind.FullScreen
				? AppWindowPresenterKind.Overlapped
				: AppWindowPresenterKind.FullScreen;

			view.SetPresenter(kind);
		}
	}
}
