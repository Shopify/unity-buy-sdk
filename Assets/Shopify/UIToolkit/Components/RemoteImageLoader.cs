namespace Shopify.UIToolkit {
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface ImageLoader {
        string GetError();

        Texture2D GetTexture();

        Dictionary<string, string> GetResponseHeaders();

        IEnumerator Load(string url, Dictionary<string, string> headers);
    }

    public class UnityImageLoader : ImageLoader {
        private WWW _www;

        public string GetError() {
            return _www.error;
        }

        public Texture2D GetTexture() {
            return _www.texture;
        }

        public Dictionary<string, string> GetResponseHeaders() {
            return _www.responseHeaders;
        }

        public IEnumerator Load(string url, Dictionary<string, string> headers) {
            _www = new WWW(url, null, headers);
            yield return _www;
        }
    }

    public delegate void RemoteImageCompletionDelegate(Texture2D texture, string error);

    /// <summary>
    /// A behaviour that fetches and caches a remote image (JPEG/PNG) from the web.
    /// </summary>
    public class RemoteImageLoader : MonoBehaviour {

        private static class HTTPHeaderFields {
            public const string LAST_MODIFIED = "Last-Modified";
            public const string STATUS_CODE = "STATUS";
            public const string IF_MODIFIED_SINCE = "If-Modified-Since";
        }

        private const int HTTP_STATUS_NOT_MODIFIED = 304;

        private ImageLoader _loader;

        public void SetImageLoader(ImageLoader loader) {
            _loader = loader;
        }

        /// <summary>
        /// Downloads the given web image located at the URL.
        /// </summary>
        /// <param name="url">URL of the image resource to download and cache.</param>
        /// <param name="cache">Determine if the downloaded resource should be cached or not.</param>
        /// <returns>On completion, check texture/error properties for result.</returns>

        public void LoadImageURL(string url, RemoteImageCompletionDelegate completion, bool cache = true) {
            StartCoroutine(LoadImageURLRoutine(url, completion, cache));
        }

        private IEnumerator LoadImageURLRoutine(string url, RemoteImageCompletionDelegate completion, bool cache) {
            _loader = _loader != null ? _loader : new UnityImageLoader();
            var imageCache = WebImageCache.SharedCache;

            // If we have a cached texture for this URL, grab it's Last-Modified timestamp to see if we need to invalidate.
            var requestHeaders = new Dictionary<string, string>();
            CachedWebResource<Texture2D>? cachedResource = imageCache.TextureResourceForURL(url);
            if (cachedResource != null) {
                requestHeaders[HTTPHeaderFields.IF_MODIFIED_SINCE] = cachedResource.Value.LastModifiedTimestamp;
            }

            yield return _loader.Load(url, requestHeaders);

            // Bail out early if we hit an error.
            if (_loader.GetError() != null) {
                completion(null, _loader.GetError());
                yield return null;
            }

            var responseHeaders = _loader.GetResponseHeaders();

            // Check to see if we got a 304 Not Modified response. In that case, just return the cached image.
            if (responseHeaders.ContainsKey(HTTPHeaderFields.STATUS_CODE)) {
                string statusCodeLine = responseHeaders[HTTPHeaderFields.STATUS_CODE];
                if (ParseResponseCode(statusCodeLine) == HTTP_STATUS_NOT_MODIFIED) {
                    CachedWebResource<Texture2D>? resource = imageCache.TextureResourceForURL(url);
                    if (resource != null) {
                        completion(resource.Value.Data, null);
                    } else {
                        completion(null, "Cached texture is missing for URL: " + url);
                    }
                    yield return null;
                }
            } else {
                // Otherwise, clear out the existing cached texture for this URL in prepration for replacing.
                imageCache.RemoveURL(url);
            }

            // Store Last Modified timestamp alongside texture into our cache.
            Texture2D downloadedTexure = _loader.GetTexture();
            string lastModified = responseHeaders.ContainsKey(HTTPHeaderFields.LAST_MODIFIED) ? responseHeaders[HTTPHeaderFields.LAST_MODIFIED] : null;
            CachedWebResource<Texture2D> textureResource = new CachedWebResource<Texture2D>(lastModified, downloadedTexure);
            imageCache.SetTextureResourceForURL(url, textureResource);
            completion(downloadedTexure, null);
            yield return null;
        }

        private static int ParseResponseCode(string statusLine) {
            int ret = 0;
            string[] components = statusLine.Split(' ');
            int.TryParse(components[1], out ret);
            return ret;
        }
    }
}

