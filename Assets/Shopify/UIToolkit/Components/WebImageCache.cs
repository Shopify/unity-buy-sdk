 namespace Shopify.UIToolkit {
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A web resource that is cached against a Last-Modified timestamp.
    /// </summary>
    public struct CachedWebResource<T> {
        public readonly string LastModifiedTimestamp;
        public readonly T Data;

        public CachedWebResource(string lastModifiedTimestamp, T data) {
            LastModifiedTimestamp = lastModifiedTimestamp;
            Data = data;
        }
    }

    /// <summary>
    /// This class implements a LRU cache for images (PNG/JPEG) that were downloaded from the web.
    /// </summary>
    public class WebImageCache {
        /// <summary>
        /// The default estimated memory size limit in bytes.
        /// </summary>
        public const int DEFAULT_MEMORY_SIZE_LIMIT = 20971520; // 20 MB in bytes

        private Dictionary<string, CachedWebResource<Texture2D>> _urlTextureCache = 
            new Dictionary<string, CachedWebResource<Texture2D>>();

        private LinkedList<string> _recentlyUsed = new LinkedList<string>();

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

        /// <summary>
        /// The estimated size limit in bytes of how much memory we want to use for the cache (readonly).
        /// </summary>
        /// <returns></returns>
        public int MemorySizeLimit { get; private set; }

        /// <summary>
        /// The current estimated size the cache takes up in memory (readonly)
        /// </summary>
        /// <returns></returns>
        public int EstimatedMemorySize { get; private set; }

        /// <summary>
        /// Returns the number of entries in the cache.
        /// </summary>
        /// <returns>Number of entries in the cache.</returns>
        public int Count {
            get {
                return _urlTextureCache.Count;
            }
        }

        /// <summary>
        /// Sets the size limit in bytes that we can store in the cache.
        /// </summary>
        /// <param name="newLimit">New size limit in bytes.</param>
        public void SetMemorySizeLimit(int newLimit) {
            Debug.Assert(newLimit >= 0, "Size limit must be greater or equal to 0.");

            // If we're bigger than the new limit then make some room by evicting all of the oldest resources
            // from the cache.
            while (EstimatedMemorySize > newLimit) {
                var oldestNode = _recentlyUsed.Last;
                var oldestUsedTexture = _urlTextureCache[oldestNode.Value].Data;
                var oldestUsedTextureSize = EstimateMemoryFootprintForTexture(oldestUsedTexture);

                _urlTextureCache.Remove(oldestNode.Value);
                _recentlyUsed.RemoveLast();
                EstimatedMemorySize -= oldestUsedTextureSize;
            }

            MemorySizeLimit = newLimit;
        }

        private WebImageCache(int memorySizeLimit) {
            MemorySizeLimit = memorySizeLimit;
        }

        /// <summary>
        /// Returns the cached Texture2D texture saved against the given url.
        /// </summary>
        /// <param name="url">URL mapping to a texture.</param>
        /// <returns>The cached resource associated with the given url.</returns>
        public CachedWebResource<Texture2D>? TextureResourceForURL(string url) {
            var texture = TextureFromCacheForURL(url);
            if (texture != null) {
                PromoteURL(url);
            }
            return texture;
        }

        /// <summary>
        /// Associates the given URL with the given texture in the cache.
        /// </summary>
        /// <param name="url">URL to key the cache with.</param>
        /// <param name="textureResource">A CachedWebResource<Texture2D> instance to cache in memory.</param>
        public void SetTextureResourceForURL(string url, CachedWebResource<Texture2D> textureResource) {
            // If we already have it in the list, promote it. If not then we'll need to add it.
            if (TextureFromCacheForURL(url) != null) {
                PromoteURL(url);
                RemoveURLFromCache(url);
            } else {
                _recentlyUsed.AddFirst(new LinkedListNode<string>(url));
            }

            var estimatedTextureSize = EstimateMemoryFootprintForTexture(textureResource.Data);
            int nextMemoryFootprint = EstimatedMemorySize + estimatedTextureSize;

            // Evict the least recently used textures from the cache until we can fit the new one in.
            while (nextMemoryFootprint > MemorySizeLimit) {
                var oldestNode = _recentlyUsed.Last;
                var oldestUsedTexture = _urlTextureCache[oldestNode.Value].Data;
                var oldestUsedTextureSize = EstimateMemoryFootprintForTexture(oldestUsedTexture);

                _urlTextureCache.Remove(oldestNode.Value);
                _recentlyUsed.RemoveLast();
                nextMemoryFootprint -= oldestUsedTextureSize;
            }

            EstimatedMemorySize = nextMemoryFootprint;
            _urlTextureCache[url] = textureResource;
        }

        /// <summary>
        /// Removes the URL and associated Texture2D instance from the cache.
        /// </summary>
        /// <param name="url">URL key to remove from the cache.</param>
        public void RemoveURL(string url) {
            RemoveURLFromCache(url);

            var node =_recentlyUsed.Find(url);
            if (node != null) {
                _recentlyUsed.Remove(node);
            }
        }

        /// <summary>
        /// Clears the cache. 
        /// </summary>
        public void Clear() {
            _urlTextureCache.Clear();
            _recentlyUsed.Clear();
            EstimatedMemorySize = 0;
        }

        private int EstimateMemoryFootprintForTexture(Texture2D texture) {
            // We can assume these images will only have a single mipmap level.
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

        private void PromoteURL(string url) {
            var node =_recentlyUsed.Find(url);
            if (node != null) {
                _recentlyUsed.Remove(node);
                _recentlyUsed.AddFirst(new LinkedListNode<string>(url));
            }
        }

        private CachedWebResource<Texture2D>? TextureFromCacheForURL(string url) {
            try {
                return _urlTextureCache[url];
            } catch {
                return null;
            }
        }

        private void RemoveURLFromCache(string url) {
            var texture = _urlTextureCache[url].Data;
            var textureSize = EstimateMemoryFootprintForTexture(texture);
            _urlTextureCache.Remove(url);
            EstimatedMemorySize -= textureSize;
        }
    }
 }
