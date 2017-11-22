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

            Texture2D downloadedTexture = null;
            string returnedError = null;
            loader.LoadImageURL(url, (texture, error) => {
                downloadedTexture = texture;
                returnedError = error;
            });

            while (downloadedTexture == null) {
                yield return null;
            }

            IntegrationTest.Assert(returnedError == null);

            Texture2D cachedTexture = null;
            loader.LoadImageURL(url, (texture, error) => {
                cachedTexture = texture;
                returnedError = error;
            });

            while (cachedTexture == null) {
                yield return null;
            }

            // Download the resource again to see caching working
            IntegrationTest.Assert(cachedTexture == downloadedTexture);
            IntegrationTest.Assert(returnedError == null);

            IntegrationTest.Pass();
        }
    }
}
