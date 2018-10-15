#if !SHOPIFY_MONO_UNIT_TEST
namespace Shopify.UIToolkit.Test.Unit {
    using System;
    using Shopify.UIToolkit;
	using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;

    [TestFixture]
    public class TestImageCache {
        [TearDown]
        public void Cleanup() {
            WebImageCache.SharedCache.Clear();

            // Reset memory size to default before we close out the test.
            WebImageCache.SharedCache.SetMemorySizeLimit(WebImageCache.DEFAULT_MEMORY_SIZE_LIMIT);
        }

		[Test]
		public void TestInit() {
            WebImageCache cache = WebImageCache.SharedCache;
            Assert.AreEqual(cache.MemorySizeLimit, WebImageCache.DEFAULT_MEMORY_SIZE_LIMIT);
        }

        [Test]
        public void TestSetTextureForURL() {
            WebImageCache cache = WebImageCache.SharedCache;
            string mockURL = "myimage.com/image";
            Texture2D mockTexture = new Texture2D(100, 100);
            cache.SetTextureForURL(mockURL, mockTexture, "");

            Assert.NotNull(cache.TextureResourceForURL(mockURL));
            Assert.AreEqual(cache.EstimatedMemorySize, 40000);
        }

        [Test]
        public void TestRemoveTextureForURL() {
            WebImageCache cache = WebImageCache.SharedCache;
            string mockURL = "myimage.com/image";
            Texture2D mockTexture = new Texture2D(100, 100);
            cache.SetTextureForURL(mockURL, mockTexture, "");

            Assert.NotNull(cache.TextureResourceForURL(mockURL).texture);
            Assert.AreEqual(cache.EstimatedMemorySize, 40000);

            cache.RemoveKey(mockURL);

            Assert.Null(cache.TextureResourceForURL(mockURL));
            Assert.AreEqual(cache.EstimatedMemorySize, 0);
        }

        public void TestReplaceTextureForURL() {
            WebImageCache cache = WebImageCache.SharedCache;
            cache.SetMemorySizeLimit(80000);
            string url = "myimage.com/image";
            var textureA = new Texture2D(100, 100);
            var textureB = new Texture2D(100, 100);

            cache.SetTextureForURL(url, textureA, "");
            Assert.AreEqual(cache.TextureResourceForURL(url).texture, textureA);
            cache.SetTextureForURL(url, textureB, "");
            Assert.AreEqual(cache.TextureResourceForURL(url).texture, textureB);

            Assert.AreEqual(cache.EstimatedMemorySize, 40000);
        }

        [Test]
        public void TestSetTextureForURLAtSizeLimit() {
            WebImageCache cache = WebImageCache.SharedCache;
            cache.SetMemorySizeLimit(80000);

            string urlA = "myimage.com/imageA";
            string urlB = "myimage.com/imageB";
            string urlC = "myimage.com/imageC";

            var textureA = new Texture2D(100, 100);
            var textureB = new Texture2D(100, 100);
            var textureC = new Texture2D(100, 100);

            cache.SetTextureForURL(urlA, textureA, "");
            cache.SetTextureForURL(urlB, textureB, "");

            Assert.NotNull(cache.TextureResourceForURL(urlA));
            Assert.NotNull(cache.TextureResourceForURL(urlB));
            Assert.AreEqual(cache.EstimatedMemorySize, 80000);

            cache.SetTextureForURL(urlC, textureC, "");

            // Make sure we're still at our limit and that url A got evicted.
            Assert.AreEqual(cache.EstimatedMemorySize, 80000);
            Assert.NotNull(cache.TextureResourceForURL(urlB));
            Assert.NotNull(cache.TextureResourceForURL(urlC));
            Assert.Null(cache.TextureResourceForURL(urlA));
        }

        [Test]
        public void TestChangingMemorySizeToLarger() {
            WebImageCache cache = WebImageCache.SharedCache;
            cache.SetMemorySizeLimit(80000);

            string urlA = "myimage.com/imageA";
            string urlB = "myimage.com/imageB";

            var textureA = new Texture2D(100, 100);
            var textureB = new Texture2D(100, 100);

            cache.SetTextureForURL(urlA, textureA, "");
            cache.SetTextureForURL(urlB, textureB, "");

            Assert.NotNull(cache.TextureResourceForURL(urlA));
            Assert.NotNull(cache.TextureResourceForURL(urlB));
            Assert.AreEqual(cache.EstimatedMemorySize, 80000);
            Assert.AreEqual(cache.MemorySizeLimit, 80000);

            cache.SetMemorySizeLimit(100000);

            // Check that nothing changed since we increased the limit.
            Assert.NotNull(cache.TextureResourceForURL(urlA));
            Assert.NotNull(cache.TextureResourceForURL(urlB));
            Assert.AreEqual(cache.EstimatedMemorySize, 80000);
            Assert.AreEqual(cache.MemorySizeLimit, 100000);
        }

        [Test]
        public void TestChangingMemorySizeToSmaller() {
            WebImageCache cache = WebImageCache.SharedCache;
            cache.SetMemorySizeLimit(80000);

            string urlA = "myimage.com/imageA";
            string urlB = "myimage.com/imageB";

            var textureA = new Texture2D(100, 100);
            var textureB = new Texture2D(100, 100);

            cache.SetTextureForURL(urlA, textureA, "");
            cache.SetTextureForURL(urlB, textureB, "");

            Assert.NotNull(cache.TextureResourceForURL(urlA));
            Assert.NotNull(cache.TextureResourceForURL(urlB));
            Assert.AreEqual(cache.EstimatedMemorySize, 80000);
            Assert.AreEqual(cache.MemorySizeLimit, 80000);

            cache.SetMemorySizeLimit(50000);

            // Making the cache smaller by an image should evict the oldest one.
            Assert.IsNull(cache.TextureResourceForURL(urlA));
            Assert.NotNull(cache.TextureResourceForURL(urlB));
            Assert.AreEqual(cache.EstimatedMemorySize, 40000);
            Assert.AreEqual(cache.MemorySizeLimit, 50000);

            cache.SetMemorySizeLimit(0);

            // Making the cache zero should remove anything else!
            Assert.IsNull(cache.TextureResourceForURL(urlA));
            Assert.IsNull(cache.TextureResourceForURL(urlB));
            Assert.AreEqual(cache.EstimatedMemorySize, 0);
            Assert.AreEqual(cache.MemorySizeLimit, 0);
        }

        [Test]
        public void TestClearingCache() {
            WebImageCache cache = WebImageCache.SharedCache;

            string url = "myimage.com/imageA";
            var texture = new Texture2D(100, 100);

            cache.SetTextureForURL(url, texture, "");

            Assert.NotNull(cache.TextureResourceForURL(url));
            Assert.AreEqual(cache.EstimatedMemorySize, 40000);

            cache.Clear();

            Assert.IsNull(cache.TextureResourceForURL(url));
            Assert.AreEqual(cache.EstimatedMemorySize, 0);
        }
    }
}
#endif
