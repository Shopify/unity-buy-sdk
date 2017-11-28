 namespace Shopify.UIToolkit {
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A cacheable image downloaed from the web stored as a Texture2D object.
    /// </summary>
    public class CachedWebImage : CachedWebResource {
        Texture2D texture {
            get {
                return (Texture2D) Data;
            }
        }

        public CachedWebImage(string lastModifiedTimestamp, Texture2D data) : base(lastModifiedTimestamp, data) {}

        public override int SizeOnDisk() {
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
    public class WebImageCache : LRUCache<CachedWebImage> {
        private static WebImageCache _sharedCache;

        /// <summary>
        /// Shared WebImageCache instanced.
        /// </summary>
        /// <returns>A singleton instance of the WebImageCache.</returns>
        public static WebImageCache SharedCache {
            get {
                _sharedCache = _sharedCache ?? new WebImageCache(DEFAULT_MEMORY_SIZE_LIMIT);
                return _sharedCache;
            }
        }

        private WebImageCache(int memoryLimit) : base(memoryLimit) {}

        public void SetTextureResourceForURL(string url, CachedWebImage resource) {
            SetResourceForURL(url, resource);
        }

        public CachedWebImage TextureResourceForURL(string url) {
            return ResourceForURL(url);
        }
    }
 }
