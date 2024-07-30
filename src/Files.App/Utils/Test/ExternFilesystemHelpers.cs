// Copyright (c) 2024 Files Community
// Licensed under the MIT License. See the LICENSE.

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace Files.App.Utils.Storage;

public class ExternFilesystemHelpers(IFilesystemHelpers externHelpers, IFilesystemHelpers defaultHelpers) : IFilesystemHelpers
{
	private readonly IFilesystemHelpers externHelpers  = externHelpers ;
	private readonly IFilesystemHelpers defaultHelpers = defaultHelpers;

	#region operation
	public async Task<ReturnResult> PerformOperationTypeAsync
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
		ReturnResult result = await externHelpers
			.PerformOperationTypeAsync(operation, packageView, destination, showDialog, registerHistory, isTargetExecutable, isTargetScriptFile);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers
			.PerformOperationTypeAsync(operation, packageView, destination, showDialog, registerHistory, isTargetExecutable, isTargetScriptFile);
	}
	#endregion

	#region create
	public async Task<(ReturnResult, IStorageItem?)> CreateAsync(IStorageItemWithPath source, bool registerHistory)
	{
		(ReturnResult result, IStorageItem? item) = await externHelpers.CreateAsync(source, registerHistory);
		if (result is ReturnResult.Success)
			return (ReturnResult.Success, item);
		return await defaultHelpers.CreateAsync(source, registerHistory);
	}

	public async Task<ReturnResult> CreateShortcutFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
	{
		ReturnResult result = await externHelpers.CreateShortcutFromClipboard(packageView, destination, showDialog, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.CreateShortcutFromClipboard(packageView, destination, showDialog, registerHistory);
	}
	#endregion

	#region delete
	public async Task<ReturnResult> DeleteItemAsync(IStorageItem source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
	{
		ReturnResult result = await externHelpers.DeleteItemAsync(source, showDialog, permanently, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.DeleteItemAsync(source, showDialog, permanently, registerHistory);
	}
	public async Task<ReturnResult> DeleteItemAsync(IStorageItemWithPath source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
	{
		ReturnResult result = await externHelpers.DeleteItemAsync(source, showDialog, permanently, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.DeleteItemAsync(source, showDialog, permanently, registerHistory);
	}

	public async Task<ReturnResult> DeleteItemsAsync(IEnumerable<IStorageItem> source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
	{
		ReturnResult result = await externHelpers.DeleteItemsAsync(source, showDialog, permanently, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.DeleteItemsAsync(source, showDialog, permanently, registerHistory);
	}
	public async Task<ReturnResult> DeleteItemsAsync(IEnumerable<IStorageItemWithPath> source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
	{
		ReturnResult result = await externHelpers.DeleteItemsAsync(source, showDialog, permanently, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.DeleteItemsAsync(source, showDialog, permanently, registerHistory);
	}
	#endregion

	#region restore
	public async Task<ReturnResult> RestoreItemFromTrashAsync(IStorageItem source, string destination, bool registerHistory)
	{
		ReturnResult result = await externHelpers.RestoreItemFromTrashAsync(source, destination, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.RestoreItemFromTrashAsync(source, destination, registerHistory);
	}
	public async Task<ReturnResult> RestoreItemFromTrashAsync(IStorageItemWithPath source, string destination, bool registerHistory)
	{
		ReturnResult result = await externHelpers.RestoreItemFromTrashAsync(source, destination, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.RestoreItemFromTrashAsync(source, destination, registerHistory);
	}

	public async Task<ReturnResult> RestoreItemsFromTrashAsync(IEnumerable<IStorageItem> source, IEnumerable<string> destination, bool registerHistory)
	{
		ReturnResult result = await externHelpers.RestoreItemsFromTrashAsync(source, destination, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.RestoreItemsFromTrashAsync(source, destination, registerHistory);
	}
	public async Task<ReturnResult> RestoreItemsFromTrashAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool registerHistory)
	{
		ReturnResult result = await externHelpers.RestoreItemsFromTrashAsync(source, destination, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.RestoreItemsFromTrashAsync(source, destination, registerHistory);
	}
	#endregion

	#region recyle
	public async Task<ReturnResult> RecycleItemsFromClipboard(DataPackageView packageView, string destination, DeleteConfirmationPolicies showDialog, bool registerHistory)
	{
		ReturnResult result = await externHelpers.RecycleItemsFromClipboard(packageView, destination, showDialog, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.RecycleItemsFromClipboard(packageView, destination, showDialog, registerHistory);
	}
	#endregion

	#region copy
	public async Task<ReturnResult> CopyItemAsync(IStorageItem source, string destination, bool showDialog, bool registerHistory)
	{
		ReturnResult result = await externHelpers.CopyItemAsync(source, destination, showDialog, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.CopyItemAsync(source, destination, showDialog, registerHistory);
	}
	public async Task<ReturnResult> CopyItemAsync(IStorageItemWithPath source, string destination, bool showDialog, bool registerHistory)
	{
		ReturnResult result = await externHelpers.CopyItemAsync(source, destination, showDialog, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.CopyItemAsync(source, destination, showDialog, registerHistory);
	}

	public async Task<ReturnResult> CopyItemsAsync(IEnumerable<IStorageItem> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
	{
		ReturnResult result = await externHelpers.CopyItemsAsync(source, destination, showDialog, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.CopyItemsAsync(source, destination, showDialog, registerHistory);
	}
	public async Task<ReturnResult> CopyItemsAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
	{
		ReturnResult result = await externHelpers.CopyItemsAsync(source, destination, showDialog, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.CopyItemsAsync(source, destination, showDialog, registerHistory);
	}

	public async Task<ReturnResult> CopyItemsFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
	{
		ReturnResult result = await externHelpers.CopyItemsFromClipboard(packageView, destination, showDialog, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.CopyItemsFromClipboard(packageView, destination, showDialog, registerHistory);
	}
	#endregion

	#region move
	public async Task<ReturnResult> MoveItemAsync(IStorageItem source, string destination, bool showDialog, bool registerHistory)
	{
		ReturnResult result = await externHelpers.MoveItemAsync(source, destination, showDialog, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.MoveItemAsync(source, destination, showDialog, registerHistory);
	}
	public async Task<ReturnResult> MoveItemAsync(IStorageItemWithPath source, string destination, bool showDialog, bool registerHistory)
	{
		ReturnResult result = await externHelpers.MoveItemAsync(source, destination, showDialog, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.MoveItemAsync(source, destination, showDialog, registerHistory);
	}

	public async Task<ReturnResult> MoveItemsAsync(IEnumerable<IStorageItem> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
	{
		ReturnResult result = await externHelpers.MoveItemsAsync(source, destination, showDialog, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.MoveItemsAsync(source, destination, showDialog, registerHistory);
	}
	public async Task<ReturnResult> MoveItemsAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
	{
		ReturnResult result = await externHelpers.MoveItemsAsync(source, destination, showDialog, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.MoveItemsAsync(source, destination, showDialog, registerHistory);
	}

	public async Task<ReturnResult> MoveItemsFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
	{
		ReturnResult result = await externHelpers.MoveItemsFromClipboard(packageView, destination, showDialog, registerHistory);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.MoveItemsFromClipboard(packageView, destination, showDialog, registerHistory);
	}
	#endregion

	#region rename
	public async Task<ReturnResult> RenameAsync(IStorageItem source, string newName, NameCollisionOption collision, bool registerHistory, bool showExtensionDialog = true)
	{
		ReturnResult result = await externHelpers.RenameAsync(source, newName, collision, registerHistory, showExtensionDialog);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.RenameAsync(source, newName, collision, registerHistory, showExtensionDialog);
	}

	public async Task<ReturnResult> RenameAsync(IStorageItemWithPath source, string newName, NameCollisionOption collision, bool registerHistory, bool showExtensionDialog = true)
	{
		ReturnResult result = await externHelpers.RenameAsync(source, newName, collision, registerHistory, showExtensionDialog);
		if (result is ReturnResult.Success)
			return ReturnResult.Success;
		return await defaultHelpers.RenameAsync(source, newName, collision, registerHistory, showExtensionDialog);
	}
	#endregion

	#region dispose
	public void Dispose()
	{
		externHelpers .Dispose();
		defaultHelpers.Dispose();
	}
	#endregion

}
