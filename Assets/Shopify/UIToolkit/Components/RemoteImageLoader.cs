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

        /// <summary>
        /// When true, downloaded images will be cached. Defaults to true.
        /// </summary>
        public bool UseCache = true;

        private Image _image;

        private delegate void RemoteImageCompletionDelegate(Texture2D texture, string error);

        private WebImageCache _imageCache = WebImageCache.SharedCache;

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
            if (UseCache) {
                StartCoroutine(CachedLoadImageURLRoutine(url, completion));
            } else {
                StartCoroutine(LoadImageURLRoutine(url, completion));
            }
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

        private IEnumerator CachedLoadImageURLRoutine(string url, RemoteImageCompletionDelegate completion) {
            var requestHeaders = new Dictionary<string, string>();
            var cachedResource = _imageCache.TextureResourceForURL(url);
            if (cachedResource != null) {
                requestHeaders["If-Modified-Since"] = cachedResource.LastModified;
            }

            var www = new WWW(url, null, requestHeaders);

            yield return www;

            // Bail out early if we hit an error.
            if (www.error != null) {
                completion(null, www.error);
                yield break;
            }

            // If the image hasn't changed then return the cached version. Otherwise, remove
            // the entry for this URL so we can insert a new image.
            var responseHeaders = www.responseHeaders;
            if (responseHeaders.ContainsKey("STATUS")) {
                string statusCodeLine = responseHeaders["STATUS"];
                if (ParseResponseCode(statusCodeLine) == 304) {
                    if (cachedResource != null) {
                        completion(cachedResource.texture, null);
                    } else {
                        completion(null, "Cached texture is missing for URL: " + url);
                    }

                    yield break;
                }
            } else {
                _imageCache.RemoveKey(url);
            }

            // Cache the image.
            var texture = www.texture;
            string lastModifiedString = responseHeaders.ContainsKey("Last-Modified") ? responseHeaders["Last-Modified"] : null;
            _imageCache.SetTextureForURL(url, texture, lastModifiedString);
            completion(texture, null);
        }

        /// <summary>
        /// Parses the 'Status XXX' HTTP header field for the status code number.
        /// </summary>
        /// <param name="statusLine">HTTP STATUS field</param>
        /// <returns>HTTP Status code</returns>
        private static int ParseResponseCode(string statusLine) {
            int ret = 0;
            string[] components = statusLine.Split(' ');
            int.TryParse(components[1], out ret);
            return ret;
        }

    }
}

