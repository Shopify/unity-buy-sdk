namespace Shopify.UIToolkit.Test.Integration {
	using UnityEngine;
    using UnityEngine.TestTools;
    using System.Collections;
    using Shopify.Unity.SDK;

	[IntegrationTest.DynamicTest("UIToolkitIntegrationTests")]
    public class TestRemoteImageLoaderIntegration {

        [UnityTest] 
        public IEnumerator TestDownloadImage() {
            var loader = GlobalGameObject.AddComponent<RemoteImageLoader>();
            UnityImageLoader networkLoader = new UnityImageLoader();
            loader.SetImageLoader(networkLoader);

            string url = "https://cdn.shopify.com/s/files/1/2094/7261/products/product-image-305453751.jpg?v=1497377691";
            yield return loader.DownloadImageURL(url, false);
            IntegrationTest.Assert(loader.texture != null);
            IntegrationTest.Assert(loader.error == null);
            Debug.Log(networkLoader.GetResponseHeaders());


            // Download the resource again to see caching working
            yield return loader.DownloadImageURL(url, false);
            IntegrationTest.Assert(loader.texture != null);
            IntegrationTest.Assert(loader.error == null);

            IntegrationTest.Pass();
        }
    }
}
