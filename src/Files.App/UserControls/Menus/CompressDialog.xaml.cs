using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using System;

namespace Files.App.UserControls.Menus
{
	public sealed partial class CompressDialog : ContentDialog
	{
		public CompressDialog() => InitializeComponent();

		public new Task<ContentDialogResult> ShowAsync() => SetContentDialogRoot(this).ShowAsync().AsTask();

		// WINUI3
		private ContentDialog SetContentDialogRoot(ContentDialog contentDialog)
		{
			if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
			{
				contentDialog.XamlRoot = App.Window.Content.XamlRoot;
			}
			return contentDialog;
		}

	}
}