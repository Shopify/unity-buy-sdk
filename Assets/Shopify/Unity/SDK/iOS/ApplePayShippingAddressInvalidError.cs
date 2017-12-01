#if UNITY_IOS
namespace Shopify.Unity.SDK.iOS {
    using System.Collections.Generic;

    public class ApplePayShippingAddressInvalidError : ApplePayAddressInvalidError {
        public ApplePayShippingAddressInvalidError(string description, AddressField field) : base(ErrorType.PaymentShippingAddress, description, field) { }
        public ApplePayShippingAddressInvalidError(string description, string mailingAddressInputField) : base(ErrorType.PaymentShippingAddress, description, mailingAddressInputField) { }
    }
}
#endif