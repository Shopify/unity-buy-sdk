package com.shopify.unity.buy;

import com.shopify.buy3.pay.PayCart;
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

    public AndroidPayCheckoutSession(ILogger logger) {
        this.logger = logger;
    }

    public boolean checkoutWithAndroidPay(
            String unityDelegateObjectName,
            String merchantName,
            String pricingLineItemsString,
            String currencyCode,
            String countryCode,
            boolean requiresShipping
    ) {
        try {
            PayCart cart = cartFromUnity(merchantName, pricingLineItemsString, currencyCode, countryCode,
                    requiresShipping);
            addAndroidPayFragment(unityDelegateObjectName, cart, countryCode);
            return true;
        } catch (IOException e) {
            logger.error("ShopifyBuyPlugin", "Failed to parse summary items from Unity");
            e.printStackTrace();
            return false;
        }
    }

    public PayCart cartFromUnity(
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

    private void addAndroidPayFragment(String unityDelegateObjectName, PayCart cart, String countryCode) {
        UnityAndroidPayFragment payFragment =
                UnityAndroidPayFragment.newInstance(unityDelegateObjectName, cart, countryCode);
        UnityPlayer.currentActivity.getFragmentManager()
                .beginTransaction()
                .add(payFragment, "payFragment")
                .commit();
    }
}
