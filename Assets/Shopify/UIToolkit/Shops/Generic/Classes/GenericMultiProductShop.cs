namespace Shopify.UIToolkit.Themes {
    using System.Collections;
    using System.Collections.Generic;
    using Shopify.Unity;
    using UnityEngine;

    public class GenericMultiProductShop : MonoBehaviour, IMultiProductShop {
        void IShop.OnCartQuantityChanged(int newQuantity) {
            throw new System.NotImplementedException();
        }

        void IShop.OnError(ShopifyError error) {
            throw new System.NotImplementedException();
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

        void IMultiProductShop.OnCartItemsChanged(CheckoutLineItem[] lineItems) {
            throw new System.NotImplementedException();
        }

        void IMultiProductShop.OnProductsLoaded(Product[] products, int cursor) {
            throw new System.NotImplementedException();
        }
    }
}