#if UNITY_IOS
namespace Shopify.Unity.SDK.iOS {
    using System.Collections.Generic;

    public class ApplePayShippingAddressUnservicableError : ApplePayError {
        public ApplePayShippingAddressUnservicableError(string description) : base(ErrorType.ShippingAddressUnservicable, description) { }
    }
}
#endif