﻿using Windows.Storage;

namespace Files.Backend.Storage
{
    public interface IStorageItemWithPath // TODO: Maybe use here : IStorageItem instead of declaring a variable,
                                          // and keep the Path property for it to override IStorageItem.Path ?
    {
        public string Name { get; }
        public string Path { get; set; }
        public IStorageItem Item { get; set; }
        public FilesystemItemType ItemType { get; }
    }
}