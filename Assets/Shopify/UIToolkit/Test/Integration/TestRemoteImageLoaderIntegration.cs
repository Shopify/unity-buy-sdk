namespace Shopify.UIToolkit.Test.Integration {
	using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.TestTools;
    using System.Collections;
    using Shopify.Unity.SDK;

	[IntegrationTest.DynamicTest("UIToolkitIntegrationTests")]
    [IntegrationTest.Timeout(10)]
    public class TestRemoteImageLoaderIntegration {

        [UnityTest] 
        public IEnumerator TestDownloadImage() {
            var gameObject = new GameObject();
            var loader = gameObject.AddComponent<RemoteImageLoader>();

            bool requestFinished = false;

            loader.LoadImage(
                imageURL: "https://cdn.shopify.com/s/files/1/2094/7261/products/product-image-305453751.jpg?v=1497377691", 
                success: () => { requestFinished = true; },
                failure: (e) =>  { requestFinished = true; }
            );

            while (!requestFinished) {
                yield return null;
            }

            var image = gameObject.GetComponent<Image>();
            IntegrationTest.Assert(image.sprite != null);
            IntegrationTest.Pass();
        }
    }
}
