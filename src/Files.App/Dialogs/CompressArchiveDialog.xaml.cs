using CommunityToolkit.Mvvm.Input;
using Files.App.Extensions;
using Files.App.Filesystem.Archive;
using Files.Backend.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation.Metadata;

namespace Files.App.Dialogs
{
	public sealed partial class CompressArchiveDialog : ContentDialog
	{
		public static readonly DependencyProperty FileNameProperty = DependencyProperty
			.Register(nameof(FileName), typeof(string), typeof(CompressArchiveDialog), new(string.Empty));

		public static readonly DependencyProperty PasswordProperty = DependencyProperty
			.Register(nameof(Password), typeof(string), typeof(CompressArchiveDialog), new(string.Empty));

		public static readonly DependencyProperty FileFormatProperty = DependencyProperty
			.Register(nameof(FileFormat), typeof(ArchiveFormats), typeof(CompressArchiveDialog), new(ArchiveFormats.Zip));

		public static readonly DependencyProperty CompressionLevelProperty = DependencyProperty
			.Register(nameof(CompressionLevel), typeof(ArchiveCompressionLevels), typeof(CompressArchiveDialog), new(ArchiveCompressionLevels.Normal));

		public static readonly DependencyProperty SplittingSizeProperty = DependencyProperty
			.Register(nameof(SplittingSize), typeof(ArchiveSplittingSizes), typeof(CompressArchiveDialog), new(ArchiveSplittingSizes.None));

		private bool canCreate = false;
		public bool CanCreate => canCreate;

		public string FileName
		{
			get => (string)GetValue(FileNameProperty);
			set => SetValue(FileNameProperty, value);
		}
		public string Password
		{
			get => (string)GetValue(PasswordProperty);
			set => SetValue(PasswordProperty, value);
		}

		public ArchiveFormats FileFormat
		{
			get => (ArchiveFormats)GetValue(FileFormatProperty);
			set => SetValue(FileFormatProperty, (int)value);
		}
		public ArchiveCompressionLevels CompressionLevel
		{
			get => (ArchiveCompressionLevels)GetValue(CompressionLevelProperty);
			set => SetValue(CompressionLevelProperty, (int)value);
		}
		public ArchiveSplittingSizes SplittingSize
		{
			get => (ArchiveSplittingSizes)GetValue(SplittingSizeProperty);
			set => SetValue(SplittingSizeProperty, (int)value);
		}

		private ICommand CreateCommand { get; }

		private IImmutableList<FileFormatItem> FileFormats { get; } = new List<FileFormatItem>
		{
			new(ArchiveFormats.Zip, ".zip", "CompressionFormatZipDescription".GetLocalizedResource()),
			new(ArchiveFormats.SevenZip, ".7z", "CompressionFormatSevenZipDescription".GetLocalizedResource()),
		}.ToImmutableList();

		private IImmutableList<CompressionLevelItem> CompressionLevels { get; } = new List<CompressionLevelItem>
		{
			new(ArchiveCompressionLevels.Ultra, "CompressionLevelUltra".GetLocalizedResource()),
			new(ArchiveCompressionLevels.High, "CompressionLevelHigh".GetLocalizedResource()),
			new(ArchiveCompressionLevels.Normal, "CompressionLevelNormal".GetLocalizedResource()),
			new(ArchiveCompressionLevels.Low, "CompressionLevelLow".GetLocalizedResource()),
			new(ArchiveCompressionLevels.Fast, "CompressionLevelFast".GetLocalizedResource()),
			new(ArchiveCompressionLevels.None, "CompressionLevelNone".GetLocalizedResource()),
		}.ToImmutableList();

		private IImmutableList<SplittingSizeItem> SplittingSizes { get; } = new List<SplittingSizeItem>
		{
			new(ArchiveSplittingSizes.None, "Do not split".GetLocalizedResource()),
			new(ArchiveSplittingSizes.Mo10, ToSizeText(10)),
			new(ArchiveSplittingSizes.Mo100, ToSizeText(100)),
			new(ArchiveSplittingSizes.Cd650, ToSizeText(650, "CD".GetLocalizedResource())),
			new(ArchiveSplittingSizes.Cd700, ToSizeText(700, "CD".GetLocalizedResource())),
			new(ArchiveSplittingSizes.Mo1024, ToSizeText(1024)),
			new(ArchiveSplittingSizes.Fat4092, ToSizeText(4092, "FAT".GetLocalizedResource())),
			new(ArchiveSplittingSizes.Dvd4480, ToSizeText(4480, "DVD".GetLocalizedResource())),
			new(ArchiveSplittingSizes.Mo5120, ToSizeText(5120)),
			new(ArchiveSplittingSizes.Dvd8128, ToSizeText(8128, "DVD".GetLocalizedResource())),
			new(ArchiveSplittingSizes.Bd23040, ToSizeText(23040, "Bluray".GetLocalizedResource())),
		}.ToImmutableList();

		public CompressArchiveDialog()
		{
			InitializeComponent();
			CreateCommand = new RelayCommand(() => canCreate = true);
		}

		public new Task<ContentDialogResult> ShowAsync() => SetContentDialogRoot(this).ShowAsync().AsTask();

		private static ContentDialog SetContentDialogRoot(ContentDialog contentDialog)
		{
			if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
				contentDialog.XamlRoot = App.Window.Content.XamlRoot; // WinUi3
			return contentDialog;
		}

		private static string ToSizeText(ulong size) => ByteSize.FromMebiBytes(size).ShortString;
		private static string ToSizeText(ulong size, string labelKey) => $"{ToSizeText(size)} - {labelKey}";

		private void FileNameBox_Loading(FrameworkElement _, object args)
		{
			FileNameBox.Loading -= FileNameBox_Loading;
			FileNameBox.SelectionStart = FileNameBox.Text.Length;
		}
		private void FileFormatSelector_Loading(FrameworkElement _, object args)
		{
			FileFormatSelector.Loading -= FileFormatSelector_Loading;
			FileFormatSelector.SelectedItem = FileFormats.First(format => format.Key == FileFormat);
		}
		private void CompressionLevelSelector_Loading(FrameworkElement _, object e)
		{
			CompressionLevelSelector.Loading -= CompressionLevelSelector_Loading;
			CompressionLevelSelector.SelectedItem = CompressionLevels.First(level => level.Key == CompressionLevel);
		}
		private void SplittingSizeSelector_Loading(FrameworkElement _, object e)
		{
			SplittingSizeSelector.Loading -= SplittingSizeSelector_Loading;
			SplittingSizeSelector.SelectedItem = SplittingSizes.First(level => level.Key == SplittingSize);
		}

		private void FileFormatSelector_SelectionChanged(object _, SelectionChangedEventArgs e)
		{
			SplittingSizeSelector.IsEnabled = FileFormat is ArchiveFormats.SevenZip;
		}
		private void ContentDialog_Closing(ContentDialog _, ContentDialogClosingEventArgs e)
		{
			FileFormatSelector.SelectionChanged -= FileFormatSelector_SelectionChanged;
		}

		private record FileFormatItem(ArchiveFormats Key, string Label, string Description);
		private record CompressionLevelItem(ArchiveCompressionLevels Key, string Label);
		private record SplittingSizeItem(ArchiveSplittingSizes Key, string Label);
	}
}