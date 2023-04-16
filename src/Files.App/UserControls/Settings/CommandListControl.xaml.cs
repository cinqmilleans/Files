using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Commands;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Files.App.UserControls.Settings
{
	public sealed partial class CommandListControl : UserControl
	{
		private readonly IReadOnlyList<IRichCommand> allCommands;
		private readonly IEqualityComparer<IRichCommand> commandComparer = new CommandComparer();

		private static DependencyProperty SelectedCommandProperty { get; } = DependencyProperty.Register
			(nameof(SelectedCommand), typeof(IRichCommand), typeof(CommandListControl), new(null));

		private static DependencyProperty FilterProperty { get; } = DependencyProperty.Register
			(nameof(Filter), typeof(string), typeof(CommandListControl), new(string.Empty, OnFilterChanged));

		public IRichCommand? SelectedCommand
		{
			get => (IRichCommand?)GetValue(SelectedCommandProperty);
			set => SetValue(SelectedCommandProperty, value);
		}

		public string Filter
		{
			get => (string)GetValue(FilterProperty);
			set => SetValue(FilterProperty, value);
		}

		private CollectionViewSource Items { get; }
		private ObservableCollection<IRichCommand> VisibleCommands { get; }

		public CommandListControl()
		{
			InitializeComponent();

			var commands = Ioc.Default.GetRequiredService<ICommandManager>();
			allCommands = commands
					.Where(command => command.Code is not CommandCodes.None)
					.OrderBy(command => command.Label)
					.ToList()
					.AsReadOnly();

			VisibleCommands = new(allCommands);

			Items = new()
			{
				Source = VisibleCommands
			};
		}

		private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is CommandListControl control)
				control.UpdateVisibleCommands();
		}

		private static bool IsFiltered(string filter, IRichCommand command)
		{
			return command.Label.Contains(filter, StringComparison.OrdinalIgnoreCase);
		}

		private void UpdateVisibleCommands()
		{
			var filter = Filter;
			var news = allCommands.Where(command => IsFiltered(filter, command)).ToList();

			VisibleCommands.Except(news, commandComparer).ToList().ForEach(Remove);
			news.Except(VisibleCommands, commandComparer).ToList().ForEach(Add);

			Items.Source = VisibleCommands.ToList();

			void Remove(IRichCommand oldCommand)
			{
				VisibleCommands.Remove(oldCommand);
			}
			void Add(IRichCommand newCommand)
			{
				var previous = VisibleCommands.LastOrDefault(visiblecommand => visiblecommand.Label.CompareTo(newCommand.Label) < 0);
				int index = previous is null ? 0 : VisibleCommands.IndexOf(previous);
				VisibleCommands.Insert(index, newCommand);
			}
		}

		private class CommandComparer : IEqualityComparer<IRichCommand>
		{
			public int GetHashCode([DisallowNull] IRichCommand command) => command.Code.GetHashCode();
			public bool Equals(IRichCommand? x, IRichCommand? y) => Equals(x?.Code, y?.Code);
		}
	}
}
