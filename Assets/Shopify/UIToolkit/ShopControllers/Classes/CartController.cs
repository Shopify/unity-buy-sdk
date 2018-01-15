namespace Shopify.UIToolkit {
    using UnityEngine.Events;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using System.Linq;

    [System.Serializable]
    public class CartController {
        [System.Serializable]
        public class QuantityChangeEvent : UnityEvent<int> {}

        [System.Serializable]
        public class PurchaseFailedEvent : UnityEvent<ShopifyError> {}

        private Cart Cart;
        public UnityEvent OnPurchaseStarted = new UnityEvent();
        public QuantityChangeEvent OnQuantityChange = new QuantityChangeEvent();
        public UnityEvent OnPurchaseComplete = new UnityEvent();
        public UnityEvent OnPurhcaseCancelled = new UnityEvent();
        public PurchaseFailedEvent OnPurhchaseFailed = new PurchaseFailedEvent();

        public CartController(Cart cart) {
            SetCart(cart);
        }

        public void SetCart(Cart cart) {
            Cart = cart;
        }

        public CartLineItems LineItems {
            get {
                return Cart.LineItems;
            }
        }

        /// <summary>
        /// Adds a variant to the cart
        /// </summary>
        /// <param name="variant">The variant to add to the cart</param>
        public void AddVariant(ProductVariant variant) {
            var existingItem = Cart.LineItems.Get(variant);
            var newQuantity = existingItem == null ? 1 : existingItem.Quantity + 1;
            Cart.LineItems.AddOrUpdate(variant, newQuantity);

            OnQuantityChange.Invoke(TotalItemsInCart());
        }

        /// <summary>
        /// Sets the variant to the specified quantity in the cart.
        /// </summary>
        /// <param name="variant">The variant to modify</param>
        /// <param name="quantity">The desired quantity</param>
        public void UpdateVariant(ProductVariant variant, int quantity) {
            if (quantity <= 0) {
                Cart.LineItems.Delete(variant);
            } else {
                Cart.LineItems.AddOrUpdate(variant, quantity);
            }

            OnQuantityChange.Invoke(TotalItemsInCart());
        }

        /// <summary>
        /// Removes a variant from the cart.
        /// </summary>
        /// <param name="variant">The variant to remove from the cart</param>
        public void RemoveVariant(ProductVariant variant) {
            var existingItem = Cart.LineItems.Get(variant);
            if (existingItem == null) return;
            var newQuantity = existingItem.Quantity - 1;

            if (newQuantity == 0) {
                Cart.LineItems.Delete(variant);
            } else {
                Cart.LineItems.AddOrUpdate(variant, newQuantity);
            }

            OnQuantityChange.Invoke(TotalItemsInCart());
        }

        /// <summary>
        /// Clears all items from the cart
        /// </summary>
        public void ClearCart() {
            Cart.Reset();

            OnQuantityChange.Invoke(0);
        }

        /// <summary>
        /// Start a purchase with the current cart.
        /// </summary>
        /// <param name="mode">How do you want to make the purchase? Native, Web or Auto</param>
        /// <param name="nativePayKey">Vendor-specific key to be passed to Native purchase methods</param>
        public void StartPurchase(CheckoutMode mode, string nativePayKey = null) {
            OnPurchaseStarted.Invoke();
            
            switch (mode) {
                case CheckoutMode.Native:
                    Cart.CheckoutWithNativePay(nativePayKey, OnPurchaseComplete.Invoke, OnPurhcaseCancelled.Invoke, OnPurhchaseFailed.Invoke);
                    break;

                case CheckoutMode.Web:
                    Cart.CheckoutWithWebView(OnPurchaseComplete.Invoke, OnPurhcaseCancelled.Invoke, OnPurhchaseFailed.Invoke);
                    break;

                case CheckoutMode.Auto:
                    Cart.CanCheckoutWithNativePay((canCheckoutWithNativePay) => {
                        if (canCheckoutWithNativePay) {
                            Cart.CheckoutWithNativePay(nativePayKey, OnPurchaseComplete.Invoke, OnPurhcaseCancelled.Invoke, OnPurhchaseFailed.Invoke);
                        } else {
                            Cart.CheckoutWithWebView(OnPurchaseComplete.Invoke, OnPurhcaseCancelled.Invoke, OnPurhchaseFailed.Invoke);
                        }
                    });
                    break;
            }
        }

        private int TotalItemsInCart() {
            return Cart.LineItems.All().Sum((x) => (int) x.Quantity);
        }
    }
}
