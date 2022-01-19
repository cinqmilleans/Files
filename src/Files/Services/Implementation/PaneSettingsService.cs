using Files.EventArguments;
using Files.Models;
using Files.Models.JsonSettings;
using Microsoft.AppCenter.Analytics;
using System;

namespace Files.Services.Implementation
{
    public class PaneSettingsService : BaseObservableJsonSettingsModel, IPaneSettingsService
    {
        public PaneContents Content
        {
            get => Get(PaneContents.None, "PaneContent");
            set => Set(value, "PaneContent");
        }

        public double HorizontalSizePx
        {
            get => Math.Min(Math.Max(Get(300d, "PaneHorizontalSizePx"), 50d), 600d);
            set => Set(Math.Max(50d, Math.Min(value, 600d)), "PaneHorizontalSizePx");
        }
        public double VerticalSizePx
        {
            get => Math.Min(Math.Max(Get(250d, "PaneVerticalSizePx"), 50d), 600d);
            set => Set(Math.Max(50d, Math.Min(value, 600d)), "PaneVerticalSizePx");
        }

        public double MediaVolume
        {
            get => Math.Min(Math.Max(Get(1d, "PaneMediaVolume"), 0d), 1d);
            set => Set(Math.Max(0d, Math.Min(value, 1d)), "PaneMediaVolume");
        }

        public bool ShowPreviewOnly
        {
            get => Get(false);
            set => Set(value);
        }

        public PaneSettingsService(ISettingsSharingContext settingsSharingContext)
            => RegisterSettingsContext(settingsSharingContext);

        public void ReportToAppCenter()
            => Analytics.TrackEvent($"{nameof(ShowPreviewOnly)}, {ShowPreviewOnly}");

        public override void RaiseOnSettingChangedEvent(object sender, SettingChangedEventArgs e)
        {
            if (e.settingName is nameof(ShowPreviewOnly))
            {
                Analytics.TrackEvent($"{e.settingName} {e.newValue}");
            }
            base.RaiseOnSettingChangedEvent(sender, e);
        }

        private void RaiseOnSettingChangedEvent(string propertyName, object newValue)
            => base.RaiseOnSettingChangedEvent(this, new SettingChangedEventArgs(propertyName, newValue));
    }
}
