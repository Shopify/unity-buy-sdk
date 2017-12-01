namespace Shopify.Unity.SDK {
    using System.Collections.Specialized;
    using System.Collections;
    using System.Text;
    using System;
    using MiniJSON;

    /// <summary>
    /// Converts C# values to GraphQL strings for queries.
    /// </summary>
    public class InputValueToString {
        /// <summary>
        /// Returns a string representing a GraphQL list from a C# iteratable object implementing IList.
        /// </summary>
        /// <param name="value">a C# IList which will be converted to a GraphQL list string</param>
        public static string GetList(IList value) {
            StringBuilder arrayString = new StringBuilder();

            arrayString.Append("[");

            bool isNotFirstListItem = false;

            foreach (object listValue in value) {
                if (isNotFirstListItem) {
                    arrayString.Append(",");
                }

                arrayString.Append(Get(listValue));
                isNotFirstListItem = true;
            }

            arrayString.Append("]");

            return arrayString.ToString();
        }

        /// <summary>
        /// Converts a C# value/object to a GraphQL string value/object.
        /// </summary>
        /// <param name="value">a C# object to be converted to a GraphQL string value</param>
        public static string Get(object value) {
            if (value is IList) {
                return GetList((IList) value);
            } else if (
                value is string ||
                value is bool ||
                value is long ||
                value is double
            ) {
                return Json.Serialize(value);
            } else {
                return value.ToString();
            }
        }
    }
}