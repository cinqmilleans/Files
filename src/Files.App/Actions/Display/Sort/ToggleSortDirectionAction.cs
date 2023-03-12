﻿using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Contexts;
using Files.App.Extensions;
using Files.Shared.Enums;
using System.Threading.Tasks;

namespace Files.App.Actions
{
	internal class ToggleSortDirectionAction : IAction
	{
		private IDisplayPageContext context = Ioc.Default.GetRequiredService<IDisplayPageContext>();

		public string Label { get; } = "ToggleSortDirection".GetLocalizedResource();

		public Task ExecuteAsync()
		{
			context.SortDirection = context.SortDirection is SortDirection.Descending ? SortDirection.Ascending : SortDirection.Descending;
			return Task.CompletedTask;
		}
	}
}
