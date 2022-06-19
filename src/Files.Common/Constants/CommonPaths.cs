using Files.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Windows.Storage;

namespace Files.Common.Constant
{
    public static class CommonPaths
    {
        public const string MyComputerPath = @"Shell:MyComputerFolder";
        public const string NetworkFolderPath = @"Shell:NetworkPlacesFolder";
        public const string RecycleBinPath = @"Shell:RecycleBinFolder";

        public static readonly string DesktopPath = UserDataPaths.GetDefault().Desktop;
        public static readonly string DownloadsPath = UserDataPaths.GetDefault().Downloads;
        public static readonly string LocalAppDataPath = UserDataPaths.GetDefault().LocalAppData;

        public static readonly string HomePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static readonly string TempPath = ApplicationData.Current.LocalSettings.Values.Get("TEMP", string.Empty);

        public static IImmutableDictionary<string, string> ShellPlaces { get; } = new Dictionary<string, string>
        {
            ["::{645FF040-5081-101B-9F08-00AA002F954E}"] = RecycleBinPath,
            ["::{5E5F29CE-E0A8-49D3-AF32-7A7BDC173478}"] = "Home".ToLocalized(),
            ["::{20D04FE0-3AEA-1069-A2D8-08002B30309D}"] = "Home".ToLocalized(),
            ["::{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}"] = NetworkFolderPath,
            ["::{208D2C60-3AEA-1069-A2D7-08002B30309D}"] = NetworkFolderPath,
            [RecycleBinPath.ToUpperInvariant()] = RecycleBinPath,
            [MyComputerPath.ToUpperInvariant()] = "Home".ToLocalized(),
            [NetworkFolderPath.ToUpperInvariant()] = NetworkFolderPath,
        }.ToImmutableDictionary();
    }
}
