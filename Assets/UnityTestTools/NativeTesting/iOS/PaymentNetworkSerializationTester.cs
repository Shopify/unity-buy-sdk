namespace Shopify.Tests {
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Shopify.Unity;
    using Shopify.Unity.MiniJSON;

    #if UNITY_IOS
    using Shopify.Unity.SDK.iOS;
    #endif

    public partial class PaymentNetworkSerializationTester : MonoBehaviour {}

    #if UNITY_IOS
    public partial class PaymentNetworkSerializationTester {

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

        void GetAllCardBrandPaymentNetworksString(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);

            var cardBrands = new List<CardBrand>() {
                CardBrand.AMERICAN_EXPRESS,
                CardBrand.DINERS_CLUB,
                CardBrand.DISCOVER,
                CardBrand.JCB,
                CardBrand.MASTERCARD,
                CardBrand.VISA,
                CardBrand.UNKNOWN
            };

            message.Respond(Json.Serialize(PaymentNetwork.NetworksFromCardBrands(cardBrands)));
        }
    }
    #endif
}
