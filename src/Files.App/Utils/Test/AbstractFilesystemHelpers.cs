// Copyright (c) 2024 Files Community
// Licensed under the MIT License. See the LICENSE.

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace Files.App.Utils.Storage;

public abstract class AbstractFilesystemHelpers(IShellPage page) : IFilesystemHelpers
{
	protected IShellPage Page { get; } = page;

	private IFoldersSettingsService FolderService { get; } = Ioc.Default.GetRequiredService<IFoldersSettingsService>();

	public virtual async Task<ReturnResult> PerformOperationTypeAsync
	(
		DataPackageOperation operation
	,	DataPackageView packageView
	,	string destination
	,	bool showDialog
	,	bool registerHistory
	,	bool isTargetExecutable = false
	,	bool isTargetScriptFile = false
	)
	{
		try
		{
			if (destination is null)
				return default;
			if (destination.StartsWith(Constants.UserEnvironmentPaths.RecycleBinPath, StringComparison.Ordinal))
				return await RecycleItemsFromClipboard(packageView, destination, FolderService.DeleteConfirmationPolicy, registerHistory);
			if (operation.HasFlag(DataPackageOperation.Move))
				return await MoveItemsFromClipboard(packageView, destination, showDialog, registerHistory);
			if (operation.HasFlag(DataPackageOperation.Copy))
				return await CopyItemsFromClipboard(packageView, destination, showDialog, registerHistory);
			if (operation.HasFlag(DataPackageOperation.Link))
			{
				// Open with piggybacks off of the link operation, since there isn't one for it
				if (isTargetExecutable || isTargetScriptFile)
				{
					var items = await FilesystemHelpers.GetDraggedStorageItems(packageView);
					await NavigationHelpers.OpenItemsWithExecutableAsync(page, items, destination);
					return ReturnResult.Success;
				}
				return await CreateShortcutFromClipboard(packageView, destination, showDialog, registerHistory);
			}
			if (operation.HasFlag(DataPackageOperation.None))
				return await CopyItemsFromClipboard(packageView, destination, showDialog, registerHistory);

			return default;
		}
		finally
		{
			packageView.ReportOperationCompleted(packageView.RequestedOperation);
		}
	}

	public virtual Task<(ReturnResult, IStorageItem?)> CreateAsync(IStorageItemWithPath source, bool registerHistory)
		=> Task.FromResult<(ReturnResult, IStorageItem?)>((ReturnResult.Failed, null));
	public virtual Task<ReturnResult> CreateShortcutFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
		=> Task.FromResult(ReturnResult.Failed);

	public virtual Task<ReturnResult> DeleteItemAsync(IStorageItem source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
		=> DeleteItemsAsync(ToItems(source), showDialog, permanently, registerHistory);
	public virtual Task<ReturnResult> DeleteItemAsync(IStorageItemWithPath source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
		=> DeleteItemsAsync(ToItems(source), showDialog, permanently, registerHistory);
	public virtual Task<ReturnResult> DeleteItemsAsync(IEnumerable<IStorageItem> source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
		=> DeleteItemsAsync(ToItems(source), showDialog, permanently, registerHistory);
	public virtual Task<ReturnResult> DeleteItemsAsync(IEnumerable<IStorageItemWithPath> source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
		=> Task.FromResult(ReturnResult.Failed);

	public virtual Task<ReturnResult> RestoreItemFromTrashAsync(IStorageItem source, string destination, bool registerHistory)
		=> RestoreItemsFromTrashAsync(ToItems(source), [destination], registerHistory);
	public virtual Task<ReturnResult> RestoreItemFromTrashAsync(IStorageItemWithPath source, string destination, bool registerHistory)
		=> RestoreItemsFromTrashAsync(ToItems(source), [destination], registerHistory);
	public virtual Task<ReturnResult> RestoreItemsFromTrashAsync(IEnumerable<IStorageItem> source, IEnumerable<string> destination, bool registerHistory)
		=> RestoreItemsFromTrashAsync(ToItems(source), destination, registerHistory);
	public virtual Task<ReturnResult> RestoreItemsFromTrashAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool registerHistory)
		=> Task.FromResult(ReturnResult.Failed);

	public virtual async Task<ReturnResult> RecycleItemsFromClipboard(DataPackageView packageView, string destination, DeleteConfirmationPolicies showDialog, bool registerHistory)
	{
		if (!FilesystemHelpers.HasDraggedStorageItems(packageView))
		{
			// Happens if you copy some text and then you Ctrl+V in Files
			return ReturnResult.BadArgumentException;
		}

		IEnumerable<IStorageItemWithPath> source = await FilesystemHelpers.GetDraggedStorageItems(packageView);
		source = source.Where(x => !RecycleBinHelpers.IsPathUnderRecycleBin(x.Path)); // Can't recycle items already in recyclebin

		return await DeleteItemsAsync(source, showDialog, false, registerHistory);
	}

	public virtual Task<ReturnResult> CopyItemAsync(IStorageItem source, string destination, bool showDialog, bool registerHistory)
		=> CopyItemsAsync(ToItems(source), [destination], showDialog, registerHistory);
	public virtual Task<ReturnResult> CopyItemAsync(IStorageItemWithPath source, string destination, bool showDialog, bool registerHistory)
		=> CopyItemsAsync(ToItems(source), [destination], showDialog, registerHistory);
	public virtual Task<ReturnResult> CopyItemsAsync(IEnumerable<IStorageItem> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
		=> CopyItemsAsync(ToItems(source), destination, showDialog, registerHistory);
	public virtual Task<ReturnResult> CopyItemsAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
		=> Task.FromResult(ReturnResult.Failed);
	public virtual Task<ReturnResult> CopyItemsFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
		=> Task.FromResult(ReturnResult.Failed);

	public virtual Task<ReturnResult> MoveItemAsync(IStorageItem source, string destination, bool showDialog, bool registerHistory)
		=> MoveItemsAsync(ToItems(source), [destination], showDialog, registerHistory);
	public virtual Task<ReturnResult> MoveItemAsync(IStorageItemWithPath source, string destination, bool showDialog, bool registerHistory)
		=> MoveItemsAsync(ToItems(source), [destination], showDialog, registerHistory);
	public virtual Task<ReturnResult> MoveItemsAsync(IEnumerable<IStorageItem> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
		=> MoveItemsAsync(ToItems(source), destination, showDialog, registerHistory);
	public virtual Task<ReturnResult> MoveItemsAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
		=> Task.FromResult(ReturnResult.Failed);
	public virtual Task<ReturnResult> MoveItemsFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
		=> Task.FromResult(ReturnResult.Failed);

	public virtual Task<ReturnResult> RenameAsync(IStorageItem source, string newName, NameCollisionOption collision, bool registerHistory, bool showExtensionDialog = true)
		=> RenameAsync(source.FromStorageItem(), newName, collision, showExtensionDialog);
	public virtual Task<ReturnResult> RenameAsync(IStorageItemWithPath source, string newName, NameCollisionOption collision, bool registerHistory, bool showExtensionDialog = true)
		=> Task.FromResult(ReturnResult.Failed);

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
	protected virtual void Dispose(bool disposing)
	{
	}

	private static IEnumerable<IStorageItemWithPath> ToItems(IStorageItem item)
		=> [item.FromStorageItem()];
	private static IEnumerable<IStorageItemWithPath> ToItems(IStorageItemWithPath item)
		=> [item];
	private static IEnumerable<IStorageItemWithPath> ToItems(IEnumerable<IStorageItem> items)
		=> items.Select(item => item.FromStorageItem());
}
