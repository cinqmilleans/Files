using System.Collections.Generic;

namespace Files.Backend.Models.Shell
{
	public interface IShellOperationResult
	{
		bool IsSucceeded { get; }

		/// <summary>
		/// File operation results: success and error code. Can contains multiple results for the same source file.
		/// E.g. if the shell shows a "replace" confirmation dialog, results can be both COPYENGINE_S_PENDING and COPYENGINE_S_USER_IGNORED.
		/// </summary>
		public IList<IShellOperationItemResult> Items { get; }

		/// <summary>
		/// Final results of a file operation. Contains last status for each source file.
		/// </summary>
		public IList<IShellOperationItemResult> ToFinal();
	}
}
