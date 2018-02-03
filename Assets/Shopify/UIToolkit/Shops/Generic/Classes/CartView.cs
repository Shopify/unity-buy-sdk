namespace Shopify.UIToolkit.Shops.Generic {
    using UnityEngine;
    using UnityEngine.UI;
    using Shopify.Unity;
    using Shopify.Unity.UI;
    using Shopify.Unity.SDK;
    using System.Collections.Generic;
    using System;

    public class CartView : GenericMultiProductShopView {
        public CartItemView CartItemTemplate;
        public ScrollRect CartItemList;
        public Text EmptyLabel;
        public NativePayButtonUI NativePayButton;
        public Button CheckoutButton;

        private List<CartItem> _cartItems = new List<CartItem>();

        private List<CartItemView> _cartItemViews = new List<CartItemView>();

        private RectTransform _scrollContent {
            get {
                return CartItemList.content;
            }
        }

        #region MonoBehaviour

        void Start() {
            Shop.CanCheckoutWithNativePay((canCheckout) => {
                NativePayButton.gameObject.SetActive(canCheckout);
            });
        }

        void OnEnable() {
            UpdateList();
        }

        #endregion

        #region Events

        void OnQuantityChanged(int quantity) {}

        public void OnCartItemsChanged(List<CartItem> cartItems) {
            var diff = cartItems.Count - _cartItems.Count;
            while (diff < 0) {
                var lastIndex = _cartItemViews.Count - 1;
                var itemView = _cartItemViews[lastIndex];
                GameObject.Destroy(itemView.gameObject);
                _cartItemViews.RemoveAt(lastIndex);
                diff += 1;
            }

            while (diff > 0) {
                var itemView = InstantiateCartItemView();
                _cartItemViews.Add(itemView);
                diff -= 1;
            }

            _cartItems = cartItems;

            if (gameObject.active) {
                UpdateList();
            }
        }

        public void PerformWebCheckout() {
            Shop.PerformWebCheckout();
        }

        public void PerformNativeCheckout() {
            Shop.PerformNativeCheckout();
        }

        private void UpdateList() {
            if (_cartItems.Count == 0) {
                CartItemList.gameObject.SetActive(false);
                EmptyLabel.gameObject.SetActive(true);
                CheckoutButton.interactable = false;
                return;
            } else {
                CartItemList.gameObject.SetActive(true);
                EmptyLabel.gameObject.SetActive(false);
                CheckoutButton.interactable = true;
            }

            for (var i = 0; i < _cartItems.Count; i++) {
                var item = _cartItems[i];
                var itemView = _cartItemViews[i];
                itemView.SetCartItem(item);
            }
        }

        #endregion

        #region Helpers

        private CartItemView InstantiateCartItemView() {
            var itemView = Instantiate(CartItemTemplate);
            itemView.gameObject.SetActive(true);
            itemView.transform.SetParent(_scrollContent.transform, false);
            itemView.OnQuantityChange.AddListener(UpdateQuantity);
            return itemView;
        }

        private void UpdateQuantity(ProductVariant variant, Product product, long quantity) {
            Shop.UpdateCartQuantityForVariant(variant, product, quantity);
        }
        
        #endregion
    }
}
