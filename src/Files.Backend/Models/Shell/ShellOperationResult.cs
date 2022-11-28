using System.Collections.Generic;
using System.Linq;

namespace Files.Backend.Models.Shell
{
	public class ShellOperationResult : IShellOperationResult
	{
		public bool IsSucceeded { get; set; }

		/// <inheritdoc />
		public IList<IShellOperationItemResult> Items { get; } = new List<IShellOperationItemResult>();

		/// <inheritdoc />
		public IList<IShellOperationItemResult> ToFinal() => Items
			.GroupBy(item => (item.Source, item.Destination))
			.Select(item => item.Last())
			.ToList();
	}
}
