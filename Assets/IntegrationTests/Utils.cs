using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shopify.Unity.Tests {
	public class Utils {
		public const float MaxQueryDuration = 1f;
		public const string MaxQueryMessage = "Query finished in 1 seconds";

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
