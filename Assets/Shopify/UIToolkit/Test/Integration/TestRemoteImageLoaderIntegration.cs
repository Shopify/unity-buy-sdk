#if !UNITY_IOS
namespace Shopify.UIToolkit.Test.Integration {
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.TestTools;
    using Shopify.Unity.SDK;
    using NUnit.Framework;

    public class TestRemoteImageLoaderIntegration : MonoBehaviour {

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
    }
}
#endif
