#if UNITY_IOS
namespace Shopify.Unity.SDK.iOS {
    using System.Collections.Generic;
    using Shopify.Unity.SDK;

    public class ApplePayError : Serializable {
        public enum ErrorType {
            PaymentBillingAddress,
            PaymentShippingAddress,
            PaymentContactInvalid,
            ShippingAddressUnservicable
        }

        public readonly ErrorType Type;
        public readonly string Description;

        public ApplePayError(ErrorType type, string description) {
            Type = type;
            Description = description;
        }

        public override object ToJson() {
            var dict = new Dictionary<string, object>();

            dict["Type"] = Type;
            dict["Description"] = Description;

            return dict;
        }
    }
}
#endif