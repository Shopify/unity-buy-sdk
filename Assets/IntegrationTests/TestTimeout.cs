namespace Shopify.Unity.Tests
{
    using UnityEngine;
    using Shopify.Unity.SDK;

    [IntegrationTest.DynamicTest ("TestScene")]
    [IntegrationTest.SucceedWithAssertions]
    public class TestTimeout : MonoBehaviour {
        void Start() {
            float startTime = Time.time;

            UnityTimeout.Start(0.5f, () => {
                float endTime = Time.time;
                float timeDiff = endTime - startTime;

                IntegrationTest.Assert(timeDiff >= 0.5f && timeDiff < 0.55f, "Timeout completed in 0.5 seconds");
                IntegrationTest.Pass();
            });
        }
    }   
}
