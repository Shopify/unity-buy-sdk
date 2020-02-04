using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shopify;

namespace Shopify.Unity.Tests {
    public class Clients {
        public static ShopifyClient GraphQL {
            get {
                return GetClient("graphql.myshopify.com", "351c122017d0f2a957d32ae728ad749c");
            }
        }

        public static ShopifyClient GraphQLMany {
            get {
                return GetClient("graphql-many-products.myshopify.com", "43b7fef8bd2f27f1d645586b72c9b825");
            }
        }

        private static ShopifyClient GetClient(string domain, string accessToken, string locale = null) {
            if (ShopifyBuy.Client(domain) == null) {
                ShopifyBuy.Init(accessToken, domain, locale);
            }

            return ShopifyBuy.Client(domain);
        }
    }
}
