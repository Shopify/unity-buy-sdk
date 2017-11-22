namespace Shopify.UIToolkit {
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A behaviour that fetches a remote image (JPEG/PNG) from the web.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class RemoteImageLoader : MonoBehaviour {

        public string ImageURL;

        private Image _image;

        private delegate void RemoteImageCompletionDelegate(Texture2D texture, string error);

        void Start() {
            _image = gameObject.GetComponent<Image>();
        } 

        public void LoadImage() {
            LoadImageURL(ImageURL, (texture, error) => {
                if (error != null) {
                    Debug.Log("Failed to download image at " + ImageURL + " Reason: " + error);
                    return;
                }

                if (texture == null) {
                    Debug.Log("Failed to generate texture from image located at " + ImageURL);
                    return;
                }

                _image.sprite = SpriteFromTexture(texture);
                _image.preserveAspect = true;
            });
        }

        private Sprite SpriteFromTexture(Texture2D texture) {
            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect
            );
        }

        /// <summary>
        /// Downloads the given web image located at the URL.
        /// </summary>
        /// <param name="url">URL of the image resource to download and cache.</param>
        /// <param name="completion">Callback called when the download is complete.</param>
        void LoadImageURL(string url, RemoteImageCompletionDelegate completion) {
            StartCoroutine(LoadImageURLRoutine(url, completion));
        }

        private IEnumerator LoadImageURLRoutine(string url, RemoteImageCompletionDelegate completion) {
            var www = new WWW(ImageURL);

            yield return www;

            // Bail out early if we hit an error.
            if (www.error != null) {
                completion(null, www.error);
                yield break;
            }

            completion(www.texture, null);
            yield break;
        }
    }
}

