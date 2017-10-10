using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Helpers {
    public static class ImageHelper {
        private static readonly Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();

        // Takes an image url, handles fetching the image and then assigning the retrieved texture to the image.
        // A cache is used to prevent re-requesting images.
        public static IEnumerator AssignImage(string url, Image image, Image brokenImage = null) {
            // If the url isn't in the cache, then we need to make the web request to fetch the image
            if (!TextureCache.ContainsKey(url)) {
                var www = new WWW(url);

                yield return www;

                if (!string.IsNullOrEmpty(www.error)) {
                    Debug.Log(www.error);

                    if (brokenImage != null) brokenImage.gameObject.SetActive(true);

                    yield break;
                }

                // Once the web request is done, assign the texture into the cache 
                TextureCache[url] = www.texture;
            }

            // As we have already ensured that the url is in the cache, we can now safely pull the texture from the cache
            var texture = TextureCache[url];

            // Turn the texture into a srpite, and assign it to the target image
            image.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect
            );

            image.preserveAspect = true;

            image.gameObject.SetActive(true);
            if (brokenImage != null) brokenImage.gameObject.SetActive(false);
        }
    }
}