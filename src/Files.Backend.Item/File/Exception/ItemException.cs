using System;

namespace Files.Backend.Item
{
    public class FileItemException : Exception
    {
        public FileItemErrors Error { get; } = FileItemErrors.Unknown;

        internal FileItemException() { }
        internal FileItemException(string message) : base(message) { }
        internal FileItemException(string message, Exception innerException) : base(message, innerException) { }
        internal FileItemException(FileItemErrors error) => Error = error;
        internal FileItemException(FileItemErrors error, string message) : base(message) => Error = error;
        internal FileItemException(FileItemErrors error, string message, Exception innerException) : base(message, innerException) => Error = error;
    }
}
