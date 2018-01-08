namespace Shopify.UIToolkit {
    using System.Collections;
    using System.Collections.Generic;
    using Shopify.Unity.SDK;
    using Shopify.Unity;
    using UnityEngine;

    public interface IThemeBase {
        /// <summary>
        /// Called when the controller has started loading data from shopify.
        /// Themes can use this to know when to show loading states or spinners.
        /// </summary>
        void OnLoadingStarted();

        /// <summary>
        /// Called when the controller has finished loading data from shopify.
        /// Themes can use this to dismiss loading states or spinners.
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
        /// Called when the total quantity of items in the cart changes.
        /// Useful for when showing a badge next to the cart button for example.
        /// </summary>
        /// <param name="newQuantity">The new number of items in the cart</param>
        void OnCartQuantityChanged(int newQuantity);
    }
}
