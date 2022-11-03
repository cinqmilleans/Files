using CommunityToolkit.Mvvm.ComponentModel;
using Files.Backend.Models;
using Windows.System;

namespace Files.App.ViewModels
{
	internal class ShortKeysViewModel : ObservableObject, IShortKeysViewModel
	{
		// selection
		private ShortKey toggleMultiSelection = ShortKey.None;
		public ShortKey ToggleMultiSelection
		{
			get => toggleMultiSelection;
			set => SetProperty(ref toggleMultiSelection, value);
		}

		private ShortKey selectAll = new(VirtualKey.A, VirtualKeyModifiers.Control);
		public ShortKey SelectAll
		{
			get => selectAll;
			set => SetProperty(ref selectAll, value);
		}

		private ShortKey invertSelection = ShortKey.None;
		public ShortKey InvertSelection
		{
			get => invertSelection;
			set => SetProperty(ref invertSelection, value);
		}

		private ShortKey clearSelection = ShortKey.None;
		public ShortKey ClearSelection
		{
			get => clearSelection;
			set => SetProperty(ref clearSelection, value);
		}

		// layout
		private ShortKey toggleLayoutDetails = new(VirtualKey.Number1, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);
		public ShortKey ToggleLayoutDetails
		{
			get => toggleLayoutDetails;
			set => SetProperty(ref toggleLayoutDetails, value);
		}

		private ShortKey toggleLayoutTiles = new(VirtualKey.Number2, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);
		public ShortKey ToggleLayoutTiles
		{
			get => toggleLayoutTiles;
			set => SetProperty(ref toggleLayoutTiles, value);
		}

		private ShortKey toggleLayoutGridSmall = new(VirtualKey.Number3, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);
		public ShortKey ToggleLayoutGridSmall
		{
			get => toggleLayoutGridSmall;
			set => SetProperty(ref toggleLayoutGridSmall, value);
		}

		private ShortKey toggleLayoutGridMedium = new(VirtualKey.Number4, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);
		public ShortKey ToggleLayoutGridMedium
		{
			get => toggleLayoutGridMedium;
			set => SetProperty(ref toggleLayoutGridMedium, value);
		}

		private ShortKey toggleLayoutGridLarge = new(VirtualKey.Number5, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);
		public ShortKey ToggleLayoutGridLarge
		{
			get => toggleLayoutGridLarge;
			set => SetProperty(ref toggleLayoutGridLarge, value);
		}

		private ShortKey toggleLayoutColumns = new(VirtualKey.Number6, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);
		public ShortKey ToggleLayoutColumns
		{
			get => toggleLayoutColumns;
			set => SetProperty(ref toggleLayoutColumns, value);
		}

		private ShortKey toggleLayoutAdaptative = new(VirtualKey.Number6, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);
		public ShortKey ToggleLayoutAdaptative
		{
			get => toggleLayoutAdaptative;
			set => SetProperty(ref toggleLayoutAdaptative, value);
		}

		private ShortKey toggleShowHiddenItems = ShortKey.None;
		public ShortKey ToggleShowHiddenItems
		{
			get => toggleShowHiddenItems;
			set => SetProperty(ref toggleShowHiddenItems, value);
		}

		private ShortKey toggleShowFileExtensions = ShortKey.None;
		public ShortKey ToggleShowFileExtensions
		{
			get => toggleShowFileExtensions;
			set => SetProperty(ref toggleShowFileExtensions, value);
		}
	}
}
