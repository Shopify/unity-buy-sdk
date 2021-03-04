namespace Shopify.Unity.SDK {
    using System.Runtime.InteropServices;
    using System;
    using Shopify.Unity.GraphQL;
    using Shopify.Unity.SDK;
    using UnityEngine;

    public partial class WebCheckoutMessageReceiver : MonoBehaviour { }

    public partial class WebCheckoutMessageReceiver {
        public ShopifyClient Client;
        public Checkout CurrentCheckout;

        public CheckoutSuccessCallback OnSuccess;
        public CheckoutCancelCallback OnCancelled;
        public CheckoutFailureCallback OnFailure;

        public void Init(
            ShopifyClient client,
            Checkout currentCheckout,
            CheckoutSuccessCallback onSuccess,
            CheckoutCancelCallback onCancelled,
            CheckoutFailureCallback onFailure) {
            Client = client;
            CurrentCheckout = currentCheckout;
            OnSuccess = onSuccess;
            OnCancelled = onCancelled;
            OnFailure = onFailure;
        }

        public void OnNativeMessage(string jsonMessage) {
            var message = NativeMessage.CreateFromJSON(jsonMessage);

            if (message.Content != "dismissed") {
                return;
            }

            CheckIfCheckoutComplete();
        }

        private void OnApplicationFocus(bool hasFocus) {
            if (!hasFocus) {
                return;
            }

            CheckIfCheckoutComplete();
        }

        private void CheckIfCheckoutComplete() {
            var query = new QueryRootQuery();
            DefaultQueries.checkout.Completed(query, CurrentCheckout.id());

            Client.Query(query, (response, error) => {
                if (error != null) {
                    OnFailure(error);
                } else {
                    var checkout = (Checkout) response.node();
                    if (checkout.completedAt() != null) {
                        OnSuccess();
#if UNITY_EDITOR
                        DestroyImmediate(this);
#else
                        Destroy(this);
#endif
                    } else {
                        OnCancelled();
#if UNITY_EDITOR
                        DestroyImmediate(this);
#else
                        Destroy(this);
#endif
                    }
                }
            });
        }
    }
}
