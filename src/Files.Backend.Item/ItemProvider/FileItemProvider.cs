namespace Files.Backend.Item
{
    /*public interface IFileItemProvider : IItemProvider
    {
        FileItemProviderOptions Option { get; }

        new IAsyncEnumerable<IFileItem> ProvideItems();
    }

    [Flags]
    public enum FileItemProviderOptions : ushort
    {
        None,
        IncludeHiddenItems,
        IncludeSystemItems,
        IncludeUnindexedItems,
    }

    public interface IBuilder<T>
    {
        T Build();
    }

    public interface IFileItemProviderBuilder : IBuilder<IFileItemProvider>
    {
        IFileItemProviderBuilder IncludeUnindexedItems(bool includeUnindexedItems = true);
        IFileItemProviderBuilder IncludeSystemItems(bool includeSystemItems = true);
        IFileItemProviderBuilder IncludeHiddenItems(bool includeHiddenItems = true);


        IFileItemProviderBuilder WithLogger(Logger logger);
    }

    public class FileItemProviderBuilder : IFileItemProviderBuilder
    {
        private FileItemProviderOptions option = FileItemProviderOptions.None;

        private Logger logger;

        public IFileItemProvider Build()
        {
            return null;
        }

        public IFileItemProviderBuilder IncludeUnindexedItems(bool includeUnindexedItems = true)
            => SetOption(FileItemProviderOptions.IncludeUnindexedItems, includeUnindexedItems);

        public IFileItemProviderBuilder IncludeSystemItems(bool includeSystemItems = true)
            => SetOption(FileItemProviderOptions.IncludeSystemItems, includeSystemItems);

        public IFileItemProviderBuilder IncludeHiddenItems(bool includeHiddenItems = true)
            => SetOption(FileItemProviderOptions.IncludeHiddenItems, includeHiddenItems);

        public IFileItemProviderBuilder WithLogger(Logger logger)
        {
            this.logger = logger;
            return this;
        }

        private FileItemProviderBuilder SetOption(FileItemProviderOptions option, bool useOption)
        {
            this.option &= useOption ? option : ~option;
            return this;
        }
    }


    public class FileItemProvider : IFileItemProvider
    {
        public string ParentPath { get; set; } = string.Empty;

        public System.Threading.CancellationToken CancellationToken { get; set; }

        public bool IncludeHiddens { get; set; } = false;
        public bool IncludeSystems { get; set; } = false;

        public bool ShowFolderSize { get; set; } = false;

        IAsyncEnumerable<IItem> IItemProvider.ProvideItems() => ProvideItems();
        public async IAsyncEnumerable<IFileItem> ProvideItems()
        {
            await Task.Delay(1);
            yield return null;
        }

        private IFileItem BuildFileItem(string path, WIN32_FIND_DATA data)
        {
            return new FileItem
            {
                Path = path,
                Name = data.cFileName,
                FileAttribute = ((IO.FileAttributes)data.dwFileAttributes).ToFileAttribute(),
                Size = data.GetSize(),
                DateCreated = ToDateTime(ref data.ftCreationTime),
                DateModified = ToDateTime(ref data.ftLastWriteTime),
                DateAccessed = ToDateTime(ref data.ftLastAccessTime),
            };
        }
        private IFileItem BuildShortcutItem(string path, WIN32_FIND_DATA data)
        {
            return new ShortcutItem
            {
                Path = path,
                Name = data.cFileName,
                FileAttribute = ((IO.FileAttributes)data.dwFileAttributes).ToFileAttribute(),
                Size = data.GetSize(),
                DateCreated = ToDateTime(ref data.ftCreationTime),
                DateModified = ToDateTime(ref data.ftLastWriteTime),
                DateAccessed = ToDateTime(ref data.ftLastAccessTime),
            };
        }
    }*/
}
