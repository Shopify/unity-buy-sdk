namespace Shopify.UIToolkit {
    using UnityEngine;
    using UnityEngine.UI;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using System.Collections.Generic;
    using System;

    public class GenericCartView : MonoBehaviour {
        public IShop Shop;
        public CartItemView CartItemTemplate;
        public ScrollRect CartItemList;

        private RectTransform _scrollContent {
            get {
                return CartItemList.content;
            }
        }

        #region MonoBehaviour

        void Awake() {}

        #endregion

        #region Events
        void OnQuantityChanged(int quantity) {
        }

        void OnCartItemsChanged(List<CartItem> cartItems) {
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
                itemView.transform.SetParent(_scrollContent.transform, false);
                itemView.gameObject.SetActive(true);
            }
        }
        
        #endregion
    }
}
