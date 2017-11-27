using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shopify.Unity.Tests {
    public class UnityTestUtils {
        public const float MaxQueryDuration = 1f;
        public static string MaxQueryMessage {
            get { return string.Format("Query failed to complete in {0} second", MaxQueryDuration); }
        }

        public static StoppableWaitForTime GetWaitQuery() {
            return new StoppableWaitForTime (MaxQueryDuration);
        }

        public static List<string> GetImageAliases() {
            return new List<string>() {
                "pico",
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
