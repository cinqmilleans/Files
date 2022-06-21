﻿using Files.Shared.Extensions;
using System;
using System.Collections.Generic;
using Windows.Storage;

namespace Files.Backend.Filesystem
{
    public static class CommonPaths
    {
        public static readonly string DesktopPath = UserDataPaths.GetDefault().Desktop;

        public static readonly string DownloadsPath = UserDataPaths.GetDefault().Downloads;

        public static readonly string LocalAppDataPath = UserDataPaths.GetDefault().LocalAppData;

        // Currently is the command to open the folder from cmd ("cmd /c start Shell:RecycleBinFolder")
        public static readonly string RecycleBinPath = @"Shell:RecycleBinFolder";

        public static readonly string NetworkFolderPath = @"Shell:NetworkPlacesFolder";

        public static string MyComputerPath = @"Shell:MyComputerFolder";

        public static readonly string TempPath = ApplicationData.Current.LocalSettings.Values.Get("TEMP", "");

        public static readonly string HomePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public static Dictionary<string, string> ShellPlaces = new Dictionary<string, string>() {
            { "::{645FF040-5081-101B-9F08-00AA002F954E}", RecycleBinPath },
            { "::{5E5F29CE-E0A8-49D3-AF32-7A7BDC173478}", "Home".ToLocalized() /*MyComputerPath*/ },
            { "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}", "Home".ToLocalized() /*MyComputerPath*/ },
            { "::{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}", NetworkFolderPath },
            { "::{208D2C60-3AEA-1069-A2D7-08002B30309D}", NetworkFolderPath },
            { RecycleBinPath.ToUpperInvariant(), RecycleBinPath },
            { MyComputerPath.ToUpperInvariant(), "Home".ToLocalized() /*MyComputerPath*/ },
            { NetworkFolderPath.ToUpperInvariant(), NetworkFolderPath },
        };
    }
}
