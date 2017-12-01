namespace Shopify.Unity.SDK {
    using System.Text.RegularExpressions;
    using System;

    public class ValidationUtils {
        public static void ValidateAlias(string alias) {
            Regex regex = new Regex("^[_A-Za-z][_0-9A-Za-z]*$");

            if (String.IsNullOrEmpty(alias)) {
                throw new AliasException("`alias` cannot be a blank string");
            } else if (!regex.IsMatch(alias)) {
                throw new AliasException("`alias` was invalid format");
            }
        }
    }
}