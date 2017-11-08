package com.shopify.unity.buy.androidpay;

import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.annotation.VisibleForTesting;

import com.shopify.buy3.pay.PayCart;
import com.shopify.unity.buy.MessageCenter;
import com.shopify.unity.buy.ShopifyUnityPlayerActivity;
import com.shopify.unity.buy.models.PricingLineItems;
import com.shopify.unity.buy.utils.CardTypeConverter;
import com.shopify.unity.buy.utils.Logger;
import com.unity3d.player.UnityPlayer;

import org.json.JSONArray;
import org.json.JSONException;

import static com.shopify.unity.buy.MessageCenter.init;

public final class AndroidPayCheckoutSession {

    private final ShopifyUnityPlayerActivity rootActivity;
    @Nullable private AndroidPayCheckout checkout;

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

    @NonNull
    private AndroidPayCheckout getCheckout() {
        if (checkout == null) {
            final GoogleApiClientFactory factory = GoogleApiClientFactory.of(rootActivity, testing);
            checkout = new AndroidPayCheckout(factory, new MessageCenter());
        }
        return checkout;
    }

    public void canCheckoutWithAndroidPay(String cardBrandsString) {
        try {
            Logger.error("CARD BRANDS: " + cardBrandsString);
            final JSONArray array = new JSONArray(cardBrandsString);
            getCheckout().isReadyToPay(CardTypeConverter.convertCardBrandsToCardNetworks(array));
        } catch (JSONException e) {
            Logger.error("Unable to parse card brands: " + cardBrandsString);
            e.printStackTrace();
        }
    }

    public void checkoutWithAndroidPay(
            String merchantName,
            String publicKey,
            String pricingLineItemsString,
            String currencyCode,
            String countryCode,
            boolean requiresShipping
    ) {
        final String msg = "merchantName = " + merchantName + "\n" +
                "publicKey = " + publicKey + "\n" +
                "pricingLineItemsString = " + pricingLineItemsString + "\n" +
                "currencyCode = " + currencyCode + "\n" +
                "countryCode = " + countryCode + "\n" +
                "requiresShipping = " + requiresShipping + "\n";
        Logger.debug(msg);

        try {
            PayCart cart = cartFromUnity(merchantName, pricingLineItemsString, currencyCode,
                    countryCode, requiresShipping);

            rootActivity.startAndroidPayCheckout(getCheckout(), cart, publicKey);
        } catch (JSONException e) {
            Logger.error("Failed to parse summary items from Unity!");
        }
    }

    @VisibleForTesting
    PayCart cartFromUnity(
            String merchantName,
            String pricingLineItemsString,
            String currencyCode,
            String countryCode,
            boolean requiresShipping
    ) throws JSONException {
        PricingLineItems items = PricingLineItems.fromJsonString(pricingLineItemsString);

        return PayCart.builder()
                .merchantName(merchantName)
                .currencyCode(currencyCode)
                .countryCode(countryCode)
                .shippingAddressRequired(requiresShipping)
                .subtotal(items.subtotal)
                .shippingPrice(items.shippingPrice)
                .taxPrice(items.taxPrice)
                .totalPrice(items.totalPrice)
                .build();
    }
}
