package com.shopify.unity.buy;

import com.google.android.gms.wallet.WalletConstants;
import com.shopify.buy3.pay.PayCart;
import com.shopify.buy3.pay.PayHelper;
import com.shopify.unity.buy.models.PricingLineItems;
import com.shopify.unity.buy.utils.AndroidLogger;
import com.shopify.unity.buy.utils.ILogger;
import com.unity3d.player.UnityPlayer;

import java.io.IOException;

public final class AndroidPayCheckoutSession {
    private ILogger logger;

    public AndroidPayCheckoutSession() {
        this.logger = new AndroidLogger();
    }

    AndroidPayCheckoutSession(ILogger logger) {
        this.logger = logger;
    }

    //CHECKSTYLE:OFF
    public boolean checkoutWithAndroidPay(
            String unityDelegateObjectName,
            String merchantName,
            String publicKey,
            String pricingLineItemsString,
            String currencyCode,
            String countryCode,
            boolean requiresShipping,
            boolean testing
    ) {
        //CHECKSTYLE:ON
        if (!PayHelper.isAndroidPayEnabledInManifest(UnityPlayer.currentActivity)) {
            // TODO: Send unsupported error to Unity
            return false;
        }

        try {
            PayCart cart = cartFromUnity(merchantName, pricingLineItemsString, currencyCode, countryCode,
                    requiresShipping);

            UnityAndroidPayFragment payFragment = UnityAndroidPayFragment.builder()
                    .setUnityDelegateObjectName(unityDelegateObjectName)
                    .setPayCart(cart)
                    .setCountryCode(countryCode)
                    .setEnvironment(testing ?
                            WalletConstants.ENVIRONMENT_TEST : WalletConstants.ENVIRONMENT_PRODUCTION)
                    .setPublicKey(publicKey)
                    .build();

            UnityPlayer.currentActivity.getFragmentManager()
                    .beginTransaction()
                    .add(payFragment, "payFragment")
                    .commit();

            return true;
        } catch (IOException e) {
            logger.error("ShopifyBuyPlugin", "Failed to parse summary items from Unity!");
            return false;
        }
    }

    PayCart cartFromUnity(
        String merchantName,
        String pricingLineItemsString,
        String currencyCode,

        // TODO: Pull in updated buy3 SDK which will use the countryCode.
        String countryCode,
        boolean requiresShipping
    ) throws IOException {
        PricingLineItems items = PricingLineItems.fromJsonString(pricingLineItemsString);

        return PayCart.builder()
                .merchantName(merchantName)
                .currencyCode(currencyCode)
                .shippingAddressRequired(requiresShipping)
                .subtotal(items.subtotal)
                .shippingPrice(items.shippingPrice)
                .taxPrice(items.taxPrice)
                .totalPrice(items.totalPrice)
                .build();
    }
}
