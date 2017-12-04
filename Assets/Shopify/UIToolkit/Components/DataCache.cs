 namespace Shopify.UIToolkit {
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// An interface for defining a cache.
    /// </summary>
    public interface Cache<T> {
        /// <summary>
        /// The estimated size limit in bytes of how much memory we want to use for the cache (readonly).
        /// </summary>
        /// <returns></returns>
        int MemorySizeLimit { get; }

        /// <summary>
        /// The current estimated size the cache takes up in memory (readonly).
        /// </summary>
        /// <returns></returns>
        int EstimatedMemorySize { get; }

        /// <summary>
        /// Returns the number of entries in the cache (readonly).
        /// </summary>
        /// <returns>Number of entries in the cache.</returns>
        int Count { get; }

        /// <summary>
        /// Sets the size limit in bytes that we can store in the cache.
        /// </summary>
        /// <param name="newLimit">New size limit in bytes.</param>
        void SetMemorySizeLimit(int newLimit);

        /// <summary>
        /// Returns the cached resource saved against the given url.
        /// </summary>
        /// <param name="key">Key mapping to a resource.</param>
        /// <returns>The cached resource associated with the given key.</returns>
        T ResourceForKey(string key);

        /// <summary>
        /// Associates the given key with the given resource in the cache.
        /// </summary>
        /// <param name="key">Key to cache with.</param>
        /// <param name="resource">A resource to cache in memory.</param>
        void SetResourceForKey(string key, T resource);

        /// <summary>
        /// Removes the key and associated resource instance from the cache.
        /// </summary>
        /// <param name="key">Key to remove from the cache.</param>
        void RemoveKey(string key);

        /// <summary>
        /// Clears the cache. 
        /// </summary>
        void Clear();
    }
}
