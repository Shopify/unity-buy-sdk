namespace Shopify.Tests {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.Unity.SDK.iOS;

    public class ApplePayFlowTester : MonoBehaviour {

        private const string AccessToken = "020fe3a820b23c7fe618656c7dc069ce";
        private const string ShopDomain = "unity-buy-sdk.myshopify.com";
        private const string CartName = "ApplePayFlowTester";
        private const string StoreName = "Test Store";

        private ShopifyClient Client;
        private Cart CurrentCart;

        void Start() {
            Initialize();
        }

        void Checkout(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            Initialize();

            SetDefaultCartProducts(completion: () => {
                SaveCheckout(callback: (error) => {

                    if (error != null) {
                        message.Respond(error.Description);
                        return;
                    }

                    CreateNativeCheckout();
                    message.Respond("");
                });
            });
        }

        void CheckoutWithShippingAddress(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            Initialize();

            SetDefaultCartProducts(completion: () => {
                SaveCheckout(callback: (checkoutError) => {

                    if (checkoutError != null) {
                        message.Respond(checkoutError.Description);
                        return;
                    }

                    SetDefaultShippingAddress(callback: (ShopifyError shippingError) => {
                        if (shippingError != null) {
                            message.Respond(shippingError.Description);
                            return;
                        }
                        CreateNativeCheckout();
                        message.Respond("");
                    });
                });
            });
        }

        private void Initialize() {
            #if (SHOPIFY_TEST)
            ShopifyBuy.Reset();
            #else
            Debug.Log("Warning: You are running tests without defining SHOPIFY_TEST");
            #endif

            ShopifyBuy.Init(AccessToken, ShopDomain);
            Client = ShopifyBuy.Client();
            CurrentCart = Client.Cart(CartName);
            CurrentCart.State = new CartState(Client);
        }

        private void SetDefaultCartProducts(Action completion) {
            Client.products((products, error) => {
                List<ProductVariant> productVariants = (List<ProductVariant>) products[0].variants();
                CurrentCart.LineItems.AddOrUpdate(productVariants[0], 1);
                completion();
            });
        }

        private void SetDefaultShippingAddress(CompletionCallback callback) {
            var shippingAddressInput =
                new MailingAddressInput(
                    address1: "80 Spadina Ave.",
                    city: "Toronto",
                    country: "Canada",
                    firstName: "John",
                    lastName: "Doe",
                    province: "Ontario",
                    zip: "M5V 2J4"
                );

            CurrentCart.State.SetShippingAddress(shippingAddressInput, (ShopifyError error) => {
                callback(error);
            });
        }

        private void SaveCheckout(CompletionCallback callback) {
            CurrentCart.GetWebCheckoutLink(
                success:(link) => {
                    callback(null);
                },
                failure:(error) => {
                    callback(error);
                }
            );
        }

        private void CreateNativeCheckout() {
            #if UNITY_IOS
            var bridge = GetComponent<ApplePayEventReceiverBridge>();
            var checkout = new iOSNativeCheckout(CurrentCart.State);
            var paymentSettingsJSON = new Dictionary<string, object>();
            var shopMetaData = new ShopMetadata(StoreName, new PaymentSettings(paymentSettingsJSON));

            bridge.Receiver = checkout;
            checkout.Checkout("com.merchant.id", shopMetaData, () => {}, () => {}, (error) => {});
            #endif
        }
    }
}
