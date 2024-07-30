namespace Files.App.Utils;

internal static class FilesystemHelpersFactory
{
	private const string defaultCode = "application"; // application or windows or ultracopier

	public static IFilesystemHelpers GetHelpers(IShellPage page, CancellationToken cancellationToken)
		=> GetHelpers(defaultCode, page, cancellationToken);

	public static IFilesystemHelpers GetHelpers(string name, IShellPage page, CancellationToken cancellationToken) => name switch
	{
		"windows" => new TryFilesystemHelpers(new FilesystemHelpers(page, cancellationToken)),
		"ultracopier" => new ExternFilesystemHelpers(
			new TryFilesystemHelpers(new UltraCopierFilesystemHelpers(page))
		,	new TryFilesystemHelpers(new FilesystemHelpers(page, cancellationToken))
		),
		_ => new FilesystemHelpers(page, cancellationToken),
	};
}
