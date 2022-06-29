using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Files.Backend.Filesystem.Storage
{
    public class NonSeekableRandomAccessStreamForRead : IRandomAccessStream
    {
        private readonly Stream stream;
        private readonly IRandomAccessStream imrac;
        private ulong virtualPosition;
        private ulong readToByte;
        private readonly ulong byteSize;

        public bool CanRead => true;
        public bool CanWrite => false;

        public ulong Position => virtualPosition;

        public ulong Size
        {
            get => byteSize;
            set => throw new NotSupportedException();
        }

        public Action DisposeCallback { get; set; }

        public NonSeekableRandomAccessStreamForRead(Stream baseStream, ulong size)
        {
            stream = baseStream;
            imrac = new InMemoryRandomAccessStream();
            virtualPosition = 0;
            readToByte = 0;
            byteSize = size;
        }

        public IRandomAccessStream CloneStream() => throw new NotSupportedException();

        public IInputStream GetInputStreamAt(ulong position)
        {
            Seek(position);
            return this;
        }
        public IOutputStream GetOutputStreamAt(ulong position)
            => throw new NotSupportedException();

        public void Seek(ulong position)
        {
            imrac.Size = Math.Max(imrac.Size, position);
            virtualPosition = position;
        }

        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            return AsyncInfo.Run((Func<CancellationToken, IProgress<uint>, Task<IBuffer>>)taskProvider);

            async Task<IBuffer> taskProvider(CancellationToken token, IProgress<uint> progress)
            {
                int read;
                var tempBuffer = new byte[16384];

                imrac.Seek(readToByte);
                while (imrac.Position < virtualPosition + count)
                {
                    read = await stream.ReadAsync(tempBuffer, 0, tempBuffer.Length);
                    if (read is 0)
                    {
                        break;
                    }
                    await imrac.WriteAsync(tempBuffer.AsBuffer(0, read));
                }
                readToByte = imrac.Position;

                imrac.Seek(virtualPosition);
                var res = await imrac.ReadAsync(buffer, count, options);
                virtualPosition = imrac.Position;
                return res;
            }
        }
        public IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer) => throw new NotSupportedException();

        public IAsyncOperation<bool> FlushAsync() => imrac.FlushAsync();

        public void Dispose()
        {
            stream.Dispose();
            imrac.Dispose();
            DisposeCallback?.Invoke();
        }
    }
}
