namespace Shopify.Unity.SDK {
    using UnityEngine;

    public class Log {
        public static void DeprecatedQueryField(string typeName, string fieldName, string deprecationReason) {
            Debug.LogWarning("The field `" + typeName + "." + fieldName + "` is deprecated with the following deprecation message:\n" + deprecationReason);
        }
    }
}
