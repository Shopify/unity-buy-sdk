namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System;
    using Shopify.Unity.GraphQL;

    /// <summary>
    /// Generates default queries for <see ref="ShopifyClient">ShopifyClient</see>.
    /// </summary>
    public class DefaultShopQueries {
        public void PaymentSettings(QueryRootQuery query) {
            query.shop(
                buildQuery: shop => shop.paymentSettings(
                    buildQuery: paymentSettings => paymentSettings
                    .countryCode()
                    .currencyCode()
                    .acceptedCardBrands()
                    .supportedDigitalWallets()
                )
            );
        }

        public void Name(QueryRootQuery query) {
            query.shop(s => s.name());
        }
    }
}