namespace Shopify.UIToolkit {
    using System.Collections;
    using System.Collections.Generic;
    using Shopify.Unity.SDK;
    using Shopify.Unity;
    using UnityEngine;

    public interface IShop {
        /// <summary>
        /// Called when the controller has started loading data from shopify.
        /// Shops can use this to know when to show loading states or spinners.
        /// </summary>
        void OnLoadingStarted();

        /// <summary>
        /// Called when the controller has finished loading data from shopify.
        /// Shops can use this to dismiss loading states or spinners.
        /// </summary>
        void OnLoadingFinished();

        /// <summary>
        /// Called when a request initiated by the controller returns an error
        /// </summary>
        /// <param name="error">The error that occured</param>
        void OnError(ShopifyError error);

        /// <summary>
        /// Called when a purchase has started in the controller.
        /// Useful for changing the UI when a purchase is ongoing.
        /// </summary>
        void OnPurchaseStarted();

        /// <summary>
        /// Called when an ongoing purchase is cancelled.
        /// This either occurs when a native payment method is cancelled or the purchase is cancelled manually.
        /// </summary>
        void OnPurchaseCancelled();

        /// <summary>
        /// Called when an ongoing purchase completes.
        /// When using the Web purchase method, it's possible this will never be called
        /// if the player navigates away from the browser and doesn't complete the purchase.
        /// In this case you should always show a UI for cancelling the purchase in-game.
        /// </summary>
        void OnPurchaseCompleted();

        /// <summary>
        /// Called when an ongoing purchase fails to complete.
        /// This can occur if the cart is invalid and a checkout cannot be created from it.
        /// If it's a native payment (ie: Apple Pay), then this can also occur if the 
        /// payment fails to process. 
        /// </summary>
        void OnPurchaseFailed(ShopifyError error);

        /// <summary>
        /// Called when there was a change in the total number of items in the cart.
        /// </summary>
        /// <param name="totalNumberOfCartItems"></param>
        void OnCartQuantityChanged(int totalNumberOfCartItems);

        /// <summary>
        /// Called when the Items in the cart have updated. 
        /// </summary>
        /// <param name="lineItems">The new line items in the cart</param>
        void OnCartItemsChanged(List<CartItem> lineItems);

        /// <summary>
        /// Call to update the quantity of the given variant in the cart.
        /// </summary>
        /// <param name="variant">Variant object that is currently in the user's Cart.</param>
        /// <param name="product">The product that the variant belongs to.</param>
        /// <param name="quantity">The quantity value to change the cart item to.</param>
        // void UpdateCartQuantityForVariant(ProductVariant variant, Product product, long quantity);
    }
}
