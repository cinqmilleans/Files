using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Files.App.UserControls.Menus
{
	public sealed partial class CompressDialog : ContentDialog
	{
		public new Task<ContentDialogResult> ShowAsync() => SetContentDialogRoot(this).ShowAsync().AsTask();

		public CompressViewModel ViewModel { get; } = new();

		public CompressDialog()
		{
			InitializeComponent();
		}

		private int FormatIndex { get; set; } = 0;

		private IList<Format> Formats { get; } = new List<Format>
		{
			new Format(".zip", "Un format qui est vraiment très très bien."),
			new Format(".7z", "Un autre format qui est bien mais qu'il faut installer."),
		};

		private IList<Format> Levels { get; } = new List<Format>
		{
			new Format("High", "Le plus lent"),
			new Format("Low", "Le plus rapide"),
		};


		// WINUI3
		private ContentDialog SetContentDialogRoot(ContentDialog contentDialog)
		{
			if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
			{
				contentDialog.XamlRoot = App.Window.Content.XamlRoot;
			}
			return contentDialog;
		}

		private void Button_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			ViewModel.IsVisible = !ViewModel.IsVisible;
		}
	}

	public record Format(string Label, string Description = "");

	public class CompressViewModel : ObservableObject
	{
		public bool isVisible = false;
		public bool IsVisible
		{
			get => isVisible;
			set => SetProperty(ref isVisible, value);
		}
	}
}
