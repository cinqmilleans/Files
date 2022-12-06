namespace Files.App.CommandActions
{
    public enum CommandCodes
    {
        None,

        // global
        Help,

        // interface
        FullScreen,

        // layout
        LayoutDetails,
        LayoutTiles,
        LayoutGridSmall,
        LayoutGridMedium,
        LayoutGridLarge,
        LayoutColumns,
        LayoutAdaptive,

        // selection
        MultiSelection,
        SelectAll,
        InvertSelection,
        ClearSelection,

        // file
        ShowHiddenItems,
        ShowFileExtensions,

        // item
        Cut,
        Copy,
        Paste,
        Open,
        Rename,
        Properties,
        OpenFolderInNewTab,
        OpenFolderInSecondaryPanel,
        OpenFolderInNewWindow,
    }
}
