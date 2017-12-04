 namespace Shopify.UIToolkit {
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A cacheable image downloaed from the web stored as a Texture2D object.
    /// </summary>
    public class CacheableWebImage : ICacheable {
        public Texture2D texture {
            get {
                return (Texture2D) Data;
            }
        }

        public readonly string LastModified;

        public object Data {
            get {
                return _data;
            }
        }

        private object _data;

        public CacheableWebImage(Texture2D data, string timestamp) {
            _data = data;
            LastModified = timestamp;
        }

        public int SizeInBytes() {
            return texture.width * texture.height * StrideForTextureFormat(texture.format);
        }

        private int StrideForTextureFormat(TextureFormat format) {
            switch (format) {
                // Non-alpha channel format (JPEG)
                case TextureFormat.RGB24:
                    return 3;
                // Alpha channel formats (PNG)
                case TextureFormat.ARGB32:
                    return 4;
                case TextureFormat.RGBA32:
                    return 4;
                default:
                    return 4;
            }
        }
    }

    /// <summary>
    /// This class implements a LRU cache for images (PNG/JPEG) that were downloaded from the web.
    /// </summary>
    public class WebImageCache : LRUCache<CacheableWebImage> {
        private static WebImageCache _sharedCache;

        /// <summary>
        /// Shared WebImageCache instance.
        /// </summary>
        /// <returns>A singleton instance of the WebImageCache.</returns>
        public static WebImageCache SharedCache {
            get {
                _sharedCache = _sharedCache ?? new WebImageCache(DEFAULT_MEMORY_SIZE_LIMIT);
                return _sharedCache;
            }
        }

        private WebImageCache(int memoryLimit) : base(memoryLimit) {}

        /// <summary>
        /// Sets the given texture to the cached keyed by the URL and associates the timestamp with 
        /// this image.
        /// </summary>
        /// <param name="url">URL the image was fetched from.</param>
        /// <param name="texture">Texture2D instance of the downloaded image.</param>
        /// <param name="lastModified">Timestamp of when the image as last modified.</param>
        public void SetTextureForURL(string url, Texture2D texture, string lastModified) {
            var webImage = new CacheableWebImage(texture, lastModified);
            SetResourceForKey(url, webImage);
        }

        /// <summary>
        /// Returns a CacheableWebImage object that contains the texture and last modified timestamp.
        /// </summary>
        /// <param name="url">URL key to look for in the image cache.</param>
        /// <returns>CacheableWebImage associated with the given URL.</returns>
        public CacheableWebImage TextureResourceForURL(string url) {
            try {
                return ResourceForKey(url);
            } catch {
                return null;
            }
        }
    }
 }
