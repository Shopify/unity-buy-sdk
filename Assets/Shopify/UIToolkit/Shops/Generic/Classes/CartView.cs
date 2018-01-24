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

        private RectTransform _scrollContent {
            get {
                return CartItemList.content;
            }
        }

        private new GenericMultiProductShop Shop {
            get {
                return Shop as GenericMultiProductShop;
            }
        }

        #region MonoBehaviour

        void Awake() {}

        #endregion

        #region Events
        void OnQuantityChanged(int quantity) {}

        public void OnCartItemsChanged(List<CartItem> cartItems) {
            UpdateCartList(cartItems);
        }

        #endregion

        #region Helpers

        private void UpdateCartList(List<CartItem> cartItems) {
            foreach (Transform child in _scrollContent) {
                GameObject.Destroy(child);
            }

            if (cartItems.Count == 0) {
                CartItemList.gameObject.SetActive(false);
                EmptyLabel.gameObject.SetActive(true);
                return;
            } else {
                CartItemList.gameObject.SetActive(true);
                EmptyLabel.gameObject.SetActive(false);
            }

            foreach (var cartItem in cartItems) {
                var itemView = Instantiate(CartItemTemplate);
                itemView.Shop = Shop;
                itemView.SetCartItem(cartItem);
                itemView.transform.SetParent(_scrollContent.transform, false);
                itemView.gameObject.SetActive(true);
            }
        }
        
        #endregion
    }
}
