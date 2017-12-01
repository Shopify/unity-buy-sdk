#if UNITY_IOS
namespace Shopify.Unity.SDK.iOS {
    using System.Collections.Generic;
    using System.Collections;
    using System;

    public class ApplePayEventResponse : Serializable {
        public readonly string AuthorizationStatus;
        public readonly List<SummaryItem> SummaryItems;
        public readonly List<ShippingMethod> ShippingMethods;
        public readonly List<ApplePayError> Errors;

        public ApplePayEventResponse(
            ApplePayAuthorizationStatus authorizationStatus,
            List<SummaryItem> summaryItems = null,
            List<ShippingMethod> shippingMethods = null,
            List<ApplePayError> errors = null) {
            AuthorizationStatus = authorizationStatus.ToString("G");
            SummaryItems = summaryItems ?? new List<SummaryItem>();
            ShippingMethods = shippingMethods ?? new List<ShippingMethod>();
            Errors = errors ?? new List<ApplePayError>();
        }

        public override object ToJson() {
            var dict = new Dictionary<string, object>();
            dict["AuthorizationStatus"] = AuthorizationStatus;

            var summaryItemsJSON = new List<Dictionary<string, object>>();
            foreach (var item in SummaryItems) {
                summaryItemsJSON.Add((Dictionary<string, object>) item.ToJson());
            }
            dict["SummaryItems"] = summaryItemsJSON;

            var shippingMethodsJSON = new List<Dictionary<string, object>>();
            foreach (var method in ShippingMethods) {
                shippingMethodsJSON.Add((Dictionary<string, object>) method.ToJson());
            }
            dict["ShippingMethods"] = shippingMethodsJSON;

            var errorsJSON = new List<Dictionary<string, object>>();
            foreach (var error in Errors) {
                errorsJSON.Add((Dictionary<string, object>) error.ToJson());
            }
            dict["Errors"] = errorsJSON;

            return dict;
        }
    }
}
#endif