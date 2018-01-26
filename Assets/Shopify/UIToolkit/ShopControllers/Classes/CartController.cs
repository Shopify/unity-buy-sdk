namespace Shopify.UIToolkit {
    using UnityEngine;
    using UnityEngine.Events;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    // A wrapper struct that encapsulates the information we want from a CartLineItem (Quantity) and the
    // ProductVariant it has to help with constructing the UI for the cart.
    /// </summary>
    public struct CartItem {
        public ProductVariant Variant;
        public Product Product;
        public long Quantity;

        public CartItem(ProductVariant variant, Product product, long quantity) {
            Product = product;
            Variant = variant;
            Quantity = quantity;
        }
    }

    [System.Serializable]
    public class CartController {
        [System.Serializable]
        public class QuantityChangeEvent : UnityEvent<int> {}

        [System.Serializable]
        public class PurchaseFailedEvent : UnityEvent<ShopifyError> {}

        [System.Serializable]
        public class CartItemsChangeEvent : UnityEvent<List<CartItem>> {}

        private Cart Cart;
        public UnityEvent OnPurchaseStarted = new UnityEvent();
        public QuantityChangeEvent OnQuantityChange = new QuantityChangeEvent();
        public CartItemsChangeEvent OnCartItemsChange = new CartItemsChangeEvent();
        public UnityEvent OnPurchaseComplete = new UnityEvent();
        public UnityEvent OnPurchaseCancelled = new UnityEvent();
        public PurchaseFailedEvent OnPurchaseFailed = new PurchaseFailedEvent();

        private string _appleMerchantID;

        private struct ProductAndVariantPair {
            public readonly Product Product;
            public readonly ProductVariant Variant;

            public ProductAndVariantPair(Product product, ProductVariant variant) {
                Product = product;
                Variant = variant;
            }
        }

        private Dictionary<string, ProductAndVariantPair> _idsToProductPairs = new Dictionary<string, ProductAndVariantPair>();

        public CartController(Cart cart, string appleMerchantID) {
            SetCart(cart);
            _appleMerchantID = appleMerchantID;
        }

        public void SetCart(Cart cart) {
            Cart = cart;
            _idsToProductPairs.Clear();
        }

        public CartLineItems LineItems {
            get {
                return Cart.LineItems;
            }
        }

        public List<CartItem> CartItems {
            get {
                var items = new List<CartItem>();
                foreach (var lineItem in LineItems.All()) {
                    var pair = _idsToProductPairs[lineItem.VariantId];
                    items.Add(new CartItem(pair.Variant, pair.Product, lineItem.Quantity));
                }
                return items; 
            }
        }

        /// <summary>
        /// Adds a variant to the cart
        /// </summary>
        /// <param name="variant">The variant to add to the cart</param>
        /// <param name="product">The product the variant belongs to</param>
        public void AddVariant(ProductVariant variant, Product product) {
            var existingItem = Cart.LineItems.Get(variant);
            var newQuantity = existingItem == null ? 1 : existingItem.Quantity + 1;
            Cart.LineItems.AddOrUpdate(variant, newQuantity);
            CacheProductVariantPair(variant, product);

            OnQuantityChange.Invoke(TotalItemsInCart());
            OnCartItemsChange.Invoke(CartItems);
        }

        /// <summary>
        /// Sets the variant to the specified quantity in the cart.
        /// </summary>
        /// <param name="variant">The variant to modify</param>
        /// <param name="product">The product the variant belongs to</param>
        /// <param name="quantity">The desired quantity</param>
        public void UpdateVariant(ProductVariant variant, Product product, long quantity) {
            if (quantity <= 0) {
                Cart.LineItems.Delete(variant);
                RemoveProductVariantPairByIDFromCache(variant.id());
            } else {
                Cart.LineItems.AddOrUpdate(variant, quantity);
                CacheProductVariantPair(variant, product);
            }

            OnQuantityChange.Invoke(TotalItemsInCart());
            OnCartItemsChange.Invoke(CartItems);
        }

        /// <summary>
        /// Removes a variant from the cart.
        /// </summary>
        /// <param name="variant">The variant to remove from the cart</param>
        /// <param name="product">The product the variant belongs to</param>
        public void RemoveVariant(ProductVariant variant, Product product) {
            var existingItem = Cart.LineItems.Get(variant);
            if (existingItem == null) return;
            var newQuantity = existingItem.Quantity - 1;

            if (newQuantity == 0) {
                Cart.LineItems.Delete(variant);
                RemoveProductVariantPairByIDFromCache(variant.id());
            } else {
                Cart.LineItems.AddOrUpdate(variant, newQuantity);
                CacheProductVariantPair(variant, product);
            }

            OnQuantityChange.Invoke(TotalItemsInCart());
            OnCartItemsChange.Invoke(CartItems);
        }

        /// <summary>
        /// Clears all items from the cart
        /// </summary>
        public void ClearCart() {
            Cart.Reset();
            _idsToProductPairs.Clear();
            OnQuantityChange.Invoke(0);
            OnCartItemsChange.Invoke(CartItems);
        }

        /// <summary>
        /// Start a purchase with the current cart.
        /// </summary>
        /// <param name="mode">How do you want to make the purchase? Native, Web or Auto</param>
        public void StartPurchase(CheckoutMode mode) {

            // Validate that the developer has given us the proper native payment key for the platform.
            var nativePayKey = NativePayKeyForCurrentPlatform();
            if ((mode == CheckoutMode.Native || mode == CheckoutMode.Auto) && nativePayKey == null) {
                throw new ArgumentException("Missing native payment key for current platform or platform does not support native pay.");
                return;
            }

            OnPurchaseStarted.Invoke();

            switch (mode) {
                case CheckoutMode.Native:
                    Cart.CheckoutWithNativePay(nativePayKey, OnPurchaseComplete.Invoke, OnPurchaseCancelled.Invoke, OnPurchaseFailed.Invoke);
                    break;

                case CheckoutMode.Web:
                    Cart.CheckoutWithWebView(OnPurchaseComplete.Invoke, OnPurchaseCancelled.Invoke, OnPurchaseFailed.Invoke);
                    break;

                case CheckoutMode.Auto:
                    Cart.CanCheckoutWithNativePay((canCheckoutWithNativePay) => {
                        if (canCheckoutWithNativePay) {
                            Cart.CheckoutWithNativePay(nativePayKey, OnPurchaseComplete.Invoke, OnPurchaseCancelled.Invoke, OnPurchaseFailed.Invoke);
                        } else {
                            Cart.CheckoutWithWebView(OnPurchaseComplete.Invoke, OnPurchaseCancelled.Invoke, OnPurchaseFailed.Invoke);
                        }
                    });
                    break;
            }
        }

        private string NativePayKeyForCurrentPlatform() {
            #if UNITY_IOS
            return _appleMerchantID;
            #else
            return null;
            #endif
        }

        private int TotalItemsInCart() {
            return Cart.LineItems.All().Sum((x) => (int) x.Quantity);
        }

        private void CacheProductVariantPair(ProductVariant variant, Product product) {
            if (!_idsToProductPairs.ContainsKey(variant.id())) {
                var pair = new ProductAndVariantPair(product, variant);
                _idsToProductPairs.Add(variant.id(), pair);
            }
        }

        private void RemoveProductVariantPairByIDFromCache(string variantId) {
            _idsToProductPairs.Remove(variantId);
        }
    }
}
