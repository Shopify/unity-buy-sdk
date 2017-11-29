namespace Shopify.UIToolkit {
    using System.Collections;
    using System.Collections.Generic;
    using Shopify.Unity.SDK;
    using UnityEngine;

    public abstract class ThemeBase : MonoBehaviour, IThemeBase {
        /// <summary>
        /// Called when the controller has started loading data from shopify.
        /// Themes can use this to know when to show loading states or spinners.
        /// </summary>
        public abstract void OnLoadingStarted();

        /// <summary>
        /// Called when the controller has finished loading data from shopify.
        /// Themes can use this to dismiss loading states or spinners.
        /// </summary>
        public abstract void OnLoadingFinished();

        /// <summary>
        /// Called when a request initiated by the controller returns an error
        /// </summary>
        /// <param name="error">The error that occured</param>
        public abstract void OnError(ShopifyError error);

        /// <summary>
        /// Called when a purchase has started in the controller.
        /// Useful for changing the UI when a purchase is ongoing.
        /// </summary>
        public abstract void OnPurchaseStarted();

        /// <summary>
        /// Called when an ongoing purchase is cancelled.
        /// This either occurs when a native payment method is cancelled or the purchase is cancelled manually.
        /// </summary>
        public abstract void OnPurchaseCancelled();

        /// <summary>
        /// Called when an ongoing purchase completes.
        /// When using the Web purchase method, it's possible this will never be called
        /// if the player navigates away from the browser and doesn't complete the purchase.
        /// In this case you should always show a UI for cancelling the purchase in-game.
        /// </summary>
        public abstract void OnPurchaseCompleted();
    }
}
