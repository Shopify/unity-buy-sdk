#if !UNITY_IOS
namespace Shopify.UIToolkit.Test.Integration {
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.TestTools;
    using Shopify.Unity.SDK;
    using NUnit.Framework;

    public class TestRemoteImageLoaderIntegration : MonoBehaviour {

        [TearDown]
        public void Cleanup() {
            WebImageCache.SharedCache.Clear();
        }

        [UnityTest] 
        public IEnumerator TestDownloadImage() {
            var gameObject = new GameObject();
            var loader = gameObject.AddComponent<RemoteImageLoader>();

            bool requestFinished = false;

            loader.LoadImage(
                imageURL: "https://cdn.shopify.com/s/files/1/1312/0893/products/003_3e206539-20d3-49c0-8bff-006e449906ca_1024x1024.jpg?v=1491850970",
                success: () => { requestFinished = true; },
                failure: (e) =>  { requestFinished = true; }
            );

            while (!requestFinished) {
                yield return null;
            }

            var image = gameObject.GetComponent<Image>();
            Assert.NotNull(image.sprite);
        }

        [UnityTest] 
        public IEnumerator TestDownloadImageThatsCached() {
            var imageURL = "https://cdn.shopify.com/s/files/1/1312/0893/products/003_3e206539-20d3-49c0-8bff-006e449906ca_1024x1024.jpg?v=1491850970";
            var gameObject = new GameObject();
            var loader = gameObject.AddComponent<RemoteImageLoader>();
            loader.UseCache = true;

            bool requestFinished = false;

            loader.LoadImage(
                imageURL: imageURL,
                success: () => { requestFinished = true; },
                failure: (e) =>  { requestFinished = true; }
            );

            while (!requestFinished) {
                yield return null;
            }

            var image = gameObject.GetComponent<Image>();
            Assert.NotNull(image.sprite);

            var cachedResource = WebImageCache.SharedCache.TextureResourceForURL(imageURL);
            Assert.NotNull(cachedResource);

            requestFinished = false;

            // Load the image again but this time it should be cached.
            loader.LoadImage(
                imageURL: imageURL,
                success: () => { requestFinished = true; },
                failure: (e) =>  { requestFinished = true; }
            );

            var spriteTexture = gameObject.GetComponent<Image>().sprite.texture;
            Assert.AreEqual(spriteTexture, cachedResource.texture);
        }
    }
}
#endif
