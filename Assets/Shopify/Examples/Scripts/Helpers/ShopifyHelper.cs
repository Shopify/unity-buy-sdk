using System;
using System.Collections.Generic;
using Shopify.Unity;
using UnityEngine;

namespace Shopify.Examples.Helpers {
    public class ShopifyAlreadyInitializedException : Exception {
    }

    public class ShopifyNotInitializedException : Exception {
    }

    public static class ShopifyHelper {
        public static bool Initialized;

        public static void Init(string accessToken, string shopDomain, string locale = null) {
            if (Initialized) {
                throw new ShopifyAlreadyInitializedException();
            }

            ShopifyBuy.Init(accessToken, shopDomain, locale);

            Initialized = true;
        }

        public static void UpdateLocale(string locale, string shopDomain) {
            // Updates the locale to fetch translated content for.
            // For more information about supported content and fields, visit
            // https://help.shopify.com/en/api/guides/multi-language/translating-content-api
            if (Initialized) {
                ShopifyBuy.Client(shopDomain).UpdateLocale(locale);
            }
        }

        public static void FetchProducts(Action<List<Product>, string> successCallback, Action failureCallback, string cursor) {
            // Fetch product listing from Shopify
            // For more information on querying products visit
            // https://help.shopify.com/api/sdks/custom-storefront/unity-buy-sdk/getting-started#query-all-products

            ShopifyBuy.Client().products((products, error, after) => {
                if (error != null) {
                    failureCallback();

                    return;
                }

                successCallback(products, after);
            }, 5, cursor);
        }

        public static Cart CreateCart() {
            return ShopifyBuy.Client().Cart();
        }
    }
}
