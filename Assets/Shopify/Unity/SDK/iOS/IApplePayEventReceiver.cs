#if UNITY_IOS
namespace Shopify.Unity.SDK.iOS {
    using System.Collections.Generic;
    using System.Collections;

    public interface IApplePayEventReceiver {
        void UpdateSummaryItemsForShippingIdentifier(string serializedMessage);
        void UpdateSummaryItemsForShippingContact(string serializedMessage);
        void FetchApplePayCheckoutStatusForToken(string serializedMessage);
        void DidFinishCheckoutSession(string serializedMessage);
    }
}
#endif