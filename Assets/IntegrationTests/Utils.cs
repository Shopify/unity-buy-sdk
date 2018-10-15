using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shopify.Unity.Tests {
    public class Utils {
        public const float MaxQueryDuration = 3f;
        public const string MaxQueryMessage = "Query failed to run in 3 seconds";
        public const string TestShopDomain = "graphql.myshopify.com";
        public const string TestAccessToken = "351c122017d0f2a957d32ae728ad749c";

        public static StoppableWaitForTime GetWaitQuery() {
            return new StoppableWaitForTime (MaxQueryDuration);
        }

        public static List<string> GetImageAliases() {
            return new List<string>() {
                "pico",
                "icon",
                "thumb",
                "small",
                "compact",
                "medium",
                "large",
                "grande",
                "resolution_1024",
                "resolution_2048"
            };
        }
    }
}
