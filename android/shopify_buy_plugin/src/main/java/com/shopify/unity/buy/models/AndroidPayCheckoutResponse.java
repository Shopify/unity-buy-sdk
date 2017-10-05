package com.shopify.unity.buy.models;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;

/**
 * @author Flavio Faria
 */

public final class AndroidPayCheckoutResponse {

    private static final String MERCHANT_NAME = "merchantName";
    private static final String PRICING_LINE_ITEMS = "pricingLineItems";
    private static final String CURRENCY_CODE = "currencyCode";
    private static final String COUNTRY_CODE = "countryCode";
    private static final String REQUIRES_SHIPPING = "requiresShipping";

    public final String merchantName;
    public final PricingLineItems pricingLineItems;
    public final String currencyCode;
    public final String countryCode;
    public final boolean requiresShipping;

    public AndroidPayCheckoutResponse(String merchantName, PricingLineItems pricingLineItems,
                                      String currencyCode, String countryCode,
                                      boolean requiresShipping) {

        this.merchantName = merchantName;
        this.pricingLineItems = pricingLineItems;
        this.currencyCode = currencyCode;
        this.countryCode = countryCode;
        this.requiresShipping = requiresShipping;
    }

    public static AndroidPayCheckoutResponse fromJsonString(String jsonString)
            throws JSONException, IOException {

        final JSONObject jsonObject = new JSONObject(jsonString);
        final String merchantName = jsonObject.getString(MERCHANT_NAME);
        final String pricingLineItemsString = jsonObject.getString(PRICING_LINE_ITEMS);

        final PricingLineItems pricingLineItems =
                PricingLineItems.fromJsonString(pricingLineItemsString);

        final String currencyCode = jsonObject.getString(CURRENCY_CODE);
        final String countryCode = jsonObject.getString(COUNTRY_CODE);
        final boolean requiresShipping = jsonObject.getBoolean(REQUIRES_SHIPPING);

        return new AndroidPayCheckoutResponse(merchantName, pricingLineItems,
                currencyCode, countryCode, requiresShipping);
    }
}
