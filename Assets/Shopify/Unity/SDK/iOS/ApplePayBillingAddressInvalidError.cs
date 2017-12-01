#if UNITY_IOS
namespace Shopify.Unity.SDK.iOS {
    using System.Collections.Generic;

    public class ApplePayBillingAddressInvalidError : ApplePayAddressInvalidError {
        public ApplePayBillingAddressInvalidError(string description, AddressField field) : base(ErrorType.PaymentBillingAddress, description, field) { }
        public ApplePayBillingAddressInvalidError(string description, string mailingAddressInputField) : base(ErrorType.PaymentBillingAddress, description, mailingAddressInputField) { }
    }
}
#endif