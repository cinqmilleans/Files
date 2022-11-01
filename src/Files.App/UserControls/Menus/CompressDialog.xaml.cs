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

		private int FormatIndex { get; set; } = 2;

		private IList<Format> Formats { get; } = new List<Format>
		{
			new Format(".zip", "Un format qui est vraiment très très bien."),
			new Format(".7z", "Un autre format qui est bien mais qu'il faut installer."),
		};

		private IList<Format> Levels { get; } = new List<Format>
		{
			new Format("Ultra", "The slowest"),
			new Format("Maximum", "The slowest"),
			new Format("Normal", "The slowest"),
			new Format("Fast", "The slowest"),
			new Format("Ultra", "The slowest"),
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

		private void HyperlinkButton_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			ViewModel.IsOption = !ViewModel.IsOption;
		}
	}

	public record Format(string Label, string Description = "");

	public class CompressViewModel : ObservableObject
	{
		public bool IsMore => !isOption && !isVisible;
		public bool IsFewer => isOption && !isVisible;

		public bool isOption = false;
		public bool IsOption
		{
			get => isOption;
			set
			{
				SetProperty(ref isOption, value);
				OnPropertyChanged(nameof(IsMore));
				OnPropertyChanged(nameof(IsFewer));
			}
		}

		public bool isVisible = false;
		public bool IsVisible
		{
			get => isVisible;
			set
			{
				SetProperty(ref isVisible, value);
				OnPropertyChanged(nameof(IsMore));
				OnPropertyChanged(nameof(IsFewer));
			}
		}
	}
}
