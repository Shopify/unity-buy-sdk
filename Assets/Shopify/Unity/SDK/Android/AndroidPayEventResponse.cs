#if UNITY_ANDROID
namespace Shopify.Unity.SDK.Android {
    using System.Collections.Generic;
    using System.Collections;
    using System;

    /// <summary>
    /// Model class that wraps all the data sent from Unity to the Android library
    /// after the shipping address is changed.
    /// </summary>
    public class AndroidPayEventResponse : Serializable {
        /// <value>JSON name for the "merchant name" attribute.</value>
        public readonly string MerchantName;
        /// <value>JSON name for the "pricing line items" attribute.</value>
        public readonly PricingLineItems PricingLineItems;
        /// <value>JSON name for the "currency code" attribute.</value>
        public readonly string CurrencyCode;
        /// <value>JSON name for the "country code" attribute.</value>
        public readonly string CountryCode;
        /// <value>JSON name for the "requires shipping" attribute.</value>
        public readonly bool RequiresShipping;
        /// <value>JSON name for the "shipping methods" attribute.</value>
        public readonly List<ShippingMethod> ShippingMethods;

        public AndroidPayEventResponse(
            string merchantName,
            PricingLineItems pricingLineItems,
            string currencyCode,
            string countryCode,
            bool requiresShipping,
            List<ShippingMethod> shippingMethods) {
            MerchantName = merchantName;
            PricingLineItems = pricingLineItems;
            CurrencyCode = currencyCode;
            CountryCode = countryCode;
            RequiresShipping = requiresShipping;
            ShippingMethods = shippingMethods;
        }

        /// <summary>
        /// Converts this object to a <see cref="Dictionary"/>
        /// that can be later converted to a JSON representation.
        /// </summary>
        public override object ToJson() {
            var dict = new Dictionary<string, object>();
            dict["merchantName"] = MerchantName;
            dict["pricingLineItems"] = PricingLineItems.ToJson();
            dict["currencyCode"] = CurrencyCode;
            dict["countryCode"] = CountryCode;
            dict["requiresShipping"] = RequiresShipping;
            dict["shippingMethods"] = ShippingMethod.ListToJson(ShippingMethods);
            return dict;
        }
    }
}
#endif