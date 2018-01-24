namespace Shopify.UIToolkit {
    using Shopify.Unity;
    using System.Collections.Generic;

    public interface IMultiProductShop : IShop {
        /// <summary>
        /// Called when products have been loaded by the controller.
        /// This usually happens asynchronously after you have told the controller to load products via
        /// `ShopController.LoadProducts()`
        /// </summary>
        /// <param name="products">
        /// The products that were loaded, the length of the array will match the count sent to `ShopController.LoadProducts()`
        /// </param>
        /// <param name="after">
        /// Because products load in pages, `after` is returned from the GraphQL API with the product ID
        /// at the end of the fetched page.
        /// </param>
        void OnProductsLoaded(Product[] products, string after);
    }
}
