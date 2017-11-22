namespace Shopify.UIToolkit.Test.Unit {
	using NUnit.Framework;
    using UnityEngine.TestTools;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    // A network-less loader for unit testing/mocking purposes.
    class MockImageLoader : ImageLoader {
        public string mockError;
        public Texture2D mockTexture;
        public Dictionary<string, string> mockResponseHeaders;

        public string GetError() {
            return mockError;
        }

        public Texture2D GetTexture() {
            return mockTexture;
        }

        public Dictionary<string, string> GetResponseHeaders() {
            return mockResponseHeaders;
        }

        public IEnumerator Load(string url, Dictionary<string, string> headers) {
            yield return null;
        }
    }

    [TestFixture]
    public class TestRemoteImageLoader {

            // [UnityTest]
            // public IEnumerator TestDownloadImage() {
            //     RemoteImageLoader loader = new RemoteImageLoader();
            //     Texture2D mockTexture = new Texture2D(100, 100);
            //     MockImageLoader mockLoader = new MockImageLoader();
            //     mockLoader.mockTexture = mockTexture;

            //     loader.SetImageLoader(mockLoader);
            //     loader.DownloadImageURL("http://notareal.url", false);
            //     Assert.IsNull(loader.error);
            //     Assert.IsNotNull(loader.texture);
            //     yield return null;
            // }
    }
}
