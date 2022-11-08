using Files.Backend.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace Files.App.UserControls
{
	public sealed partial class ArchiveSetup : ContentDialog
	{
		public static readonly DependencyProperty FileNameProperty = DependencyProperty
			.Register(nameof(FileName), typeof(string), typeof(ArchiveSetup), new(string.Empty));

		public static readonly DependencyProperty FormatProperty = DependencyProperty
			.Register(nameof(Format), typeof(ArchiveFormat), typeof(ArchiveSetup), new(ArchiveFormat.Zip));

		public static readonly DependencyProperty LevelProperty = DependencyProperty
			.Register(nameof(CompressionLevel), typeof(ArchiveCompressionLevel), typeof(ArchiveSetup), new(ArchiveCompressionLevel.Normal));

		public string FileName
		{
			get => (string)GetValue(FileNameProperty);
			set => SetValue(FileNameProperty, value);
		}

		public ArchiveFormat Format
		{
			get => (ArchiveFormat)GetValue(FormatProperty);
			set
			{
				SetValue(FormatProperty, (ushort)value);

				if (FormatLabel is not null)
					FormatLabel.Text = Formats.First(format => format.Key == Format).Label;
			}
		}

		public ArchiveCompressionLevel CompressionLevel
		{
			get => (ArchiveCompressionLevel)GetValue(LevelProperty);
			set => SetValue(LevelProperty, (ushort)value);
		}

		private IList<FormatItem> Formats { get; } = new List<FormatItem>
		{
			new(ArchiveFormat.Zip, ".zip", "Works natively with Windows."),
			new(ArchiveFormat.SevenZip, ".7z", "Smaller archives but requires compatible software."),
		};

		private IList<CompressionLevelItem> CompressionLevels { get; } = new List<CompressionLevelItem>
		{
			new(ArchiveCompressionLevel.Ultra, "Ultra"),
			new(ArchiveCompressionLevel.High, "High"),
			new(ArchiveCompressionLevel.Normal, "Normal"),
			new(ArchiveCompressionLevel.Low, "Low"),
			new(ArchiveCompressionLevel.Fast, "Fast"),
			new(ArchiveCompressionLevel.None, "None"),
		};

		public ArchiveSetup() => InitializeComponent();

		public new Task<ContentDialogResult> ShowAsync() => SetContentDialogRoot(this).ShowAsync().AsTask();

		private static ContentDialog SetContentDialogRoot(ContentDialog contentDialog)
		{
			if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
				contentDialog.XamlRoot = App.Window.Content.XamlRoot; // WinUi3
			return contentDialog;
		}

		private void FormatButton_Tapped(object _, TappedRoutedEventArgs e) => FormatPopup.IsOpen = true;

		private void FormatLabel_Loading(FrameworkElement _, object e)
			=> FormatLabel.Text = Formats.First(format => format.Key == Format).Label;
		private void FormatSelector_Loading(FrameworkElement sender, object args)
			=> FormatSelector.SelectedItem = Formats.First(format => format.Key == Format);
		private void CompressionLevelSelector_Loading(FrameworkElement _, object e)
			=> CompressionLevelSelector.SelectedItem = CompressionLevels.First(level => level.Key == CompressionLevel);

		private record FormatItem(ArchiveFormat Key, string Label, string Description);
		private record CompressionLevelItem(ArchiveCompressionLevel Key, string Label);
	}
}
