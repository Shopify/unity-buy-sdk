namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System.Collections;
    using Shopify.Unity.MiniJSON;
    using Shopify.Unity;

    public class NativePayment {
        private enum PaymentField {
            BillingContact,
            ShippingContact,
            ShippingIdentifier,
            TokenData
        }

        private enum TokenField {
            PaymentData,
            TransactionIdentifier,
            Identifier
        }

        readonly public MailingAddressInput BillingAddress;
        readonly public MailingAddressInput ShippingAddress;
        readonly public string Email;
        readonly public string ShippingIdentifier;
        readonly public string TransactionIdentifier;
        readonly public string Identifier;
        readonly public string PaymentData;

        public NativePayment(string stringJson) {
            var dictionary = (Dictionary<string, object>) Json.Deserialize(stringJson);

            // Billing Address
            var billingContactDictionary = (Dictionary<string, object>) dictionary[PaymentField.BillingContact.ToString("G")];
            BillingAddress = new MailingAddressInput(billingContactDictionary);

            // Shipping Address
            var shippingContactDictionary = (Dictionary<string, object>) dictionary[PaymentField.ShippingContact.ToString("G")];
            ShippingAddress = new MailingAddressInput(shippingContactDictionary);

            // Email
            Email = (string) shippingContactDictionary["email"];

            // Shipping Identifier
            if (dictionary.ContainsKey(PaymentField.ShippingIdentifier.ToString("G"))) {
                ShippingIdentifier = (string) dictionary[PaymentField.ShippingIdentifier.ToString("G")];
            }

            // Transaction Identifier
            var tokenDictionary = (Dictionary<string, object>) dictionary[PaymentField.TokenData.ToString("G")];
            TransactionIdentifier = (string) tokenDictionary[TokenField.TransactionIdentifier.ToString("G")];

            // Identifier (Android Pay only)
            if (tokenDictionary.ContainsKey(TokenField.Identifier.ToString("G"))) {
                Identifier = (string) tokenDictionary[TokenField.Identifier.ToString("G")];
            }

            // Payment Data
            // Payment Data can be null when running on a simulator
            if (tokenDictionary.ContainsKey(TokenField.PaymentData.ToString("G"))) {
                var paymentDictionary = tokenDictionary[TokenField.PaymentData.ToString("G")];
                PaymentData = Json.Serialize(paymentDictionary);
            } else {
                PaymentData = "";
            }
        }
    }
}