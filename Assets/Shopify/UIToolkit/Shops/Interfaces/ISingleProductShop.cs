namespace Shopify.UIToolkit {
    using Shopify.Unity;

    /// <summary>
    /// Override this class to create a shop that sells a single product
    /// </summary>
    public interface ISingleProductShop : IShop {
        /// <summary>
        /// Called when the controller is finished loading the product that the shop is selling.
        /// </summary>
        /// <param name="product">The product that was loaded</param>
        /// <param name="variants">The variants of the product that was loaded</param>
        void OnProductLoaded(Product product, ProductVariant[] variants);
    }
}
