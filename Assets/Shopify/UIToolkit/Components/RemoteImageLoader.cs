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
        private Image _image;

        private delegate void RemoteImageCompletionDelegate(Texture2D texture, string error);

        void Start() {
            _image = gameObject.GetComponent<Image>();
        } 

        /// <summary>
        /// Downloads the image located at the given ImageURL. The resulting image will be assigned to 
        /// the GameObject's Image component.
        /// </summary>
        /// <param name="imageURL">URL for the image to download.</param>
        public void LoadImage(string imageURL) {
            LoadImage(imageURL, null, null);
        }

        /// <summary>
        /// Downloads the image located at the given ImageURL. The resulting image will be assigned to 
        /// the GameObject's Image component.
        /// </summary>
        /// <param name="imageURL">URL for the image to download.</param>
        /// <param name="success">Callback called when image was successfully downloaded and assigned.</param>
        /// <param name="failure">Callback called when an error occurs.</param>
        public void LoadImage(string imageURL, Action success, Action<string> failure) {
            LoadImageURL(imageURL, (texture, error) => {
                if (error != null) {
                    Debug.LogWarning("Failed to download image at " + imageURL + " Reason: " + error);
                    failure(error);
                    return;
                }

                if (texture == null) {
                    Debug.LogWarning("Failed to generate texture from image located at " + imageURL);
                    failure("Texture not found");
                    return;
                }

                _image.sprite = SpriteFromTexture(texture);
                _image.preserveAspect = true;

                success();
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

        private void LoadImageURL(string url, RemoteImageCompletionDelegate completion) {
            StartCoroutine(LoadImageURLRoutine(url, completion));
        }

        private IEnumerator LoadImageURLRoutine(string url, RemoteImageCompletionDelegate completion) {
            var www = new WWW(url);

            yield return www;

            // Bail out early if we hit an error.
            if (www.error != null) {
                completion(null, www.error);
                yield break;
            }

            completion(www.texture, null);
        }
    }
}

