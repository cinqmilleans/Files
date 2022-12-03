using Files.App.Actions.HotKeys;
using Microsoft.UI.Windowing;
using Windows.System;

namespace Files.App.Actions.Action
{
	internal class FullScreenAction : SyncAction
	{
		public override ActionCodes Code => ActionCodes.FullScreen;
		public override string Label => "FullScreen";

		public override HotKey HotKey => new(VirtualKey.F11);

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
