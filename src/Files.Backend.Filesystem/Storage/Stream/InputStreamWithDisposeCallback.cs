using System;
using System.IO;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Files.Backend.Filesystem.Storage
{
    public class InputStreamWithDisposeCallback : IInputStream
    {
        private readonly Stream stream;
        private readonly IInputStream inputStream;

        public Action DisposeCallback { get; set; }

        public InputStreamWithDisposeCallback(Stream stream)
        {
            this.stream = stream;
            inputStream = stream.AsInputStream();
        }

        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
            => inputStream.ReadAsync(buffer, count, options);

        public void Dispose()
        {
            inputStream.Dispose();
            stream.Dispose();
            DisposeCallback?.Invoke();
        }
    }
}
