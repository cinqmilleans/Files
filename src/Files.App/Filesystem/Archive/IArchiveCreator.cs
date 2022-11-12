using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Files.App.Filesystem.Archive
{
	public interface IArchiveCreator
	{
		event EventHandler<IArchiveCreator>? ProgressionUpdated;

		string ArchiveName { get; }

		string Directory { get; set; }
		string FileName { get; set; }
		string Password { get; set; }

		IEnumerable<string> Sources { get; set; }

		ArchiveFormats FileFormat { get; set; }
		ArchiveCompressionLevels CompressionLevel { get; set; }
		ArchiveSplittingSizes SplittingSize { get; set; }

		ArchiveCreatorStatus Status { get; }
		float Percent { get; }

		void RunCreationAsync();
	}
}
