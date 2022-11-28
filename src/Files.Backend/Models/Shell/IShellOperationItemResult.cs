namespace Files.Backend.Models.Shell
{
	public interface IShellOperationItemResult
	{
		bool IsSucceeded { get; }
		int HResult { get; }

		string Source { get; }
		string Destination { get; }
	}
}
