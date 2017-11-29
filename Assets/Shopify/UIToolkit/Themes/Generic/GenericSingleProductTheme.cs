namespace Shopify.UIToolkit.Themes {
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using UnityEngine;

    [RequireComponent(typeof(SingleProductThemeController))]
    public class GenericSingleProductTheme : MonoBehaviour, ISingleProductTheme {
        void IThemeBase.OnError(ShopifyError error) {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnLoadingFinished() {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnLoadingStarted() {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnPurchaseCancelled() {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnPurchaseCompleted() {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnPurchaseStarted() {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnCartQuantityChanged(int newQuantity) {
            throw new System.NotImplementedException();
        }

        void ISingleProductTheme.OnShouldShowProduct(Product product, ProductVariant[] variants) {
            throw new System.NotImplementedException();
        }
    }
}