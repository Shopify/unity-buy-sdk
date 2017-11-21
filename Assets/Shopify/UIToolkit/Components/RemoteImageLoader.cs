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

    /// <summary>
    /// A behaviour that fetches and caches a remote image (JPEG/PNG) from the web.!-- from the web.
    /// </summary>
    public class RemoteImageLoader : MonoBehaviour {

        private class HTTPHeaderFields {
            public const string LAST_MODIFIED = "Last-Modified";
            public const string STATUS_CODE = "STATUS";
            public const string IF_MODIFIED_SINCE = "If-Modified-Since";
        }

        /// <summary>
        /// The resulting error after downloading a web image.
        /// </summary>
        /// <returns></returns>
        public string error {
            get {
                return _error;
            }
        }

        /// <summary>
        /// The resulting Texture2D web image after downloading from the given URL.
        /// </summary>
        /// <returns></returns>
        public Texture2D texture {
            get {
                return _texture;
            }
        }

        private const int HTTP_STATUS_NOT_MODIFIED = 304;

        private WebImageCache _imageCache;
        private string _error;
        private Texture2D _texture;
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
        public IEnumerator DownloadImageURL(string url, bool cache = true) {
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
                _error = _loader.GetError();
                yield return null;
            }

            var responseHeaders = _loader.GetResponseHeaders();

            // Check to see if we got a 304 Not Modified response. In that case, just return the cached image.
            if (responseHeaders.ContainsKey(HTTPHeaderFields.STATUS_CODE)) {
                string statusCodeLine = responseHeaders[HTTPHeaderFields.STATUS_CODE];
                if (ParseResponseCode(statusCodeLine) == HTTP_STATUS_NOT_MODIFIED) {
                    CachedWebResource<Texture2D>? resource = _imageCache.TextureResourceForURL(url);
                    if (resource != null) {
                        _texture = resource.Value.Data;
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
            _texture = downloadedTexure;
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

