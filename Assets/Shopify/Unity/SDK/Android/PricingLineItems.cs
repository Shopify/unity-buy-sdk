#if UNITY_ANDROID
namespace Shopify.Unity.SDK.Android {
    using System.Collections.Generic;
    using System.Collections;
    using System;

    /// <summary>
    /// Model class that wraps the aggregate of all purchases, taxes and shipping
    /// costs of the cart.
    /// </summary>
    public class PricingLineItems : Serializable {
        /// <value>JSON name for the "tax price" attribute.</value>
        public readonly decimal TaxPrice;
        /// <value>JSON name for the "subtotal" attribute.</value>
        public readonly decimal Subtotal;
        /// <value>JSON name for the "total price" attribute.</value>
        public readonly decimal TotalPrice;
        /// <value>JSON name for the "shipping price" attribute.</value>
        public readonly decimal? ShippingPrice;

        public PricingLineItems(
            decimal taxPrice,
            decimal subtotal,
            decimal totalPrice,
            decimal? shippingPrice = null) {
            TaxPrice = taxPrice;
            Subtotal = subtotal;
            TotalPrice = totalPrice;
            ShippingPrice = shippingPrice;
        }

        public override object ToJson() {
            var dict = new Dictionary<string, object>();
            dict["taxPrice"] = TaxPrice;
            dict["subtotal"] = Subtotal;
            dict["totalPrice"] = TotalPrice;
            if (ShippingPrice != null) {
                dict["shippingPrice"] = ShippingPrice;
            }
            return dict;
        }
    }
}
#endif