﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.Commands
{
	public interface IRichCommand : ICommand, INotifyPropertyChanging, INotifyPropertyChanged
	{
		CommandCodes Code { get; }

		string Label { get; }
		string LabelWithHotKey { get; }
		string AutomationName { get; }

		string Description { get; }

		RichGlyph Glyph { get; }
		object? Icon { get; }
		FontIcon? FontIcon { get; }
		Style? OpacityStyle { get; }

		string? HotKeyText { get; }
		HotKeyCollection DefaultHotKeys { get; }
		HotKeyCollection CustomHotKeys { get; set; }

		bool IsToggle { get; }
		bool IsOn { get; set; }
		bool IsExecutable { get; }

		Task ExecuteAsync();
		void ExecuteTapped(object sender, TappedRoutedEventArgs e);
	}
}
