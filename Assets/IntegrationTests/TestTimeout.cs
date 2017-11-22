namespace Shopify.Unity.Tests
{
    using UnityEngine;
	using UnityEngine.TestTools;
	using NUnit.Framework;
    using Shopify.Unity.SDK;
	using System.Collections;

    public class TestTimeout : MonoBehaviour {
		[UnityTest]
		public IEnumerator TimeoutHalfSecond() {
            float startTime = Time.time;
			bool didCallCallback = false;

            UnityTimeout.Start(0.5f, () => {
				didCallCallback = true;
                float endTime = Time.time;
                float timeDiff = endTime - startTime;

				Assert.IsTrue(timeDiff >= 0.5f && timeDiff < 0.55f, "Timeout completed in 0.5 seconds");
            });

			yield return new WaitForSeconds (0.6f);

			Assert.IsTrue (didCallCallback);
        }
    }   
}
