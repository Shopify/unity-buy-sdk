package com.shopify.unity.buy;

import com.shopify.buy3.pay.PayCart;
import com.shopify.unity.buy.utils.AndroidLogger;
import com.shopify.unity.buy.utils.ILogger;
import com.unity3d.player.UnityPlayer;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.math.BigDecimal;

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
            String summaryItemsString,
            String currencyCode,
            String countryCode,
            boolean requiresShipping
    ) {
        try {
            PayCart cart = cartFromUnity(merchantName, summaryItemsString, currencyCode, countryCode,
                    requiresShipping);
            addAndroidPayFragment(unityDelegateObjectName, cart, countryCode);
            return true;
        } catch (JSONException e) {
            logger.error("ShopifyBuyPlugin", "Failed to parse summary items from Unity");
            e.printStackTrace();
            return false;
        } catch (IllegalArgumentException e) {
            logger.error("ShopifyBuyPlugin", e.getMessage());
            e.printStackTrace();
            return false;
        }
    }

    public PayCart cartFromUnity(
        String merchantName,
        String summaryItemsString,
        String currencyCode,

        // TODO: Pull in updated buy3 SDK which will use the countryCode.
        String countryCode,
        boolean requiresShipping
    ) throws JSONException {
        SummaryItems items = SummaryItems.fromJsonString(summaryItemsString);

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

    // Wrapper around SummaryItems from Unity that performs validation.
    private static class SummaryItems {
        private BigDecimal subtotal;
        private BigDecimal shippingPrice;
        private BigDecimal taxPrice;
        private BigDecimal totalPrice;

        private static final String LABEL = "LABEL";

        private static final String SUBTOTAL = "SUBTOTAL";
        private static final String SHIPPING = "SHIPPING";
        private static final String TAXES = "TAXES";
        private static final String TOTAL = "TOTAL";

        static SummaryItems fromJsonString(String jsonString) throws JSONException {
            SummaryItems items = new SummaryItems();
            JSONArray jsonArray = new JSONArray(jsonString);
            for (int i = 0; i < jsonArray.length(); i++) {
                JSONObject summaryItemObj = new JSONObject(jsonArray.getString(i));
                String label = summaryItemObj.getString(LABEL);

                if (label.equals(SUBTOTAL)) {
                    items.subtotal = amountFromJson(summaryItemObj);
                } else if (label.equals(SHIPPING)) {
                    items.shippingPrice = amountFromJson(summaryItemObj);
                } else if (label.equals(TAXES)) {
                    items.taxPrice = amountFromJson(summaryItemObj);
                } else if (label.equals(TOTAL)) {
                    items.totalPrice = amountFromJson(summaryItemObj);
                }
            }

            // Verify that we've set all of the required parameters that we need.
            checkNotNull(items.subtotal, "subtotal");
            checkNotNull(items.taxPrice, "taxPrice");
            checkNotNull(items.totalPrice, "totalPrice");

            return items;
        }

        private static BigDecimal amountFromJson(JSONObject obj) throws JSONException {
            return new BigDecimal(obj.getString("Amount"));
        }

        private static void checkNotNull(BigDecimal value, String missingField)
                throws JSONException {
            if (value == null) {
                String msg = String.format("Missing '%s' field from summary items.", missingField);
                throw new JSONException(msg);
            }
        }
    }
}
