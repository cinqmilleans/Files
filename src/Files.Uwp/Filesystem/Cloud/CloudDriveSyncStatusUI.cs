using CommunityToolkit.Mvvm.ComponentModel;
using Files.Shared.Cloud;
using Microsoft.Toolkit.Uwp;

namespace Files.Uwp.Filesystem.Cloud
{
    public class CloudDriveSyncStatusUI : ObservableObject
    {
        public CloudDriveSyncStatus SyncStatus { get; }
        public string Glyph { get; }

        public bool LoadSyncStatus { get; }
        public string SyncStatusString { get; } = "CloudDriveSyncStatus_Unknown".GetLocalized();

        public CloudDriveSyncStatusUI() {}
        private CloudDriveSyncStatusUI(CloudDriveSyncStatus syncStatus) => SyncStatus = syncStatus;
        private CloudDriveSyncStatusUI(CloudDriveSyncStatus syncStatus, string glyph, bool loadSyncStatus, string SyncStatusStringKey)
        {
            SyncStatus = syncStatus;
            Glyph = glyph;
            LoadSyncStatus = loadSyncStatus;
            SyncStatusString = SyncStatusStringKey.GetLocalized();
        }

        public static CloudDriveSyncStatusUI FromCloudDriveSyncStatus(CloudDriveSyncStatus syncStatus) => syncStatus switch
        {
            // File
            CloudDriveSyncStatus.FileOnline
                => new CloudDriveSyncStatusUI(syncStatus, "\uE753", true, "CloudDriveSyncStatus_Online"),
            CloudDriveSyncStatus.FileOffline or CloudDriveSyncStatus.FileOfflinePinned
                => new CloudDriveSyncStatusUI(syncStatus, "\uE73E", true, "CloudDriveSyncStatus_Offline"),
            CloudDriveSyncStatus.FileSync
                => new CloudDriveSyncStatusUI(syncStatus, "\uE895", true, "CloudDriveSyncStatus_Sync"),

            // Folder
            CloudDriveSyncStatus.FolderOnline or CloudDriveSyncStatus.FolderOfflinePartial
                => new CloudDriveSyncStatusUI(syncStatus, "\uE753", true, "CloudDriveSyncStatus_PartialOffline"),
            CloudDriveSyncStatus.FolderOfflineFull or CloudDriveSyncStatus.FolderOfflinePinned or CloudDriveSyncStatus.FolderEmpty
                => new CloudDriveSyncStatusUI(syncStatus, "\uE73E", true, "CloudDriveSyncStatus_Offline"),
            CloudDriveSyncStatus.FolderExcluded
                => new CloudDriveSyncStatusUI(syncStatus, "\uF140", true, "CloudDriveSyncStatus_Excluded"),

            // Unknown
            _ => new CloudDriveSyncStatusUI(syncStatus),
        };
    }
}