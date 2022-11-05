using Files.App.Keyboard;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using Windows.System;

namespace Files.App.Short
{
	/*internal interface IShortKeyAction
	{
		event EventHandler ShortKeyEvent;

		string Label { get; }
		string Description { get; }

		ShortKey ShortKey { get; }
		ShortKey DefaultShortKey { get; }

		void Execute();
	}



	internal class HelpKeyAction : IShortKeyAction
	{
		public string Label => "Aide";
		public string Description => "Ouvre l'aide";

		public ShortKey ShortKey => "F1";
		public ShortKey DefaultShortKey => "F1";

		public async void Execute()
		{
			var url = new Uri(Constants.GitHub.DocumentationUrl);
			await Launcher.LaunchUriAsync(url);
		}
	}

	internal class Actions
	{
		private readonly IList<KeyboardAccelerator> accelerators;
		private readonly IList<IShortKeyAction> actions;

		public Actions(IList<KeyboardAccelerator> accelerators, IList<IShortKeyAction> actions)
		{
			this.accelerators = accelerators;
			this.actions = actions;

			var accelerator = new KeyboardAccelerator
			{
				IsEnabled = true,
				Key = action.ShortKey.Key,
				Modifiers = action.ShortKey.Modifiers,
			};
			accelerator.Invoked += Accelerator_Invoked;
			this.accelerators.Add(accelerator);
		}

		private void Accelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
		{
			//args.KeyboardAccelerator.
		}


	}*/
}
