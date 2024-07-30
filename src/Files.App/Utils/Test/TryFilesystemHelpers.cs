// Copyright (c) 2024 Files Community
// Licensed under the MIT License. See the LICENSE.

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace Files.App.Utils.Storage;

public class TryFilesystemHelpers : DelegateFilesystemHelpers
{
	public TryFilesystemHelpers(IFilesystemHelpers helper) : base(helper)
	{
	}

	#region operation
	public override async Task<ReturnResult> PerformOperationTypeAsync
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
			return await base.PerformOperationTypeAsync(operation, packageView, destination, showDialog, registerHistory, isTargetExecutable, isTargetScriptFile);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
		
	#endregion

	#region create
	public override async Task<(ReturnResult, IStorageItem?)> CreateAsync(IStorageItemWithPath source, bool registerHistory)
	{
		try
		{
			return await base.CreateAsync(source, registerHistory);
		}
		catch
		{
			return (ReturnResult.Failed, null);
		}
	}
	public override async Task<ReturnResult> CreateShortcutFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
	{
		try
		{
			return await base.CreateShortcutFromClipboard(packageView, destination, showDialog, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	#endregion

	#region delete
	public override async Task<ReturnResult> DeleteItemAsync(IStorageItem source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
	{
		try
		{
			return await base.DeleteItemAsync(source, showDialog, permanently, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> DeleteItemAsync(IStorageItemWithPath source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
	{
		try
		{
			return await base.DeleteItemAsync(source, showDialog, permanently, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> DeleteItemsAsync(IEnumerable<IStorageItem> source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
	{
		try
		{
			return await base.DeleteItemsAsync(source, showDialog, permanently, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> DeleteItemsAsync(IEnumerable<IStorageItemWithPath> source, DeleteConfirmationPolicies showDialog, bool permanently, bool registerHistory)
	{
		try
		{
			return await base.DeleteItemsAsync(source, showDialog, permanently, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	#endregion

	#region restore
	public override async Task<ReturnResult> RestoreItemFromTrashAsync(IStorageItem source, string destination, bool registerHistory)
	{
		try
		{
			return await base.RestoreItemFromTrashAsync(source, destination, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> RestoreItemFromTrashAsync(IStorageItemWithPath source, string destination, bool registerHistory)
	{
		try
		{
			return await base.RestoreItemFromTrashAsync(source, destination, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> RestoreItemsFromTrashAsync(IEnumerable<IStorageItem> source, IEnumerable<string> destination, bool registerHistory)
	{
		try
		{
			return await base.RestoreItemsFromTrashAsync(source, destination, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> RestoreItemsFromTrashAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool registerHistory)
	{
		try
		{
			return await base.RestoreItemsFromTrashAsync(source, destination, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> RecycleItemsFromClipboard(DataPackageView packageView, string destination, DeleteConfirmationPolicies showDialog, bool registerHistory)
	{
		try
		{
			return await base.RecycleItemsFromClipboard(packageView, destination, showDialog, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	#endregion

	#region copy
	public override async Task<ReturnResult> CopyItemAsync(IStorageItem source, string destination, bool showDialog, bool registerHistory)
	{
		try
		{
			return await base.CopyItemAsync(source, destination, showDialog, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> CopyItemAsync(IStorageItemWithPath source, string destination, bool showDialog, bool registerHistory)
	{
		try
		{
			return await base.CopyItemAsync(source, destination, showDialog, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> CopyItemsAsync(IEnumerable<IStorageItem> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
	{
		try
		{
			return await base.CopyItemsAsync(source, destination, showDialog, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> CopyItemsAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
	{
		try
		{
			return await base.CopyItemsAsync(source, destination, showDialog, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> CopyItemsFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
	{
		try
		{
			return await base.CopyItemsFromClipboard(packageView, destination, showDialog, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	#endregion

	#region move
	public override async Task<ReturnResult> MoveItemAsync(IStorageItem source, string destination, bool showDialog, bool registerHistory)
	{
		try
		{
			return await base.MoveItemAsync(source, destination, showDialog, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> MoveItemAsync(IStorageItemWithPath source, string destination, bool showDialog, bool registerHistory)
	{
		try
		{
			return await base.MoveItemAsync(source, destination, showDialog, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> MoveItemsAsync(IEnumerable<IStorageItem> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
	{
		try
		{
			return await base.MoveItemsAsync(source, destination, showDialog, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> MoveItemsAsync(IEnumerable<IStorageItemWithPath> source, IEnumerable<string> destination, bool showDialog, bool registerHistory)
	{
		try
		{
			return await base.MoveItemsAsync(source, destination, showDialog, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> MoveItemsFromClipboard(DataPackageView packageView, string destination, bool showDialog, bool registerHistory)
	{
		try
		{
			return await base.MoveItemsFromClipboard(packageView, destination, showDialog, registerHistory);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	#endregion

	#region rename
	public override async Task<ReturnResult> RenameAsync(IStorageItem source, string newName, NameCollisionOption collision, bool registerHistory, bool showExtensionDialog = true)
	{
		try
		{
			return await base.RenameAsync(source, newName, collision, showExtensionDialog);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	public override async Task<ReturnResult> RenameAsync(IStorageItemWithPath source, string newName, NameCollisionOption collision, bool registerHistory, bool showExtensionDialog = true)
	{
		try
		{
			return await base.RenameAsync(source, newName, collision, showExtensionDialog);
		}
		catch
		{
			return ReturnResult.Failed;
		}
	}
	#endregion

}
