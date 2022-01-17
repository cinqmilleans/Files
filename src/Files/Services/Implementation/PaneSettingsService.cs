using Files.EventArguments;
using Files.Models;
using Files.Models.JsonSettings;
using Microsoft.AppCenter.Analytics;
using System;

namespace Files.Services.Implementation
{
    public class PaneSettingsService : BaseObservableJsonSettingsModel, IPaneSettingsService
    {
        public PaneSettingsService(ISettingsSharingContext settingsSharingContext)
            => RegisterSettingsContext(settingsSharingContext);

        public Panes Pane
        {
            get => Get(Panes.None);
            set => Set(value);
        }

        public double SizeHorizontalPx
        {
            get => Get(Math.Min(Math.Max(Get(300d), 50d), 600d));
            set => Set(Math.Max(50d, Math.Min(value, 600d)));
        }

        public double SizeVerticalPx
        {
            get => Get(Math.Min(Math.Max(Get(250d), 50d), 600d));
            set => Set(Math.Max(50d, Math.Min(value, 600d)));
        }

        public double MediaVolume
        {
            get => Math.Min(Math.Max(Get(1d), 0d), 1d);
            set => Set(Math.Max(0d, Math.Min(value, 1d)));
        }

        public bool ShowPreviewOnly
        {
            get => Get(false);
            set => Set(value);
        }

        public void ReportToAppCenter()
            => Analytics.TrackEvent($"{nameof(ShowPreviewOnly)}, {ShowPreviewOnly}");

        public override void RaiseOnSettingChangedEvent(object sender, SettingChangedEventArgs e)
        {
            if (e.settingName == nameof(ShowPreviewOnly))
            {
                Analytics.TrackEvent($"{e.settingName} {e.newValue}");
            }
            base.RaiseOnSettingChangedEvent(sender, e);
        }
    }
}
