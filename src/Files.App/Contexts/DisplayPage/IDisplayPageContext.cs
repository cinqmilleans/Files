﻿using Files.Shared.Enums;
using System.ComponentModel;

namespace Files.App.Contexts
{
	public interface IDisplayPageContext : INotifyPropertyChanged
	{
		bool IsLayoutAdaptiveEnabled { get; set; }
		LayoutTypes LayoutType { get; set; }

		SortOption SortOption { get; set; }
		SortDirection SortDirection { get; set; }

		GroupOption GroupOption { get; set; }
		SortDirection GroupDirection { get; set; }
	}
}
