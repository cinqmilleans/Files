using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Commands;
using Files.App.Contexts;
using Files.App.Views;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using Files.App.ViewModels;
using Files.App.UserControls.MultitaskingControl;

namespace Files.App.Actions
{
	internal abstract class ToggleLayoutAction : ObservableObject, IToggleAction
	{
		protected IDisplayPageContext Context { get; } = Ioc.Default.GetRequiredService<IDisplayPageContext>();

		protected abstract LayoutTypes LayoutType { get; }

		public abstract string Label { get; }

		public abstract RichGlyph Glyph { get; }
		public abstract HotKey HotKey { get; }

		private bool isOn;
		public bool IsOn => isOn;

		public virtual bool IsExecutable => true;

		public ToggleLayoutAction()
		{
			isOn = Context.LayoutType == LayoutType;
			Context.PropertyChanged += Context_PropertyChanged;
		}

		public Task ExecuteAsync()
		{
			//var a = PaneHolderPage.Instances;
			//var d = a.Where(i => i.IsCurrentInstance).ToList();
			//var c2 = a.Where(i => i.ActivePaneOrColumn.IsPageMainPane).ToList();
			//var d2 = a.Where(i => i.ActivePaneOrColumn.IsCurrentInstance).ToList();
			//int n = c.Count;
			//int m = d.Count;
			//int n2 = c2.Count;
			//int m2 = d2.Count;

			//var instance = MainPageViewModel.AppInstances.FirstOrDefault(x => x.Control.TabItemContent.IsCurrentInstance);
			//if (instance is not null)
			//{
			//	var page = (instance.Control.TabItemContent as PaneHolderPage)?.ActivePaneOrColumn;
			//}

			Context.LayoutType = LayoutType;
			return Task.CompletedTask;
		}

		private void Context_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(IDisplayPageContext.LayoutType))
				SetProperty(ref isOn, Context.LayoutType == LayoutType, nameof(IsOn));

			if (e.PropertyName is not null)
				OnContextChanged(e.PropertyName);
		}

		protected virtual void OnContextChanged(string propertyName) {}
	}
}
