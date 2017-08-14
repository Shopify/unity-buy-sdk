#if UNITY_IOS
namespace Shopify.Tests {
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.Unity.SDK.iOS;

    [TestFixture]
    public class TestWebCheckout {
        [SetUp]
        public void Setup() {
            #if (SHOPIFY_TEST)
            ShopifyBuy.Reset();
            #endif

            ShopifyBuy.Init(new MockLoader());
        }

        [Test]
        public void TestWebCheckoutSucceeded() {
            bool didSucceed = false;
            bool didCancel = false;
            bool didFail = false;

            var webCheckoutMessageReceiver = CreateMockMessageReceiver("checkout-id");

            webCheckoutMessageReceiver.OnSuccess = () => didSucceed = true;
            webCheckoutMessageReceiver.OnCancelled = () => didCancel = true;
            webCheckoutMessageReceiver.OnFailure = (e) => didFail = true;

            string mockMessage = "{\"Identifier\": \"test\", \"Content\": \"dismissed\"}";
            webCheckoutMessageReceiver.OnNativeMessage(mockMessage);

            Assert.IsTrue(didSucceed);
            Assert.IsFalse(didCancel);
            Assert.IsFalse(didFail);
        }

        [Test]
        public void TestWebCheckoutCancelled() {
            bool didSucceed = false;
            bool didCancel = false;
            bool didFail = false;

            var webCheckoutMessageReceiver = CreateMockMessageReceiver("checkout-id-cancelled");

            webCheckoutMessageReceiver.OnSuccess = () => didSucceed = true;
            webCheckoutMessageReceiver.OnCancelled = () => didCancel = true;
            webCheckoutMessageReceiver.OnFailure = (e) => didFail = true;

            string mockMessage = "{\"Identifier\": \"test\", \"Content\": \"dismissed\"}";
            webCheckoutMessageReceiver.OnNativeMessage(mockMessage);

            Assert.IsFalse(didSucceed);
            Assert.IsTrue(didCancel);
            Assert.IsFalse(didFail);
        }

        [Test]
        public void TestWebCheckoutFailure() {
            bool didSucceed = false;
            bool didCancel = false;
            bool didFail = false;

            var webCheckoutMessageReceiver = CreateMockMessageReceiver("checkout-id-failed");

            webCheckoutMessageReceiver.OnSuccess = () => didSucceed = true;
            webCheckoutMessageReceiver.OnCancelled = () => didCancel = true;
            webCheckoutMessageReceiver.OnFailure = (e) => didFail = true;

            string mockMessage = "{\"Identifier\": \"test\", \"Content\": \"dismissed\"}";
            webCheckoutMessageReceiver.OnNativeMessage(mockMessage);

            Assert.IsFalse(didSucceed);
            Assert.IsFalse(didCancel);
            Assert.IsTrue(didFail);
        }

        private WebCheckoutMessageReceiver CreateMockMessageReceiver(string checkoutId) {

            var webCheckoutMessageReceiver = new WebCheckoutMessageReceiver();

            var cartDict = new Dictionary<string, object> { {"id", checkoutId} };
            webCheckoutMessageReceiver.CurrentCheckout = new Checkout(cartDict);
            webCheckoutMessageReceiver.Client = ShopifyBuy.Client();

            return webCheckoutMessageReceiver;
        }
    }
}
#endif
