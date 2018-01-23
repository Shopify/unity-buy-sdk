namespace Shopify.UIToolkit.Shops {
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.UIToolkit;
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Globalization;
    using Shopify.UIToolkit.Shops.Generic;

    [RequireComponent(typeof(SingleProductShopController))]
    public class GenericSingleProductShop : MonoBehaviour, ISingleProductShop {
        public ProductDetailsViewBindings ViewBindings;

        private SingleProductShopController _controller;


        #region Mono Behaviour
        
        void Awake() {
            _controller = GetComponent<SingleProductShopController>();
            _controller.Shop = this;
            _controller.OnShow();
        }

        #endregion

        #region IShop Interface

        void IShop.OnLoadingStarted() {
            throw new System.NotImplementedException(); 
        }

        void IShop.OnLoadingFinished() {
            throw new System.NotImplementedException(); 
        }

        void IShop.OnError(ShopifyError error) {
            throw new System.NotImplementedException(); 
        }

        void IShop.OnPurchaseStarted() {
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

        void IShop.OnCartQuantityChanged(int totalNumberOfCartItems) {
            throw new System.NotImplementedException();
        }

        void IShop.OnCartItemsChanged(List<CartItem> cartItems) {
            throw new System.NotImplementedException();
        }

        void IShop.UpdateCartQuantityForVariant(ProductVariant variant, long quantity) {
            throw new System.NotImplementedException();
        }

        void ISingleProductShop.OnProductLoaded(Product product, ProductVariant[] variants) {
            ViewBindings.FillWithProductWithVariants(product, variants);
            ViewBindings.OnAddToCartClicked.AddListener(() => {
                OnBuyNowClicked(ViewBindings.CurrentSelectedVariant);
            });
        }

        #endregion

        #region events

        public void OnBuyNowClicked(ProductVariant variant) {
            _controller.Cart.AddVariant(variant);
            _controller.Cart.StartPurchase(CheckoutMode.Auto);
        }

        #endregion
    }
}
