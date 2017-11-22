namespace Shopify.UIToolkit.Test.Unit {
	using NUnit.Framework;
    using UnityEngine.TestTools;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Shopify.Unity.SDK;

    // A network-less loader for unit testing/mocking purposes.
    class MockImageLoader : ImageLoader {
        public string mockError;
        public Texture2D mockTexture;

        public string GetError() {
            return mockError;
        }

        public Texture2D GetTexture() {
            return mockTexture;
        }

        public IEnumerator Load(string url) {
            yield return null;
        }
    }

    [TestFixture]
    public class TestRemoteImageLoader {
        [UnityTest]
        public IEnumerator TestLoadImage() {
            var loader = GlobalGameObject.AddComponent<RemoteImageLoader>();
            Texture2D mockTexture = new Texture2D(100, 100);
            MockImageLoader mockLoader = new MockImageLoader();
            mockLoader.mockTexture = mockTexture;
            loader.SetImageLoader(mockLoader);

            Texture2D downloadedTexture = null;
            loader.LoadImageURL("http://notareal.url", (texture, error) => {
                downloadedTexture = texture;
            });

            while (downloadedTexture == null) {
                yield return null;
            }

            Assert.IsNotNull(downloadedTexture);
        }

        [UnityTest]
        public IEnumerator TestLoadImageWithError() {
            var loader = GlobalGameObject.AddComponent<RemoteImageLoader>();
            MockImageLoader mockLoader = new MockImageLoader();
            mockLoader.mockError = "Test error";
            loader.SetImageLoader(mockLoader);

            string returnedError = null;
            loader.LoadImageURL("http://notareal.url", (texture, error) => {
                returnedError = error;
            });

            while (returnedError == null) {
                yield return null;
            }

            Assert.IsNotNull(returnedError);
        }
    }
}
