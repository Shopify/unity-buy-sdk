#if UNITY_IOS
namespace Shopify.Tests {
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.Unity.MiniJSON;

    [TestFixture]
    public class TestPaymentNetwork {

        [Test]
        public void TestFromCardBrandsNonNull() {

            var cardBrands = new List<CardBrand>();
            cardBrands.Add(CardBrand.AMERICAN_EXPRESS);
            cardBrands.Add(CardBrand.DISCOVER);
            cardBrands.Add(CardBrand.JCB);
            cardBrands.Add(CardBrand.MASTERCARD);
            cardBrands.Add(CardBrand.VISA);

            var paymentNetworks = PaymentNetwork.NetworksFromCardBrands(cardBrands);
            Assert.AreEqual(cardBrands.Count, paymentNetworks.Count, "Not all CardBrands were converted to PaymentNetworks");

            for (int i = 0; i < cardBrands.Count; i ++) {

                var expectedPaymentNetwork = PaymentNetwork.FromCardBrand(cardBrands[i]);
                var actualPaymentNetwork = paymentNetworks[i];

                Assert.AreEqual(
                    expectedPaymentNetwork.ToString(),
                    actualPaymentNetwork.ToString(),
                    "Results from PaymentNetwork.FromCardBrands and PaymentNetwork.FromCardBrand differ for a CardBrand");
            }
        }

        [Test]
        public void TestFromCardBrandsNull() {
            var cardBrands = new List<CardBrand>();
            cardBrands.Add(CardBrand.UNKNOWN);

            var paymentNetworks = PaymentNetwork.NetworksFromCardBrands(cardBrands);
            Assert.AreEqual(0, paymentNetworks.Count, "Expected PaymentNetwork from an unsupported CardBrand to have returned null");
        }
    }
}
#endif
