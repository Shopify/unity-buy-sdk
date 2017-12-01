namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System.Collections;
    using System;

    public class PaymentNetwork {
        readonly string rawValue;

        public static PaymentNetwork FromCardBrand(CardBrand cardBrand) {
            var network = new PaymentNetwork(cardBrand);
            if (network.rawValue == null) {
                return null;
            } else {
                return network;
            }
        }

        public static List<PaymentNetwork> NetworksFromCardBrands(List<CardBrand> cardBrands) {
            var paymentNetworks = new List<PaymentNetwork>();

            foreach (var cardBrand in cardBrands) {
                var paymentNetwork = PaymentNetwork.FromCardBrand(cardBrand);

                if (paymentNetwork != null) {
                    paymentNetworks.Add(paymentNetwork);
                }
            }
            return paymentNetworks;
        }

        private PaymentNetwork(CardBrand cardBrand) {
            switch (cardBrand) {
                case CardBrand.AMERICAN_EXPRESS:
                    rawValue = "AmEx";
                    break;
                case CardBrand.DISCOVER:
                    rawValue = "Discover";
                    break;
                case CardBrand.JCB:
                    rawValue = "JCB";
                    break;
                case CardBrand.MASTERCARD:
                    rawValue = "MasterCard";
                    break;
                case CardBrand.VISA:
                    rawValue = "Visa";
                    break;
                default:
                    rawValue = null;
                    break;
            }
        }

        public override string ToString() {
            return rawValue;
        }
    }
}