using Shopify.Unity;

namespace Shopify.UIToolkit {
    /// <summary>
    /// A base class for a theme that sells products from a single collection
    /// </summary>
    public abstract class CollectionTheme : ThemeBase {
        /// <summary>
        /// Called when the implementor should show the UI for selecting products from a list.
        /// </summary>
        /// <param name="products">The products that the UI should list</param>
        public abstract void ShouldShowProductList(Product[] products);

        /// <summary>
        /// Called when the implementor should show the UI for a particular product and its associated variants.
        /// </summary>
        /// <param name="product">The product to show details for</param>
        /// <param name="variants">The variants of the product</param>
        public abstract void OnShouldShowProductDetails(Product product, ProductVariant[] variants);

        /// <summary>
        /// Called when the implementor should show the cart UI with the provided line items.
        /// </summary>
        /// <param name="lineItems">The line items to display in the cart</param>
        public abstract void OnShouldShowCart(CheckoutLineItem[] lineItems);

        /// <summary>
        /// Called when the total number of items in the cart has updated. 
        /// Good for showing a badge on the cart button, for example.
        /// </summary>
        /// <param name="newQuantity">The new quantity of items in the cart</param>
        public abstract void OnCartQuantityChanged(int newQuantity);

        /// <summary>
        /// Called when the Items in the cart are updated. 
        /// Useful if your cart is always displayed, for example, if it is in a sidebar.
        /// </summary>
        /// <param name="lineItems">The new line items in the cart</param>
        public abstract void OnCartItemsChanged(CheckoutLineItem[] lineItems);
    }
}
