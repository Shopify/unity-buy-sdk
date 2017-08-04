namespace Shopify.Tests {
    using System;
    using UnityEngine;
    using Shopify.Unity;
    using Shopify.Unity.SDK.iOS;
    using Shopify.Unity.MiniJSON;

    public class PaymentNetworkSerializationTester : MonoBehaviour {

        void GetAmexPaymentNetworkString(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            message.Respond(Json.Serialize(PaymentNetwork.FromCardBrand(CardBrand.AMERICAN_EXPRESS)));
        }

        void GetDiscoverPaymentNetworkString(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            message.Respond(Json.Serialize(PaymentNetwork.FromCardBrand(CardBrand.DISCOVER)));
        }

        void GetJCBPaymentNetworkString(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            message.Respond(Json.Serialize(PaymentNetwork.FromCardBrand(CardBrand.JCB)));
        }

        void GetMasterCardPaymentNetworkString(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            message.Respond(Json.Serialize(PaymentNetwork.FromCardBrand(CardBrand.MASTERCARD)));
        }

        void GetVisaPaymentNetworkString(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            message.Respond(Json.Serialize(PaymentNetwork.FromCardBrand(CardBrand.VISA)));
        }
    }
}
