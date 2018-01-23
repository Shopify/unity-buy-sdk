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

            foreach (var cartItem in cartItems) {
                var model = new CartItemViewModel(cartItem);
                var itemView = Instantiate(CartItemTemplate);
                itemView.SetCartItemViewModel(model);
                itemView.transform.SetParent(_scrollContent.transform, false);
                itemView.gameObject.SetActive(true);
            }
        }
        
        #endregion
    }
}
