package com.shopify.unity.buy.models;

import com.shopify.buy3.pay.PayCart;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.List;

/**
 * Model class that wraps the all the data sent from Unity to the Android library
 * after the shipping address is changed. The data held by this class is used
 * to recreate a {@link PayCart} before the full wallet is requested.
 */

public final class AndroidPayEventResponse {

    /** JSON name for the <i>merchant name</i> attribute. */
    private static final String MERCHANT_NAME = "merchantName";
    /** JSON name for the <i>pricing line items</i> attribute. */
    private static final String PRICING_LINE_ITEMS = "pricingLineItems";
    /** JSON name for the <i>currency code</i> attribute. */
    private static final String CURRENCY_CODE = "currencyCode";
    /** JSON name for the <i>country code</i> attribute. */
    private static final String COUNTRY_CODE = "countryCode";
    /** JSON name for the <i>requires shipping</i> attribute. */
    private static final String REQUIRES_SHIPPING = "requiresShipping";
    /** JSON name for the <i>shipping methods</i> attribute. */
    private static final String SHIPPING_METHODS = "shippingMethods";

    /** The name of the merchant. */
    public final String merchantName;
    /** Aggregate of all purchases, taxes and shipping costs of the cart. */
    public final PricingLineItems pricingLineItems;
    /** The currency code of the cart. */
    public final String currencyCode;
    /** The country code of the cart. */
    public final String countryCode;
    /** Whether if the purchased items require shipping or not. */
    public final boolean requiresShipping;
    /** A list of available shipping methods. */
    public final List<ShippingMethod> shippingMethods;

    private AndroidPayEventResponse(String merchantName, PricingLineItems pricingLineItems,
                                    String currencyCode, String countryCode,
                                    boolean requiresShipping,
                                    List<ShippingMethod> shippingMethods) {

        this.merchantName = merchantName;
        this.pricingLineItems = pricingLineItems;
        this.currencyCode = currencyCode;
        this.countryCode = countryCode;
        this.requiresShipping = requiresShipping;
        this.shippingMethods = shippingMethods;
    }

    /**
     * Parses a JSON string into an {@code AndroidPayEventResponse} object.
     *
     * @param jsonString an {@code AndroidPayEventResponse} represented as a string
     * @return an {@code AndroidPayEventResponse} representation of {@code jsonString}
     * @throws JSONException if {@code jsonString} does not match the object definition
     * of {@code AndroidPayEventResponse}
     */
    public static AndroidPayEventResponse fromJsonString(String jsonString) throws JSONException {
        final JSONObject jsonObject = new JSONObject(jsonString);
        final String merchantName = jsonObject.getString(MERCHANT_NAME);
        final JSONObject pricingLineItemsJson = jsonObject.getJSONObject(PRICING_LINE_ITEMS);

        final PricingLineItems pricingLineItems =
                PricingLineItems.fromJson(pricingLineItemsJson);

        final String currencyCode = jsonObject.getString(CURRENCY_CODE);
        final String countryCode = jsonObject.getString(COUNTRY_CODE);
        final boolean requiresShipping = jsonObject.getBoolean(REQUIRES_SHIPPING);

        final JSONArray shippingMethodsJson = jsonObject.getJSONArray(SHIPPING_METHODS);
        final List<ShippingMethod> shippingMethods =
                ShippingMethod.listFromJsonString(shippingMethodsJson.toString());

        return new AndroidPayEventResponse(merchantName, pricingLineItems,
                currencyCode, countryCode, requiresShipping, shippingMethods);
    }
}

