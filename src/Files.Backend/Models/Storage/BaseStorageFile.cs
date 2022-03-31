using System;
using System.Collections.Generic;
using System.Text;

namespace Files.Backend.Models.Storage
{
    internal abstract class BaseStorageFile : IBaseStorageFile
    {
        public static implicit operator BaseStorageFile(StorageFile value) => value != null ? new SystemStorageFile(value) : null;

        public abstract IAsyncOperation<StorageFile> ToStorageFileAsync();
        public abstract IAsyncOperation<IRandomAccessStream> OpenAsync(FileAccessMode accessMode);
        public abstract IAsyncOperation<StorageStreamTransaction> OpenTransactedWriteAsync();
        public abstract IAsyncOperation<BaseStorageFile> CopyAsync(IStorageFolder destinationFolder);
        public abstract IAsyncOperation<BaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName);
        public abstract IAsyncOperation<BaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option);
        public abstract IAsyncAction CopyAndReplaceAsync(IStorageFile fileToReplace);
        public abstract IAsyncAction MoveAsync(IStorageFolder destinationFolder);
        public abstract IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName);
        public abstract IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option);
        public abstract IAsyncAction MoveAndReplaceAsync(IStorageFile fileToReplace);

        public abstract string ContentType { get; }
        public abstract string FileType { get; }

        public abstract IAsyncAction RenameAsync(string desiredName);
        public abstract IAsyncAction RenameAsync(string desiredName, NameCollisionOption option);
        public abstract IAsyncAction DeleteAsync();
        public abstract IAsyncAction DeleteAsync(StorageDeleteOption option);
        public abstract IAsyncOperation<BaseBasicProperties> GetBasicPropertiesAsync();
        public abstract bool IsOfType(StorageItemTypes type);

        public abstract FileAttributes Attributes { get; }
        public abstract DateTimeOffset DateCreated { get; }
        public abstract string Name { get; }
        public abstract string Path { get; }

        public abstract IAsyncOperation<IRandomAccessStreamWithContentType> OpenReadAsync();
        public abstract IAsyncOperation<IInputStream> OpenSequentialReadAsync();
        public abstract IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode);
        public abstract IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize);
        public abstract IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize, ThumbnailOptions options);

        public abstract string DisplayName { get; }
        public abstract string DisplayType { get; }
        public abstract string FolderRelativeId { get; }
        public abstract IStorageItemExtraProperties Properties { get; }

        IAsyncOperation<StorageFile> IStorageFile.CopyAsync(IStorageFolder destinationFolder)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                return await (await CopyAsync(destinationFolder)).ToStorageFileAsync();
            });
        }

        IAsyncOperation<StorageFile> IStorageFile.CopyAsync(IStorageFolder destinationFolder, string desiredNewName)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                return await (await CopyAsync(destinationFolder, desiredNewName)).ToStorageFileAsync();
            });
        }

        IAsyncOperation<StorageFile> IStorageFile.CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                return await (await CopyAsync(destinationFolder, desiredNewName, option)).ToStorageFileAsync();
            });
        }

        public IAsyncOperation<StorageItemThumbnail> GetScaledImageAsThumbnailAsync(ThumbnailMode mode)
        {
            return Task.FromResult<StorageItemThumbnail>(null).AsAsyncOperation();
        }

        public IAsyncOperation<StorageItemThumbnail> GetScaledImageAsThumbnailAsync(ThumbnailMode mode, uint requestedSize)
        {
            return Task.FromResult<StorageItemThumbnail>(null).AsAsyncOperation();
        }

        public IAsyncOperation<StorageItemThumbnail> GetScaledImageAsThumbnailAsync(ThumbnailMode mode, uint requestedSize, ThumbnailOptions options)
        {
            return Task.FromResult<StorageItemThumbnail>(null).AsAsyncOperation();
        }

        public abstract IAsyncOperation<BaseStorageFolder> GetParentAsync();
        public abstract bool IsEqual(IStorageItem item);

        public StorageProvider Provider => null;

        public bool IsAvailable => true;

        public abstract IAsyncOperation<IRandomAccessStream> OpenAsync(FileAccessMode accessMode, StorageOpenOptions options);
        public abstract IAsyncOperation<StorageStreamTransaction> OpenTransactedWriteAsync(StorageOpenOptions options);

        IAsyncOperation<StorageFolder> IStorageItem2.GetParentAsync()
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                return await (await GetParentAsync()).ToStorageFolderAsync();
            });
        }

        public static IAsyncOperation<BaseStorageFile> GetFileFromPathAsync(string path)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                BaseStorageFile file = null;
                file ??= await ZipStorageFile.FromPathAsync(path);
                file ??= await FtpStorageFile.FromPathAsync(path);
                file ??= await SystemStorageFile.FromPathAsync(path);
                return file;
            });
        }

        IAsyncOperation<BasicProperties> IStorageItem.GetBasicPropertiesAsync()
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                var item = await ToStorageFileAsync();
                return await item.GetBasicPropertiesAsync();
            });
        }

        StorageItemContentProperties IStorageItemProperties.Properties
        {
            get
            {
                if (this is SystemStorageFile sysFile)
                {
                    return sysFile.File.Properties;
                }
                return null;
            }
        }

        public async Task<string> ReadTextAsync(int maxLength = -1)
        {
            using var inputStream = await OpenSequentialReadAsync();
            using var dataReader = new DataReader(inputStream);
            StringBuilder builder = new StringBuilder();
            uint bytesRead, bytesToRead;
            do
            {
                bytesToRead = maxLength < 0 ? 4096 : (uint)Math.Min(maxLength, 4096);
                bytesRead = await dataReader.LoadAsync(bytesToRead);
                builder.Append(dataReader.ReadString(bytesRead));
            } while (bytesRead > 0);
            return builder.ToString();
        }

        public async Task WriteTextAsync(string text)
        {
            using var stream = await OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.AllowOnlyReaders);
            using var outputStream = stream.GetOutputStreamAt(0);
            using var dataWriter = new DataWriter(outputStream);
            dataWriter.WriteString(text);
            await dataWriter.StoreAsync();
            await stream.FlushAsync();
        }

        public async Task WriteBytesAsync(byte[] dataBytes)
        {
            using var stream = await OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.AllowOnlyReaders);
            using var outputStream = stream.GetOutputStreamAt(0);
            using var dataWriter = new DataWriter(outputStream);
            dataWriter.WriteBytes(dataBytes);
            await dataWriter.StoreAsync();
            await stream.FlushAsync();
        }
    }
}
