#if UNITY_IOS
namespace Shopify.Unity.SDK.iOS {
    public enum ApplePayAuthorizationStatus {
        Failure,
        Success,
        InvalidBillingPostalAddress,
        InvalidShippingPostalAddress,
        InvalidShippingContact,
        PinRequired,
        PinIncorrect,
        PinLockout
    }
}
#endif