﻿using Files.App.Contexts;
using Files.App.Extensions;
using Files.Shared.Enums;

namespace Files.App.Actions
{
	internal class GroupByOriginalFolderAction : GroupByAction
	{
		protected override GroupOption GroupOption { get; } = GroupOption.OriginalFolder;

		public override string Label { get; } = "NavToolbarArrangementOptionOriginalFolder/Text".GetLocalizedResource();

		protected override bool GetIsExecutable(ContentPageTypes pageType) => pageType is ContentPageTypes.CloudDrive;
	}
}
