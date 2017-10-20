package com.shopify.unity.buy.androidpay;

import android.support.annotation.VisibleForTesting;

import com.shopify.buy3.pay.PayCart;
import com.shopify.buy3.pay.PayHelper;
import com.shopify.unity.buy.ShopifyUnityPlayerActivity;
import com.shopify.unity.buy.models.PricingLineItems;
import com.shopify.unity.buy.utils.Logger;
import com.unity3d.player.UnityPlayer;

import org.json.JSONException;

import static com.shopify.unity.buy.MessageCenter.init;

public final class AndroidPayCheckoutSession {

    private final ShopifyUnityPlayerActivity rootActivity;
    private final boolean testing;

    public AndroidPayCheckoutSession(String unityDelegateObjectName, boolean testing) {
        this((ShopifyUnityPlayerActivity) UnityPlayer.currentActivity,
                unityDelegateObjectName, testing);
    }

    @VisibleForTesting
    AndroidPayCheckoutSession(ShopifyUnityPlayerActivity rootActivity,
                              String unityDelegateObjectName, boolean testing) {
        this.rootActivity = rootActivity;
        this.testing = testing;
        Logger.setEnabled(testing);
        init(unityDelegateObjectName);
    }

    public boolean checkoutWithAndroidPay(
            String merchantName,
            String publicKey,
            String pricingLineItemsString,
            String currencyCode,
            String countryCode,
            boolean requiresShipping
    ) {
        if (!PayHelper.isAndroidPayEnabledInManifest(rootActivity)) {
            // TODO: Send unsupported error to Unity
            return false;
        }

        final String msg = "merchantName = " + merchantName + "\n" +
                "publicKey = " + publicKey + "\n" +
                "pricingLineItemsString = " + pricingLineItemsString + "\n" +
                "currencyCode = " + currencyCode + "\n" +
                "countryCode = " + countryCode + "\n" +
                "requiresShipping = " + requiresShipping + "\n";
        Logger.debug(msg);

        try {
            // TODO: Pull in updated buy3 SDK which will use the countryCode.
            PayCart cart = cartFromUnity(merchantName, pricingLineItemsString, currencyCode,
                    requiresShipping);

            rootActivity.startAndroidPayCheckout(cart, publicKey, testing);
            return true;
        } catch (JSONException e) {
            Logger.error("Failed to parse summary items from Unity!");
            return false;
        }
    }

    private PayCart cartFromUnity(
            String merchantName,
            String pricingLineItemsString,
            String currencyCode,
            boolean requiresShipping
    ) throws JSONException {
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
