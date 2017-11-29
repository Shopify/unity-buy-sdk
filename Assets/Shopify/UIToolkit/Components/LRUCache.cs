 namespace Shopify.UIToolkit {
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface ICacheable {
        object Data { get; }
        int SizeInBytes();
    }

    /// <summary>
    /// A cache that uses a Least Recently Used (LRU) heurstic for evicting items.
    /// </summary>
    public abstract class LRUCache<T> : DataCache<T> where T: ICacheable {
        /// <summary>
        /// The default estimated memory size limit in bytes.
        /// </summary>
        public const int DEFAULT_MEMORY_SIZE_LIMIT = 20971520; // 20 MB in bytes

        private Dictionary<string, T> _memoryCache = new Dictionary<string, T>();

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
                var oldestUsedDataSize = oldestUsedResource.SizeInBytes();

                _memoryCache.Remove(oldestNode.Value);
                _recentlyUsed.RemoveLast();
                EstimatedMemorySize -= oldestUsedDataSize;
            }

            MemorySizeLimit = newLimit;
        }

        public T ResourceForKey(string key) {
            var resource = _memoryCache[key];
            PromoteKey(key);
            return resource;
        }

        public void SetResourceForKey(string key, T resource) {
            // If we already have it in the list, promote it. If not then we'll need to add it.
            if (_memoryCache.ContainsKey(key)) {
                PromoteKey(key);
                RemoveKeyFromCache(key);
            } else {
                _recentlyUsed.AddFirst(new LinkedListNode<string>(key));
            }

            var size = resource.SizeInBytes();
            int nextMemoryFootprint = EstimatedMemorySize + size;

            // Evict the least recently used resources from the cache until we can fit the new one in.
            while (nextMemoryFootprint > MemorySizeLimit) {
                var oldestNode = _recentlyUsed.Last;
                var oldestUsedResource = _memoryCache[oldestNode.Value];
                var oldestUsedDataSize = oldestUsedResource.SizeInBytes();

                _memoryCache.Remove(oldestNode.Value);
                _recentlyUsed.RemoveLast();
                nextMemoryFootprint -= oldestUsedDataSize;
            }

            EstimatedMemorySize = nextMemoryFootprint;
            _memoryCache[key] = resource;
        }

        public void RemoveKey(string key) {
            RemoveKeyFromCache(key);

            var node =_recentlyUsed.Find(key);
            if (node != null) {
                _recentlyUsed.Remove(node);
            }
        }

        public void Clear() {
            _memoryCache.Clear();
            _recentlyUsed.Clear();
            EstimatedMemorySize = 0;
        }

        private void PromoteKey(string key) {
            var node =_recentlyUsed.Find(key);
            if (node != null) {
                _recentlyUsed.Remove(node);
                _recentlyUsed.AddFirst(new LinkedListNode<string>(key));
            }
        }

        private void RemoveKeyFromCache(string key) {
            var resource = _memoryCache[key];
            var dataSize = resource.SizeInBytes();
            _memoryCache.Remove(key);
            EstimatedMemorySize -= dataSize;
        }
    }
 }
