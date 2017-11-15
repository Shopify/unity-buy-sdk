namespace Shopify.UIToolkit.Test.Integration {
	using UnityEngine;

	[IntegrationTest.DynamicTest("UIToolkitIntegrationTests")]
    public class TestSanity : MonoBehaviour {
        public void Start() {
            IntegrationTest.Pass();
        }
    }
}
