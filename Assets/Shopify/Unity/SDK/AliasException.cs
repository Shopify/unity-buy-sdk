namespace Shopify.Unity.SDK {
    using System;

    /// <summary>
    /// This exception is thrown when trying to access a response field using an alias which was not used to build the query.
    /// <see ref="AliasException">AliasException </see> is also thrown when the alias format is invalid for instance a blank string 
    /// or the alias uses invalid characters.
    /// </summary>
    public class AliasException : Exception {
        public AliasException(string message) : base(message) { }
    }
}