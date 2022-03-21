using System;

namespace Files.Backend.Item
{
    public class ItemException : Exception
    {
        public ItemErrors Error { get; } = ItemErrors.Unknown;

        internal ItemException() { }
        internal ItemException(string message) : base(message) { }
        internal ItemException(string message, Exception innerException) : base(message, innerException) { }
        internal ItemException(ItemErrors error) => Error = error;
        internal ItemException(ItemErrors error, string message) : base(message) => Error = error;
        internal ItemException(ItemErrors error, string message, Exception innerException) : base(message, innerException) => Error = error;
    }
}
