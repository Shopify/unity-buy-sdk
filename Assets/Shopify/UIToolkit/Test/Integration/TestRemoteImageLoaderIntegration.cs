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

            loader.ImageURL = "https://cdn.shopify.com/s/files/1/2094/7261/products/product-image-305453751.jpg?v=1497377691";
            loader.LoadImage();

            var image = gameObject.GetComponent<Image>();
            while (image.sprite == null) {
                yield return null;
            }

            IntegrationTest.Assert(image.sprite != null);
            IntegrationTest.Pass();
        }
    }
}
