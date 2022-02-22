using Files.Enums;
using Files.Services;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Files.UserControls
{
    public sealed partial class PaneControl : UserControl, IPane
    {
        public event EventHandler ContentUpdated;

        private Control pane;

        public PanePositions Position { get; private set; } = PanePositions.None;

        private IPaneSettingsService PaneService { get; } = Ioc.Default.GetService<IPaneSettingsService>();

        public PaneControl() => InitializeComponent();

        public void UpdatePosition(double panelWidth, double panelHeight)
        {
            if (pane is IPane p)
            {
                p.UpdatePosition(panelWidth, panelHeight);
                Position = p.Position;
            }
            else
            {
                Position = pane is not null ? PanePositions.Right : PanePositions.None;
            }

            if (pane is not null)
            {
                MinWidth = pane.MinWidth;
                MaxWidth = pane.MaxWidth;
                MinHeight = pane.MinHeight;
                MaxHeight = pane.MaxHeight;
            }
            Position = PanePositions.Right;
        }

        private void Pane_Loading(FrameworkElement sender, object args)
        {
            pane = sender as Control;
            ContentUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    public class PaneTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PreviewTemplate { get; set; }
        public DataTemplate SearchTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            PaneContents.Preview => PreviewTemplate,
            PaneContents.Search => SearchTemplate,
            _ => null,
        };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }
}
