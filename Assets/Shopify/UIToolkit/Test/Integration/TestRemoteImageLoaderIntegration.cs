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
                imageURL: "https://cdn.shopify.com/s/files/1/2094/7261/products/product-image-305453751.jpg?v=1497377691", 
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
            var imageURL = "https://cdn.shopify.com/s/files/1/2094/7261/products/product-image-305453751.jpg?v=1497377691";  
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
