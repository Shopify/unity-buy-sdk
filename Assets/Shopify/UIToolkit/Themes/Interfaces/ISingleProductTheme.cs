namespace Shopify.UIToolkit {
    using Shopify.Unity;

    public interface ISingleProductTheme : IThemeBase {
        void OnShouldShowProduct(Product product, ProductVariant[] variants);
    }
}
