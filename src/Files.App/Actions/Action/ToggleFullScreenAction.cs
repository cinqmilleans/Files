using Files.App.Actions.HotKeys;
using Microsoft.UI.Windowing;
using Windows.System;

namespace Files.App.Actions.Action
{
	internal class ToggleFullScreenAction : SyncAction
	{
		public override ActionCodes Code => ActionCodes.ToggleFullScreen;
		public override string Label => "ToggleFullScreen";

		public override HotKey HotKey => new(VirtualKey.F11);

		public override void Execute()
		{
			var view = App.GetAppWindow(App.Window);

			var kind = view.Presenter.Kind is AppWindowPresenterKind.FullScreen
				? AppWindowPresenterKind.Overlapped
				: AppWindowPresenterKind.FullScreen;

			view.SetPresenter(kind);
		}
	}
}
