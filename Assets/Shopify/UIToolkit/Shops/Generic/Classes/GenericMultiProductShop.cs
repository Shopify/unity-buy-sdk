namespace Shopify.UIToolkit.Shops {
    using System.Collections;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.UIToolkit;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(MultiProductShopController))]
    public class GenericMultiProductShop : MonoBehaviour, IMultiProductShop {

        public MultiProductListItem ListItemTemplate;
        public ScrollRect ListView;

        private MultiProductShopController _controller;

        private RectTransform _scrollContentRect {
            get { return ListView.content; }
        }

        private RectTransform _viewportRect {
            get { return ListView.viewport; }
        }

        void Awake() {
            _controller = GetComponent<MultiProductShopController>();
            _controller.Show();
        }

        void IMultiProductShop.OnCartItemsChanged(CheckoutLineItem[] lineItems) {
            throw new System.NotImplementedException();
        }

        void IShop.OnCartQuantityChanged(int newQuantity) {
            throw new System.NotImplementedException();
        }

        void IShop.OnError(ShopifyError error) {
            Debug.Log(error.Description);
        }

        void IShop.OnLoadingFinished() {
            throw new System.NotImplementedException();
        }

        void IShop.OnLoadingStarted() {
            throw new System.NotImplementedException();
        }

        void IShop.OnPurchaseCancelled() {
            throw new System.NotImplementedException();
        }

        void IShop.OnPurchaseCompleted() {
            throw new System.NotImplementedException();
        }

        void IShop.OnPurchaseFailed(ShopifyError error) {
            throw new System.NotImplementedException();
        }

        void IShop.OnPurchaseStarted() {
            throw new System.NotImplementedException();
        }

        void IMultiProductShop.OnProductsLoaded(Product[] products, string after) {
            foreach (var product in products) {
                var listItem = Instantiate(ListItemTemplate);
                listItem.transform.SetParent(_scrollContentRect.transform, false);
                listItem.SetProduct(product);
            }
        }
    }
}
