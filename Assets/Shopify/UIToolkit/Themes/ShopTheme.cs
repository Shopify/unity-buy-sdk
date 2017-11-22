namespace Shopify.UIToolkit {
    using Shopify.Unity;

    public abstract class ShopTheme : ThemeBase {
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

        /// <summary>
        /// Called when the items in the product list changed because a filter was applied. 
        /// The implementor should update the products in the product list to match the supplied products.
        /// </summary>
        /// <param name="products">The new products to display in the list</param>
        public abstract void OnProductListChanged(Product[] products);

        /// <summary>
        /// Called when the selected collection filter for the product list has been changed
        /// </summary>
        /// <param name="collection">The collection that is now selected as a filter, null if no collection is selected.</param>
        public abstract void OnCollectionFilterChanged(Collection collection);

        /// <summary>
        /// Called when the list of available collections is loaded and the implementor should make them available as selections to the user.
        /// For example, populating a dropdown with the names or displaying them in a sidebar.
        /// </summary>
        /// <param name="collections">The collections that should be displayed</param>
        public abstract void OnShouldShowCollectionList(Collection[] collections);
    }
}
