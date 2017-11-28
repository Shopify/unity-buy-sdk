 namespace Shopify.UIToolkit {
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A web resource that is cached against a Last-Modified timestamp.
    /// </summary>
    public abstract class CachedWebResource {
        /// <summary>
        /// The Last-Modified timestamp from a HTTP header field.
        /// </summary>
        public readonly string LastModifiedTimestamp;

        /// <summary>
        /// Data instance to cache.
        /// </summary>
        public readonly object Data;

        /// <summary>
        /// Returns the size of the associated Data object in bytes.
        /// </summary>
        /// <returns>Size of Data property in bytes.</returns>
        public virtual int SizeOnDisk() {
            return 0;
        }

        public CachedWebResource(string lastModifiedTimestamp, object data) {
            LastModifiedTimestamp = lastModifiedTimestamp;
            Data = data;
        }
    }

    /// <summary>
    /// A cache that uses a Least Recently Used (LRU) heurstic for evicting items.
    /// </summary>
    public abstract class LRUCache<T> : DataCache<T> where T: CachedWebResource {
        /// <summary>
        /// The default estimated memory size limit in bytes.
        /// </summary>
        public const int DEFAULT_MEMORY_SIZE_LIMIT = 20971520; // 20 MB in bytes

        private Dictionary<string, T> _memoryCache = 
            new Dictionary<string, T>();

        private LinkedList<string> _recentlyUsed = new LinkedList<string>();

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
                return _memoryCache.Count;
            }
        }

        protected LRUCache(int memorySizeLimit) {
            MemorySizeLimit = memorySizeLimit;
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
                var oldestUsedResource = _memoryCache[oldestNode.Value];
                var oldestUsedDataSize = oldestUsedResource.SizeOnDisk();

                _memoryCache.Remove(oldestNode.Value);
                _recentlyUsed.RemoveLast();
                EstimatedMemorySize -= oldestUsedDataSize;
            }

            MemorySizeLimit = newLimit;
        }

        /// <summary>
        /// Returns the cached resource saved against the given url.
        /// </summary>
        /// <param name="url">URL mapping to a texture.</param>
        /// <returns>The cached resource associated with the given url.</returns>
        public T ResourceForURL(string url) {
            return ResourceForKey(url);
        }

        /// <summary>
        /// Associates the given URL with the given resource in the cache.
        /// </summary>
        /// <param name="url">URL to key the cache with.</param>
        /// <param name="resource">A CachedWebResource<Texture2D> instance to cache in memory.</param>
        public void SetResourceForURL(string url, T resource) {
            SetResourceForKey(url, resource);
        }

        /// <summary>
        /// Removes the URL and associated resource instance from the cache.
        /// </summary>
        /// <param name="url">URL key to remove from the cache.</param>
        public void RemoveURL(string url) {
            RemoveKey(url);
        }

        /// <summary>
        /// Clears the cache. 
        /// </summary>
        public void Clear() {
            _memoryCache.Clear();
            _recentlyUsed.Clear();
            EstimatedMemorySize = 0;
        }

        public T ResourceForKey(string key) {
            var resource = ResourceFromCacheForURL(key);
            if (resource != null) {
                PromoteURL(key);
            }
            return resource;
        }

        public void SetResourceForKey(string key, T resource) {
            // If we already have it in the list, promote it. If not then we'll need to add it.
            if (ResourceFromCacheForURL(key) != null) {
                PromoteURL(key);
                RemoveURLFromCache(key);
            } else {
                _recentlyUsed.AddFirst(new LinkedListNode<string>(key));
            }

            var size = resource.SizeOnDisk();
            int nextMemoryFootprint = EstimatedMemorySize + size;

            // Evict the least recently used resources from the cache until we can fit the new one in.
            while (nextMemoryFootprint > MemorySizeLimit) {
                var oldestNode = _recentlyUsed.Last;
                var oldestUsedResource = _memoryCache[oldestNode.Value];
                var oldestUsedDataSize = oldestUsedResource.SizeOnDisk();

                _memoryCache.Remove(oldestNode.Value);
                _recentlyUsed.RemoveLast();
                nextMemoryFootprint -= oldestUsedDataSize;
            }

            EstimatedMemorySize = nextMemoryFootprint;
            _memoryCache[key] = resource;
        }

        public void RemoveKey(string key) {
            RemoveURLFromCache(key);

            var node =_recentlyUsed.Find(key);
            if (node != null) {
                _recentlyUsed.Remove(node);
            }
        }

        private void PromoteURL(string url) {
            var node =_recentlyUsed.Find(url);
            if (node != null) {
                _recentlyUsed.Remove(node);
                _recentlyUsed.AddFirst(new LinkedListNode<string>(url));
            }
        }

        private T ResourceFromCacheForURL(string url) {
            try {
                return _memoryCache[url];
            } catch {
                return null;
            }
        }

        private void RemoveURLFromCache(string url) {
            var resource = _memoryCache[url];
            var dataSize = resource.SizeOnDisk();
            _memoryCache.Remove(url);
            EstimatedMemorySize -= dataSize;
        }
    }
 }
