namespace Shopify.UIToolkit {
    using Shopify.Unity;

    public interface IMultiProductShop : IShop {
        /// <summary>
        /// Called when products have been loaded by the controller.
        /// This usually happens asynchronously after you have told the controller to load products via
        /// `ShopController.LoadProducts()`
        /// </summary>
        /// <param name="products">
        /// The products that were loaded, the length of the array will match the count sent to `ShopController.LoadProducts()`
        /// </param>
        /// <param name="cursor">
        /// Because products load in pages, the cursor is the index of the first product in the returned list.
        /// It will match the cursor you send to `ShopController.LoadProducts()` and is there to help you position
        /// the newly loaded products in your UI.
        /// </param>
        void OnProductsLoaded(Product[] products, int cursor);

        /// <summary>
        /// Called when the Items in the cart have updated. 
        /// </summary>
        /// <param name="lineItems">The new line items in the cart</param>
        void OnCartItemsChanged(CheckoutLineItem[] lineItems);
    }
}
