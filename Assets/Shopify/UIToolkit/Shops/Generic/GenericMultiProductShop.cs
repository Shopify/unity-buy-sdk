namespace Shopify.UIToolkit.Themes {
    using System.Collections;
    using System.Collections.Generic;
    using Shopify.Unity;
    using UnityEngine;

    public class GenericMultiProductShop : MonoBehaviour, IMultiProductShop {
        void IMultiProductShop.OnCartItemsChanged(CheckoutLineItem[] lineItems) {
            throw new System.NotImplementedException();
        }

        void IMultiProductShop.OnCollectionFilterChanged(Collection collection) {
            throw new System.NotImplementedException();
        }

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

        void IMultiProductShop.OnProductListChanged(Product[] products) {
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

        void IMultiProductShop.OnShouldShowCart(CheckoutLineItem[] lineItems) {
            throw new System.NotImplementedException();
        }

        void IMultiProductShop.OnShouldShowCollectionList(Collection[] collections) {
            throw new System.NotImplementedException();
        }

        void IMultiProductShop.OnShouldShowProductDetails(Product product, ProductVariant[] variants) {
            throw new System.NotImplementedException();
        }

        void IMultiProductShop.ShouldShowProductList(Product[] products) {
            throw new System.NotImplementedException();
        }
    }
}