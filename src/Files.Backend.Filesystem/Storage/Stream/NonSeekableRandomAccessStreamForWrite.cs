using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Files.Backend.Filesystem.Storage
{
    public class NonSeekableRandomAccessStreamForWrite : IRandomAccessStream
    {
        private readonly Stream stream;
        private readonly IOutputStream outputStream;
        private readonly IRandomAccessStream imrac;
        private ulong byteSize;
        private bool isWritten;

        public bool CanRead => false;
        public bool CanWrite => true;

        public ulong Position => byteSize;

        public ulong Size
        {
            get => byteSize;
            set => throw new NotSupportedException();
        }

        public Action DisposeCallback { get; set; }

        public NonSeekableRandomAccessStreamForWrite(Stream stream)
        {
            this.stream = stream;
            outputStream = stream.AsOutputStream();
            imrac = new InMemoryRandomAccessStream();
        }

        public IRandomAccessStream CloneStream() => throw new NotSupportedException();

        public IInputStream GetInputStreamAt(ulong position) => throw new NotSupportedException();
        public IOutputStream GetOutputStreamAt(ulong position) => position is 0 ? this : throw new NotSupportedException();

        public void Seek(ulong position)
        {
            if (position != 0)
            {
                throw new NotSupportedException();
            }
        }

        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
            => throw new NotSupportedException();
        public IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
        {
            return AsyncInfo.Run((Func<CancellationToken, IProgress<uint>, Task<uint>>)taskProvider);

            async Task<uint> taskProvider(CancellationToken token, IProgress<uint> progress)
            {
                var res = await outputStream.WriteAsync(buffer);
                byteSize += res;
                return res;
            }
        }

        public IAsyncOperation<bool> FlushAsync()
        {
            if (isWritten)
            {
                return imrac.FlushAsync();
            }

            isWritten = true;

            return AsyncInfo.Run(async (cancellationToken) =>
            {
                await stream.FlushAsync();
                return true;
            });
        }

        public void Dispose()
        {
            outputStream.Dispose();
            stream.Dispose();
            imrac.Dispose();
            DisposeCallback?.Invoke();
        }
    }
}
