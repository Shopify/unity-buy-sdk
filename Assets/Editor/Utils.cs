namespace Shopify.Tests {
    using System;
    using System.Linq;
    using UnityEditor;

    public class Utils {
        public const string TestShopDomain = "graphql.myshopify.com";
        public const string TestAccessToken = "351c122017d0f2a957d32ae728ad749c";

        public static bool DoesExist(string className) {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.Name == className
                    select type).Any();
        }
    }
}
