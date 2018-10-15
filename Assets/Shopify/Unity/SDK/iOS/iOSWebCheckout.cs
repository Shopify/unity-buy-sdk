#if UNITY_IOS
namespace Shopify.Unity.SDK.iOS {
    using System.Runtime.InteropServices;
    using System;
    using Shopify.Unity.SDK;

    class iOSWebCheckout : WebCheckout {
        [DllImport("__Internal")]
        private static extern void _CheckoutWithWebView(string unityDelegateObjectName, string checkoutURL);

        private ShopifyClient _client;
        private Cart _cart;

        protected override ShopifyClient Client {
            get {
                return _client;
            }
        }

        protected override Cart Cart {
            get {
                return _cart;
            }
        }

        public iOSWebCheckout(Cart cart, ShopifyClient client) {
            _cart = cart;
            _client = client;
        }

        public override void Checkout(string checkoutURL, CheckoutSuccessCallback success, CheckoutCancelCallback cancelled, CheckoutFailureCallback failure) {
            SetupWebCheckoutMessageReceiver(success, cancelled, failure);
            _CheckoutWithWebView(GetNativeMessageReceiverName(), checkoutURL);
        }
    }
}
#endif
