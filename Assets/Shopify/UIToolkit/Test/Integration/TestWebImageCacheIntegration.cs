namespace Shopify.UIToolkit.Test.Integration {
    using System;
    using System.Collections;
    using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.TestTools;
    using Shopify.UIToolkit;
	using NUnit.Framework;

    public class TestWebImageCacheIntegration: MonoBehaviour {
        private List<string> TenMBInImages = new List<string>() {
            "https://cdn.shopify.com/s/files/1/2094/7261/products/product-image-270658459.jpg?v=1497377685",
            "https://cdn.shopify.com/s/files/1/2094/7261/products/product-image-305453751.jpg?v=1497377691",
            "https://cdn.shopify.com/s/files/1/2094/7261/products/product-image-158349373.jpg?v=1497377687",
            "https://cdn.shopify.com/s/files/1/2094/7261/products/product-image-272143053.jpg?v=1497377687",
            "https://cdn.shopify.com/s/files/1/2094/7261/products/product-image-272143072.jpg?v=1497377690",
            "https://cdn.shopify.com/s/files/1/2094/7261/products/product-image-275559099.jpg?v=1497377686"
        };

        [TearDown]
        public void Cleanup() {
            WebImageCache.SharedCache.SetMemorySizeLimit(WebImageCache.DEFAULT_MEMORY_SIZE_LIMIT);
        }

		[UnityTest]
        public IEnumerator TestDownloadAFewImagesAndCache() {
            var cache = WebImageCache.SharedCache;

            foreach (var imageURL in TenMBInImages) {
                WWW www = new WWW(imageURL);
                yield return www;
                var texture = www.texture;
                var resource = new CachedWebResource<Texture2D>(DateTime.Now.ToShortTimeString(), texture);
                cache.SetTextureResourceForURL(imageURL, resource);
            }

            Assert.AreEqual(cache.Count, 6);
        }

        [UnityTest]
        public IEnumerator TestDownloadMoreThanFitsCache() {
            var cache = WebImageCache.SharedCache;
            int fiveMBInBytes = 5242880;
            cache.SetMemorySizeLimit(fiveMBInBytes);

            foreach (var imageURL in TenMBInImages) {
                WWW www = new WWW(imageURL);
                yield return www;
                var texture = www.texture;
                var resource = new CachedWebResource<Texture2D>(DateTime.Now.ToShortTimeString(), texture);
                cache.SetTextureResourceForURL(imageURL, resource);
            }

            Assert.AreEqual(cache.Count, 3);
            Assert.IsNull(cache.TextureResourceForURL(TenMBInImages[0]));
            Assert.IsNull(cache.TextureResourceForURL(TenMBInImages[1]));
            Assert.IsNull(cache.TextureResourceForURL(TenMBInImages[2]));
            Assert.NotNull(cache.TextureResourceForURL(TenMBInImages[3]));
            Assert.NotNull(cache.TextureResourceForURL(TenMBInImages[4]));
            Assert.NotNull(cache.TextureResourceForURL(TenMBInImages[5]));
            Debug.Log(cache.EstimatedMemorySize);
        }
    }
}
