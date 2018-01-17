namespace Shopify.UIToolkit.Shops {
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using UnityEngine;

    [RequireComponent(typeof(SingleProductShopController))]
    public class GenericSingleProductShop : MonoBehaviour, ISingleProductShop {
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

        void IShop.OnPurchaseStarted() {
            throw new System.NotImplementedException();
        }

        void IShop.OnPurchaseFailed(ShopifyError error) {
            throw new System.NotImplementedException();
        }

        void IShop.OnCartQuantityChanged(int newQuantity) {
            throw new System.NotImplementedException();
        }

        void ISingleProductShop.OnProductLoaded(Product product, ProductVariant[] variants) {
            throw new System.NotImplementedException();
        }
    }
}
