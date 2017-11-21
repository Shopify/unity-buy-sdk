#if !SHOPIFY_MONO_UNIT_TEST
namespace Shopify.UIToolkit.Test.Unit {
    using Shopify.UIToolkit;
	using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;

    [TestFixture]
    public class TestImageCache {
        [TearDown]
        public void Cleanup() {
            WebImageCache.SharedCache.Clear();
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
            var mockResource = new CachedWebResource<Texture2D>("0", mockTexture);

            cache.SetTextureResourceForURL(mockURL, mockResource);

            Assert.NotNull(cache.TextureResourceForURL(mockURL));
            Assert.AreEqual(cache.EstimatedMemorySize, 40000);
        }

        [Test]
        public void TestRemoveTextureForURL() {
            WebImageCache cache = WebImageCache.SharedCache;
            string mockURL = "myimage.com/image";
            Texture2D mockTexture = new Texture2D(100, 100);
            var mockResource = new CachedWebResource<Texture2D>("0", mockTexture);

            cache.SetTextureResourceForURL(mockURL, mockResource);

            Assert.NotNull(cache.TextureResourceForURL(mockURL).Value.Data);
            Assert.AreEqual(cache.EstimatedMemorySize, 40000);

            cache.RemoveURL(mockURL);

            Assert.Null(cache.TextureResourceForURL(mockURL));
            Assert.AreEqual(cache.EstimatedMemorySize, 0);
        }

        public void TestReplaceTextureForURL() {
            WebImageCache cache = new WebImageCache(80000);
            string url = "myimage.com/image";
            var textureA = new CachedWebResource<Texture2D>("0", new Texture2D(100, 100));
            var textureB = new CachedWebResource<Texture2D>("0", new Texture2D(100, 100));

            cache.SetTextureResourceForURL(url, textureA);
            Assert.AreEqual(cache.TextureResourceForURL(url), textureA);
            cache.SetTextureResourceForURL(url, textureB);
            Assert.AreEqual(cache.TextureResourceForURL(url), textureB);

            Assert.AreEqual(cache.EstimatedMemorySize, 40000);
        }

        [Test]
        public void TestSetTextureForURLAtSizeLimit() {
            WebImageCache cache = new WebImageCache(80000);

            string urlA = "myimage.com/imageA";
            string urlB = "myimage.com/imageB";
            string urlC = "myimage.com/imageC";

            var textureA = new CachedWebResource<Texture2D>("0", new Texture2D(100, 100));
            var textureB = new CachedWebResource<Texture2D>("0", new Texture2D(100, 100));
            var textureC = new CachedWebResource<Texture2D>("0", new Texture2D(100, 100));

            cache.SetTextureResourceForURL(urlA, textureA);
            cache.SetTextureResourceForURL(urlB, textureB);

            Assert.NotNull(cache.TextureResourceForURL(urlA));
            Assert.NotNull(cache.TextureResourceForURL(urlB));
            Assert.AreEqual(cache.EstimatedMemorySize, 80000);

            cache.SetTextureResourceForURL(urlC, textureC);

            // Make sure we're still at our limit and that url A got evicted.
            Assert.AreEqual(cache.EstimatedMemorySize, 80000);
            Assert.NotNull(cache.TextureResourceForURL(urlB));
            Assert.NotNull(cache.TextureResourceForURL(urlC));
            Assert.Null(cache.TextureResourceForURL(urlA));
        }
    }
}
#endif
