namespace Shopify.Unity.SDK {
    interface INativeCheckout {
        /// <summary>
        /// Check whether the device supports displaying a native wallet application to setup the user's payment card
        /// </summary>
        /// <param name="paymentSettings">The Shop's payment settings</param>
        /// <returns>True if the device supports displaying a native wallet application</returns>
        bool CanShowPaymentSetup(PaymentSettings paymentSettings);

        /// <summary>
        /// Invokes a native method that will prompt the user to setup the user's payment card via the native wallet application
        /// </summary>
        void ShowPaymentSetup();

        /// <summary>
        /// Check whether the device supports making a native payment given the Shop's payment settings
        /// </summary>
        /// <param name="paymentSettings">The Shop's payment settings</param>
        /// <param name="callback">The callback that will deliver the result to tha caller</param>
        /// <returns>True if the device supports making a native payment</returns>
        void CanCheckout(PaymentSettings paymentSettings, CanCheckoutWithNativePayCallback callback);

        /// <summary>
        /// Display the native payment sheet to perform a native payment
        /// </summary>
        /// <param name="key">The public key that is used for encrypting a native payment's token data</param>
        /// <param name="shopMetadata">The shop's metadata containing name and payment settings</param>
        /// <param name="success">The closure that is invoked upon success of the payment</param>
        /// <param name="canceled">The closure that is invoked upon cancelation of the payment</param>
        /// <param name="failure">The closure that is invoked upon failure of the payment</param>
        void Checkout(string key, ShopMetadata shopMetadata, CheckoutSuccessCallback success, CheckoutCancelCallback canceled, CheckoutFailureCallback failure);
    }
}