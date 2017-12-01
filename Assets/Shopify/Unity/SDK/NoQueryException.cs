namespace Shopify.Unity.SDK {
    using System;

    /// <summary>
    /// This exception is thrown when a field was accessed from a query response but the field was not queried.
    /// </summary>
    public class NoQueryException : Exception {
        public NoQueryException(string message) : base(message) { }
        public NoQueryException(string message, Exception innerException) : base(message, innerException) { }
    }
}