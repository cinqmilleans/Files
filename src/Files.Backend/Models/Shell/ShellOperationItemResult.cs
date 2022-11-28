namespace Files.Backend.Models.Shell
{
	public class ShellOperationItemResult : IShellOperationItemResult
	{
		public bool IsSucceeded { get; init; } = false;
		public int HResult { get; init; } = -1;

		public string Source { get; init; } = string.Empty;
		public string Destination { get; init; } = string.Empty;
	}
}
