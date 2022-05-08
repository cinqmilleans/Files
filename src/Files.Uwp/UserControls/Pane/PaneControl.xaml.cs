﻿using Files.Shared.Enums;
using Files.Backend.Services.Settings;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Files.Uwp.UserControls
{
    public sealed partial class PaneControl : UserControl, IPane
    {
        private readonly IPaneSettingsService paneService = Ioc.Default.GetService<IPaneSettingsService>();

        private PaneContents content;

        public PanePositions Position => Panel.Content is IPane pane ? pane.Position : PanePositions.Right;

        public PaneControl()
        {
            InitializeComponent();

            paneService.PropertyChanged += PaneService_PropertyChanged;
            Update();
        }

        public void UpdatePosition(double panelWidth, double panelHeight)
        {
            if (Panel.Content is IPane pane)
            {
                pane.UpdatePosition(panelWidth, panelHeight);
            }
            if (Panel.Content is FrameworkElement element)
            {
                MinWidth = element.MinWidth;
                MaxWidth = element.MaxWidth;
                MinHeight = element.MinHeight;
                MaxHeight = element.MaxHeight;
            }
        }

        private void PaneService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(IPaneSettingsService.Content))
            {
                Update();
            }
        }

        private void Update()
        {
            var newContent = paneService.Content;
            if (content != newContent)
            {
                content = newContent;
                Panel.Content = GetPane(content);
            }
        }

        private static Control GetPane(PaneContents content) => content switch
        {
            PaneContents.Preview => new PreviewPane(),
            _ => null,
        };
    }
}
