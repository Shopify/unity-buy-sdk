namespace Shopify.Tests {
    using System;
    using UnityEngine;
    using Shopify.Unity;
    using Shopify.Unity.SDK.iOS;
    using Shopify.Unity.MiniJSON;

    public class PaymentNetworkSerializationTester : MonoBehaviour {

        void GetAmexPaymentNetworkString(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            message.Respond(PaymentNetwork.FromCardBrand(CardBrand.AMERICAN_EXPRESS).ToString());
        }

        void GetDiscoverPaymentNetworkString(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            message.Respond(PaymentNetwork.FromCardBrand(CardBrand.DISCOVER).ToString());
        }

        void GetJCBPaymentNetworkString(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            message.Respond(PaymentNetwork.FromCardBrand(CardBrand.JCB).ToString());
        }

        void GetMasterCardPaymentNetworkString(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            message.Respond(PaymentNetwork.FromCardBrand(CardBrand.MASTERCARD).ToString());
        }

        void GetVisaPaymentNetworkString(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            message.Respond(PaymentNetwork.FromCardBrand(CardBrand.VISA).ToString());
        }
    }
}
