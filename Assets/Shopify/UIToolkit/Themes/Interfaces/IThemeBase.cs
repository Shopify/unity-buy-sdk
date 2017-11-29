namespace Shopify.UIToolkit {
    using Shopify.Unity.SDK;

    public interface IThemeBase {
        void OnLoadingStarted();
        void OnLoadingFinished();
        void OnError(ShopifyError error);
        void OnPurchaseStarted();
        void OnPurchaseCancelled();
        void OnPurchaseCompleted();
    }
}

