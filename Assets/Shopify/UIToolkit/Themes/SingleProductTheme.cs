namespace Shopify.UIToolkit {
    using Shopify.Unity;

    /// <summary>
    /// Override this class to create a theme that sells a single product
    /// </summary>
    public abstract class SingleProductTheme : ThemeBase {
        /// <summary>
        /// Called when the theme should show information about the product it's selling
        /// </summary>
        /// <param name="product">The product that should be displayed</param>
        /// <param name="variants">The variants of the product that should be displayed</param>
        public abstract void OnShouldShowProduct(Product product, ProductVariant[] variants);
    }
}
