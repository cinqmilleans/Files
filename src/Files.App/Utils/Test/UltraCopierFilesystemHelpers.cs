// Copyright (c) 2024 Files Community
// Licensed under the MIT License. See the LICENSE.

using System.IO;
using Windows.ApplicationModel.DataTransfer;

namespace Files.App.Utils.Storage;

public class UltraCopierFilesystemHelpers : AbstractFilesystemHelpers
{
	public UltraCopierFilesystemHelpers(IShellPage page) : base(page)
	{
	}

	public override Task<ReturnResult> CopyItemsAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
	{
		IList<string> sources = source.Select(s => s.Path).ToList();
		IList<string> targets = destination.Select(d => Directory.GetParent(d)!.FullName).ToList();

		if (targets.Distinct().Count() is 1)
		{
			return Execute("cp", sources, targets[0])
				? Task.FromResult(ReturnResult.Success)
				: Task.FromResult(ReturnResult.Failed);
		}

		for (int i = 0; i < sources.Count; ++i)
		{
			if (!Execute("cp", [sources[i]], targets[i]))
				return Task.FromResult(ReturnResult.Failed);
		}

		return Task.FromResult(ReturnResult.Success);
	}
	public override async Task<ReturnResult> CopyItemsFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
	{
		IEnumerable<IStorageItemWithPath> sources = await FilesystemHelpers.GetDraggedStorageItems(packageView);
		IEnumerable<string> items = sources.Select(s => s.Path);
		
		return Execute("cp", items, destination) ? ReturnResult.Success : ReturnResult.Failed;
	}

	public override Task<ReturnResult> MoveItemsAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
	{
		IList<string> sources = source.Select(s => s.Path).ToList();
		IList<string> targets = destination.Select(d => Directory.GetParent(d)!.FullName).ToList();

		if (targets.Distinct().Count() is 1)
		{
			return Execute("mv", sources, targets[0])
				? Task.FromResult(ReturnResult.Success)
				: Task.FromResult(ReturnResult.Failed);
		}
		
		for (int i = 0; i < sources.Count; ++i)
		{
			if (!Execute("mv", [sources[i]], targets[i]))
				return Task.FromResult(ReturnResult.Failed);
		}

		return Task.FromResult(ReturnResult.Success);
	}
	public override async Task<ReturnResult> MoveItemsFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
	{
		IEnumerable<IStorageItemWithPath> sources = await FilesystemHelpers.GetDraggedStorageItems(packageView);
		IEnumerable<string> items = sources.Select(s => s.Path);

		return Execute("mv", items, destination) ? ReturnResult.Success : ReturnResult.Failed;
	}

	private static bool Execute(string type, IEnumerable<string> sources, string destination)
	{
		Process? p = Process.GetProcessesByName("ultracopier").FirstOrDefault();
		if (p?.MainModule is null)
			return false;

		string query = type + ' ' + string.Join(' ', sources.Append(destination).Select(item => $@"""{item}"""));
		Process.Start(p.MainModule.FileName, query);

		return true;
	}
}
