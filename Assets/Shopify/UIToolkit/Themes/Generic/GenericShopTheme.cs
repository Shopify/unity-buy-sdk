namespace Shopify.UIToolkit.Themes {
    using System.Collections;
    using System.Collections.Generic;
    using Shopify.Unity;
    using UnityEngine;

    public class GenericShopTheme : MonoBehaviour, IShopTheme {
        void IShopTheme.OnCartItemsChanged(CheckoutLineItem[] lineItems) {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnCartQuantityChanged(int newQuantity) {
            throw new System.NotImplementedException();
        }

        void IShopTheme.OnCollectionFilterChanged(Collection collection) {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnError(ShopifyError error) {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnLoadingFinished() {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnLoadingStarted() {
            throw new System.NotImplementedException();
        }

        void IShopTheme.OnProductListChanged(Product[] products) {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnPurchaseCancelled() {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnPurchaseCompleted() {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnPurchaseFailed(ShopifyError error) {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnPurchaseStarted() {
            throw new System.NotImplementedException();
        }

        void IShopTheme.OnShouldShowCart(CheckoutLineItem[] lineItems) {
            throw new System.NotImplementedException();
        }

        void IShopTheme.OnShouldShowCollectionList(Collection[] collections) {
            throw new System.NotImplementedException();
        }

        void IShopTheme.OnShouldShowProductDetails(Product product, ProductVariant[] variants) {
            throw new System.NotImplementedException();
        }

        void IShopTheme.ShouldShowProductList(Product[] products) {
            throw new System.NotImplementedException();
        }
    }
}