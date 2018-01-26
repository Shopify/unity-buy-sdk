namespace Shopify.UIToolkit.Shops.Generic {
    using UnityEngine;
    using UnityEngine.UI;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using System.Collections.Generic;
    using System;

    public class CartView : GenericMultiProductShopView {
        public CartItemView CartItemTemplate;
        public ScrollRect CartItemList;
        public Text EmptyLabel;

        private List<CartItem> _cartItems = new List<CartItem>();

        private RectTransform _scrollContent {
            get {
                return CartItemList.content;
            }
        }

        #region MonoBehaviour

        void Awake() {}

        void OnEnable() {
            InvalidateListView();
        }

        #endregion

        #region Events
        void OnQuantityChanged(int quantity) {}

        public void OnCartItemsChanged(List<CartItem> cartItems) {
            _cartItems = cartItems;

            if (gameObject.active) {
                InvalidateListView();
            }
        }

        public void PerformWebCheckout() {
            Shop.PerformWebCheckout();
        }

        public void PerformNativeCheckout() {
            Shop.PerformNativeCheckout();
        }

        #endregion

        #region Helpers

        private void InvalidateListView() {
            foreach (Transform child in _scrollContent) {
                GameObject.Destroy(child.gameObject);
            }

            if (_cartItems.Count == 0) {
                CartItemList.gameObject.SetActive(false);
                EmptyLabel.gameObject.SetActive(true);
                return;
            } else {
                CartItemList.gameObject.SetActive(true);
                EmptyLabel.gameObject.SetActive(false);
            }

            foreach (var cartItem in _cartItems) {
                var itemView = Instantiate(CartItemTemplate);
                itemView.gameObject.SetActive(true);
                itemView.transform.SetParent(_scrollContent.transform, false);
                itemView.SetCartItem(cartItem);
                itemView.OnQuantityChange.AddListener(UpdateQuantity);
            }
        }

        private void UpdateQuantity(ProductVariant variant, Product product, long quantity) {
            Shop.UpdateCartQuantityForVariant(variant, product, quantity);
        }
        
        #endregion
    }
}
