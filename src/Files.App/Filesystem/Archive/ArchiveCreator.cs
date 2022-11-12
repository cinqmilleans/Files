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

		private ArchiveCreatorStatus status = ArchiveCreatorStatus.Initial;
		public ArchiveCreatorStatus Status => status;

		private float percent = 0;
		public float Percent => percent;

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

		public void RunCreationAsync()
		{
			status = ArchiveCreatorStatus.Running;
			percent = 0;
			UpdateProgression();

			var path = ArchiveName;
			var sources = Sources.ToArray();

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
				compressor.Compressing += Compressor_Compressing;
				compressor.CompressionFinished += Compressor_CompressionFinished;

				using var stream = new FileStream(path, FileMode.CreateNew);
				{
					if (string.IsNullOrEmpty(Password))
						compressor.BeginCompressFiles(stream, sources);
					else
						compressor.BeginCompressFilesEncrypted(path, Password, sources);
				}

			}
			catch (Exception ex)
			{
				App.Logger.Warn(ex, $"Error compressing folder: {path}");
				NativeFileOperationsHelper.DeleteFileFromApp(path);

				compressor.Compressing -= Compressor_Compressing;
				compressor.CompressionFinished -= Compressor_CompressionFinished;

				status = ArchiveCreatorStatus.Failed;
				percent = 0;
				UpdateProgression();
			}
		}

		private void Compressor_Compressing(object? sender, ProgressEventArgs e)
		{
			percent = e.PercentDone;
			UpdateProgression();
		}
		private void Compressor_CompressionFinished(object? sender, EventArgs e)
		{
			var compressor = (sender as SevenZipCompressor)!;
			compressor.Compressing -= Compressor_Compressing;
			compressor.CompressionFinished -= Compressor_CompressionFinished;

			status = ArchiveCreatorStatus.Completed;
			percent = 100;
			UpdateProgression();
		}

		private void UpdateProgression()
			=> ProgressionUpdated?.Invoke(this, this);
	}
}
