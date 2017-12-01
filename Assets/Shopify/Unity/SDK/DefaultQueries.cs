namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System;
    using Shopify.Unity.GraphQL;

    /// <summary>
    /// Generates default queries for <see ref="ShopifyClient.products">ShopifyClient.products </see> and <see ref="ShopifyClient.collections">ShopifyClient.collections </see>.
    /// </summary>
    public class DefaultQueries {
        public static readonly int MaxPageSize = 250;
        public static readonly int MaxProductPageSize = 10;
        public static readonly int MaxCollectionsPageSize = 40;

        public static DefaultProductQueries products = new DefaultProductQueries();
        public static DefaultCollectionQueries collections = new DefaultCollectionQueries();
        public static DefaultCheckoutQueries checkout = new DefaultCheckoutQueries();
        public static DefaultShopQueries shop = new DefaultShopQueries();

        public static string GetAliasFromIndex(int idx) {
            return String.Format("a{0}", idx);
        }
    }
}