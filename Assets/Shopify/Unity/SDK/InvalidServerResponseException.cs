namespace Shopify.Unity.SDK {
    using System;

    /// <summary>
    /// This exception is thrown whenenver the server responds with an unexpected response which cannot be handled gracefully.
    /// </summary>
    public class InvalidServerResponseException : Exception {
        public InvalidServerResponseException(string message) : base(message) { }
    }
}