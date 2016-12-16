namespace Shopify.Unity {
    using System;

    public class InvalidServerResponseException : Exception {
        public InvalidServerResponseException() : base(GetMessage()) {}

        private static string GetMessage() {
            return String.Format(String.Format("Response JSON did not contain {0} or {1}", TopLevelResponse.KEY_DATA, TopLevelResponse.KEY_ERRORS));
        }
    }
}
