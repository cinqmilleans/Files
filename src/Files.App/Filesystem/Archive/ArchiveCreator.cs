using Files.App.Helpers;
using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Files.App.Filesystem.Archive
{
	public class ArchiveCreator : IArchiveCreator
	{
		public event EventHandler<IArchiveCreator>? ProgressionUpdated;

		public string ArchiveName => Path.Combine(Directory, FileName + ArchiveExtension);

		public string Directory { get; set; } = string.Empty;
		public string FileName { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;

		public IEnumerable<string> Sources { get; set; } = Enumerable.Empty<string>();

		public ArchiveFormats FileFormat { get; set; } = ArchiveFormats.Zip;
		public ArchiveCompressionLevels CompressionLevel { get; set; } = ArchiveCompressionLevels.Normal;
		public ArchiveSplittingSizes SplittingSize { get; set; } = ArchiveSplittingSizes.None;

		public IProgress<float> Progress { get; set; } = new Progress<float>();

		private string ArchiveExtension => FileFormat switch
		{
			ArchiveFormats.Zip => ".zip",
			ArchiveFormats.SevenZip => ".7z",
			_ => throw new ArgumentOutOfRangeException(nameof(FileFormat)),
		};
		private OutArchiveFormat SevenZipArchiveFormat => FileFormat switch
		{
			ArchiveFormats.Zip => OutArchiveFormat.Zip,
			ArchiveFormats.SevenZip => OutArchiveFormat.SevenZip,
			_ => throw new ArgumentOutOfRangeException(nameof(FileFormat)),
		};
		private CompressionLevel SevenZipCompressionLevel => CompressionLevel switch
		{
			ArchiveCompressionLevels.High => SevenZip.CompressionLevel.High,
			ArchiveCompressionLevels.Ultra => SevenZip.CompressionLevel.Ultra,
			ArchiveCompressionLevels.Normal => SevenZip.CompressionLevel.Normal,
			ArchiveCompressionLevels.Low => SevenZip.CompressionLevel.Low,
			ArchiveCompressionLevels.Fast => SevenZip.CompressionLevel.Fast,
			ArchiveCompressionLevels.None => SevenZip.CompressionLevel.None,
			_ => throw new ArgumentOutOfRangeException(nameof(CompressionLevel)),
		};
		private int SevenZipVolumeSize => SplittingSize switch
		{
			ArchiveSplittingSizes.None => 0,
			ArchiveSplittingSizes.Mo10 => 10,
			ArchiveSplittingSizes.Mo100 => 100,
			ArchiveSplittingSizes.Mo1024 => 1024,
			ArchiveSplittingSizes.Mo5120 => 5120,
			ArchiveSplittingSizes.Fat4092 => 4092,
			ArchiveSplittingSizes.Cd650 => 650,
			ArchiveSplittingSizes.Cd700 => 700,
			ArchiveSplittingSizes.Dvd4480 => 4480,
			ArchiveSplittingSizes.Dvd8128 => 8128,
			ArchiveSplittingSizes.Bd23040 => 23040,
			_ => throw new ArgumentOutOfRangeException(nameof(SplittingSize)),
		};

		public async Task<bool> RunCreationAsync()
		{
			string path = ArchiveName;
			string[] sources = Sources.ToArray();
			bool hasPassword = !string.IsNullOrEmpty(Password);

			var compressor = new SevenZipCompressor
			{
				ArchiveFormat = SevenZipArchiveFormat,
				CompressionLevel = SevenZipCompressionLevel,
				VolumeSize = SevenZipVolumeSize,
				FastCompression = true,
				IncludeEmptyDirectories = true,
				EncryptHeaders = true,
				PreserveDirectoryRoot = sources.Length > 1,
				EventSynchronization = EventSynchronizationStrategy.AlwaysAsynchronous,
			};

			try
			{
				for (int i = 0; i < sources.Length; ++i)
				{
					if (i > 0)
						compressor.CompressionMode = CompressionMode.Append;

					var item = sources[i];
					if (hasPassword)
						await compressor.CompressFilesEncryptedAsync(path, Password, item);
					else if (File.Exists(item))
						await compressor.CompressFilesAsync(path, item);
					else if (System.IO.Directory.Exists(item))
						await compressor.CompressDirectoryAsync(item, path);

					float percentage = (i + 1.0f) / sources.Length * 100.0f;
					Progress.Report(percentage);
				}
				return true;
			}
			catch (Exception ex)
			{
				App.Logger.Warn(ex, $"Error compressing folder: {path}");
				NativeFileOperationsHelper.DeleteFileFromApp(path);
				return false;
			}
		}
	}
}
