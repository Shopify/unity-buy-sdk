namespace Shopify.UIToolkit {
    using Shopify.Unity;

    /// <summary>
    /// Override this class to create a shop that sells a single product
    /// </summary>
    public interface ISingleProductShop : IShop {
        /// <summary>
        /// Called when the shop should show information about the product it's selling
        /// </summary>
        /// <param name="product">The product that should be displayed</param>
        /// <param name="variants">The variants of the product that should be displayed</param>
        void OnShouldShowProduct(Product product, ProductVariant[] variants);
    }
}
