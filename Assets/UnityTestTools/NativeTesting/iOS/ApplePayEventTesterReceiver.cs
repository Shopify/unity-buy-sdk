#if UNITY_IOS
namespace Shopify.Tests.iOS {
    using System.Collections;
    using System.Collections.Generic;
    using Shopify.Unity.SDK.iOS;
    using Shopify.Unity.SDK;

    public class ApplePayEventTesterReceiver : IApplePayEventReceiver {
        public void UpdateSummaryItemsForShippingIdentifier(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            var response = new ApplePayEventResponse(ApplePayAuthorizationStatus.Success, GetExpectedSummaryItems());
            message.Respond(response.ToJsonString());
         }

        public void UpdateSummaryItemsForShippingContact(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            var response = new ApplePayEventResponse(ApplePayAuthorizationStatus.Success, GetExpectedSummaryItems(), GetExpectedShippingMethods());
            message.Respond(response.ToJsonString());
        }

        public void FetchApplePayCheckoutStatusForToken(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            var response = new ApplePayEventResponse(ApplePayAuthorizationStatus.Success);
            message.Respond(response.ToJsonString());
        }

        public void DidFinishCheckoutSession(string serializedMessage) {
            // Create the message to record it as received
             NativeMessageTester.CreateFromJSON(serializedMessage);
        }

        private List<SummaryItem> GetExpectedSummaryItems() {
            var summaryItems = new List<SummaryItem>();
            summaryItems.Add(new SummaryItem("SubTotal", "1.00"));
            return summaryItems;
        }

        private List<ShippingMethod> GetExpectedShippingMethods() {
            var shippingMethods = new List<ShippingMethod>();
            shippingMethods.Add(new ShippingMethod("Free Shipping", "0", "unique_id", "10-15 Days"));
            return shippingMethods;
        }
    }
}
#endif
