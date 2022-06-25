using Windows.Foundation;
using Windows.Storage.Streams;

namespace Files.Backend.Filesystem.Storage
{
    public class StreamWithContentType : IRandomAccessStreamWithContentType
    {
        private readonly IRandomAccessStream stream;

        public bool CanRead => stream.CanRead;
        public bool CanWrite => stream.CanWrite;

        public ulong Position => stream.Position;

        public ulong Size
        {
            get => stream.Size;
            set => stream.Size = value;
        }

        public string ContentType { get; set; } = "application/octet-stream";

        public StreamWithContentType(IRandomAccessStream stream) => this.stream = stream;

        public IRandomAccessStream CloneStream() => stream.CloneStream();

        public IInputStream GetInputStreamAt(ulong position) => stream.GetInputStreamAt(position);
        public IOutputStream GetOutputStreamAt(ulong position) => stream.GetOutputStreamAt(position);

        public void Seek(ulong position) => stream.Seek(position);

        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
            => stream.ReadAsync(buffer, count, options);
        public IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
            => stream.WriteAsync(buffer);

        public IAsyncOperation<bool> FlushAsync() => stream.FlushAsync();

        public void Dispose() => stream.Dispose();
    }
}
