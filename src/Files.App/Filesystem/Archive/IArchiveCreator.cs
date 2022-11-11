using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Files.App.Filesystem.Archive
{
	public interface IArchiveCreator
	{
		string FileName { get; set; }
		string Password { get; set; }

		IEnumerable<string> Sources { get; set; }

		ArchiveFormats FileFormat { get; set; }
		ArchiveCompressionLevels CompressionLevel { get; set; }
		ArchiveSplittingSizes SplittingSize { get; set; }

		IProgress<float> Progress { get; set; }

		Task<bool> CreateArchive();
	}
}
