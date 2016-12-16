namespace Shopify.Unity {
    using System;

    public class NoQueryException : Exception {
        public NoQueryException(string fieldName) : base(GetMessage(fieldName)) {}

        private static string GetMessage(string fieldName) {
            return "It looks like you did not query the field: " + fieldName;
        }
    }
}
