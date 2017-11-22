namespace Shopify.UIToolkit {
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface ImageLoader {
        string GetError();

        Texture2D GetTexture();

        IEnumerator Load(string url);
    }

    public class UnityImageLoader : ImageLoader {
        private WWW _www;

        public string GetError() {
            return _www.error;
        }

        public Texture2D GetTexture() {
            return _www.texture;
        }

        public IEnumerator Load(string url) {
            _www = new WWW(url);
            yield return _www;
        }
    }

    /// <summary>
    /// Completion callback that returns the downloaded texture and error description from a load operation.
    /// </summary>
    /// <param name="texture">Downloaded image as a Texture2D</param>
    /// <param name="error">Error description</param>
    public delegate void RemoteImageCompletionDelegate(Texture2D texture, string error);

    /// <summary>
    /// A behaviour that fetches a remote image (JPEG/PNG) from the web.
    /// </summary>
    public class RemoteImageLoader : MonoBehaviour {

        private ImageLoader _loader;

        public void SetImageLoader(ImageLoader loader) {
            _loader = loader;
        }

        /// <summary>
        /// Downloads the given web image located at the URL.
        /// </summary>
        /// <param name="url">URL of the image resource to download and cache.</param>
        /// <param name="completion">Callback called when the download is complete.</param>
        public void LoadImageURL(string url, RemoteImageCompletionDelegate completion) {
            StartCoroutine(LoadImageURLRoutine(url, completion));
        }

        private IEnumerator LoadImageURLRoutine(string url, RemoteImageCompletionDelegate completion) {
            _loader = _loader != null ? _loader : new UnityImageLoader();

            yield return _loader.Load(url);

            // Bail out early if we hit an error.
            if (_loader.GetError() != null) {
                completion(null, _loader.GetError());
                yield return null;
            }

            Texture2D downloadedTexure = _loader.GetTexture();
            completion(downloadedTexure, null);
            yield return null;
        }
    }
}

