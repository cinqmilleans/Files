using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Commands;
using Files.App.DataModels;
using Files.App.Extensions;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Files.App.Actions
{
	internal class MultiSelectAction : ObservableObject, IToggleAction
	{
		private readonly ICommandContext context = Ioc.Default.GetRequiredService<ICommandContext>();

		public CommandCodes Code => CommandCodes.MultiSelect;
		public string Label => "NavToolbarMultiselect/Text".GetLocalizedResource();

		public IGlyph Glyph = new Glyph("\uE762");

		public bool IsOn
		{
			get => context?.AppModel?.MultiselectEnabled ?? false;
			set
			{
				if (context?.AppModel is AppModel model)
					model.MultiselectEnabled = value;
			}
		}

		public MultiSelectAction()
		{
			if (context?.AppModel is AppModel model)
				model.PropertyChanged += Model_PropertyChanged;
		}

		public Task ExecuteAsync()
		{
			if (context.AppModel is AppModel model)
				model.MultiselectEnabled = !model.MultiselectEnabled;

			return Task.CompletedTask;
		}

		private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(AppModel.MultiselectEnabled))
				OnPropertyChanged(nameof(IsOn));

		}
	}
}
