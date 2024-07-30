// Copyright (c) 2024 Files Community
// Licensed under the MIT License. See the LICENSE.

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace Files.App.Utils.Storage;

public abstract class DelegateFilesystemHelpers : IFilesystemHelpers
{
	private readonly IFilesystemHelpers helper;

	public DelegateFilesystemHelpers(IFilesystemHelpers helper)
		=> this.helper = helper;

	#region operation
	public virtual Task<ReturnResult> PerformOperationTypeAsync
	(
		DataPackageOperation operation
	,	DataPackageView packageView
	,	string destination
	,	bool showDialog
	,	bool registerHistory
	,	bool isTargetExecutable = false
	,	bool isTargetScriptFile = false
	) => helper.PerformOperationTypeAsync(operation, packageView, destination, showDialog, registerHistory, isTargetExecutable, isTargetScriptFile);
	#endregion

	#region create
	public virtual Task<(ReturnResult, IStorageItem?)> CreateAsync(IStorageItemWithPath source, bool registerHistory)
		=> helper.CreateAsync(source, registerHistory);
	public virtual Task<ReturnResult> CreateShortcutFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
		=> helper.CreateShortcutFromClipboard(packageView, destination, showDialog, registerHistory);
	#endregion

	#region delete
	public virtual Task<ReturnResult> DeleteItemAsync(IStorageItem source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
		=> helper.DeleteItemAsync(source, showDialog, permanently, registerHistory);
	public virtual Task<ReturnResult> DeleteItemAsync(IStorageItemWithPath source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
		=> helper.DeleteItemAsync(source, showDialog, permanently, registerHistory);
	public virtual Task<ReturnResult> DeleteItemsAsync(IEnumerable<IStorageItem> source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
		=> helper.DeleteItemsAsync(source, showDialog, permanently, registerHistory);
	public virtual Task<ReturnResult> DeleteItemsAsync(IEnumerable<IStorageItemWithPath> source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
		=> helper.DeleteItemsAsync(source, showDialog, permanently, registerHistory);
	#endregion

	#region restore
	public virtual Task<ReturnResult> RestoreItemFromTrashAsync(IStorageItem source, string destination, bool registerHistory)
		=> helper.RestoreItemFromTrashAsync(source, destination, registerHistory);
	public virtual Task<ReturnResult> RestoreItemFromTrashAsync(IStorageItemWithPath source, string destination, bool registerHistory)
		=> helper.RestoreItemFromTrashAsync(source, destination, registerHistory);
	public virtual Task<ReturnResult> RestoreItemsFromTrashAsync(IEnumerable<IStorageItem> source, IEnumerable<string> destination, bool registerHistory)
		=> helper.RestoreItemsFromTrashAsync(source, destination, registerHistory);
	public virtual Task<ReturnResult> RestoreItemsFromTrashAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool registerHistory)
		=> helper.RestoreItemsFromTrashAsync(source, destination, registerHistory);
	public virtual Task<ReturnResult> RecycleItemsFromClipboard(DataPackageView packageView, string destination, DeleteConfirmationPolicies showDialog, bool registerHistory)
		=> helper.RecycleItemsFromClipboard(packageView, destination, showDialog, registerHistory);
	#endregion

	#region copy
	public virtual Task<ReturnResult> CopyItemAsync(IStorageItem source, string destination, bool showDialog, bool registerHistory)
		=> helper.CopyItemAsync(source, destination, showDialog, registerHistory);
	public virtual Task<ReturnResult> CopyItemAsync(IStorageItemWithPath source, string destination, bool showDialog, bool registerHistory)
		=> helper.CopyItemAsync(source, destination, showDialog, registerHistory);
	public virtual Task<ReturnResult> CopyItemsAsync(IEnumerable<IStorageItem> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
		=> helper.CopyItemsAsync(source, destination, showDialog, registerHistory);
	public virtual Task<ReturnResult> CopyItemsAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
		=> helper.CopyItemsAsync(source, destination, showDialog, registerHistory);
	public virtual Task<ReturnResult> CopyItemsFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
		=> helper.CopyItemsFromClipboard(packageView, destination, showDialog, registerHistory);
	#endregion

	#region move
	public virtual Task<ReturnResult> MoveItemAsync(IStorageItem source, string destination, bool showDialog, bool registerHistory)
		=> helper.MoveItemAsync(source, destination, showDialog, registerHistory);
	public virtual Task<ReturnResult> MoveItemAsync(IStorageItemWithPath source, string destination, bool showDialog, bool registerHistory)
		=> helper.MoveItemAsync(source, destination, showDialog, registerHistory);
	public virtual Task<ReturnResult> MoveItemsAsync(IEnumerable<IStorageItem> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
		=> helper.MoveItemsAsync(source, destination, showDialog, registerHistory);
	public virtual Task<ReturnResult> MoveItemsAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
		=> helper.MoveItemsAsync(source, destination, showDialog, registerHistory);
	public virtual Task<ReturnResult> MoveItemsFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
		=> helper.MoveItemsFromClipboard(packageView, destination, showDialog, registerHistory);
	#endregion

	#region rename
	public virtual Task<ReturnResult> RenameAsync(IStorageItem source, string newName, NameCollisionOption collision, bool registerHistory, bool showExtensionDialog = true)
		=> helper.RenameAsync(source, newName, collision, showExtensionDialog);
	public virtual Task<ReturnResult> RenameAsync(IStorageItemWithPath source, string newName, NameCollisionOption collision, bool registerHistory, bool showExtensionDialog = true)
		=> helper.RenameAsync(source, newName, collision, showExtensionDialog);
	#endregion

	#region dispose
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			helper.Dispose();
		}
	}
	#endregion
}
