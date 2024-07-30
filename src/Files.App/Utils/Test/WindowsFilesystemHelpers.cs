// Copyright (c) 2024 Files Community
// Licensed under the MIT License. See the LICENSE.

using System.IO;
using Vanara.Windows.Shell;
using Windows.ApplicationModel.DataTransfer;

namespace Files.App.Utils.Storage;

public class WindowsFilesystemHelpers : AbstractFilesystemHelpers
{
	public WindowsFilesystemHelpers(IShellPage page) : base(page)
	{
	}

	public override Task<ReturnResult> CopyItemsAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
	{
		IList<ShellItem> sources = source.Select(s => new ShellItem(s.Path)).ToList();
		IList<string> targets = destination.Select(d => Directory.GetParent(d)!.FullName).ToList();

		if (targets.Distinct().Count() is 1)
		{
			ShellFileOperations2.Copy(sources, new ShellFolder(targets[0]));
		}
		else
		{
			for (int i = 0; i < sources.Count; ++i)
				ShellFileOperations2.Copy(sources[i], new ShellFolder(targets[i]));
		}

		return Task.FromResult(ReturnResult.Success);
	}
	public override async Task<ReturnResult> CopyItemsFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
	{
		IEnumerable<IStorageItemWithPath> sources = await FilesystemHelpers.GetDraggedStorageItems(packageView);
		IEnumerable<ShellItem> items = sources.Select(i => new ShellItem(i.Path));
		ShellFolder target = new(destination);

		ShellFileOperations2.Copy(items, target);
		return ReturnResult.Success;
	}

	public override Task<ReturnResult> MoveItemsAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
	{
		IList<ShellItem> sources = source.Select(s => new ShellItem(s.Path)).ToList();
		IList<string> targets = destination.Select(d => Directory.GetParent(d)!.FullName).ToList();

		if (targets.Distinct().Count() is 1)
		{
			ShellFileOperations2.Move(sources, new ShellFolder(targets[0]));
		}
		else
		{
			for (int i = 0; i < sources.Count; ++i)
				ShellFileOperations2.Move(sources[i], new ShellFolder(targets[i]));
		}

		return Task.FromResult(ReturnResult.Success);
	}
	public override async Task<ReturnResult> MoveItemsFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
	{
		IEnumerable<IStorageItemWithPath> sources = await FilesystemHelpers.GetDraggedStorageItems(packageView);
		IEnumerable<ShellItem> items = sources.Select(i => new ShellItem(i.Path));
		ShellFolder target = new(destination);

		ShellFileOperations2.Move(items, target);
		return ReturnResult.Success;
	}
}
