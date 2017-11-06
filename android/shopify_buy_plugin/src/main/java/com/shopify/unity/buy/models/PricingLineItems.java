/**
 * The MIT License (MIT)
 * Copyright (c) 2017 Shopify
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */
package com.shopify.unity.buy.models;

import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import com.shopify.unity.buy.utils.BigDecimalConverter;
import com.shopify.unity.buy.utils.Logger;

import org.json.JSONException;
import org.json.JSONObject;

import java.math.BigDecimal;
import java.math.RoundingMode;

/**
 * Model class that holds the lines that compose the total price of the purchase.
 */
public final class PricingLineItems implements JsonSerializable {

    /** JSON name for the <i>subtotal</i> attribute. */
    private static final String SUBTOTAL = "subtotal";
    /** JSON name for the <i>tax price</i> attribute. */
    private static final String TAX_PRICE = "taxPrice";
    /** JSON name for the <i>total price</i> attribute. */
    private static final String TOTAL_PRICE = "totalPrice";
    /** JSON name for the <i>shippign price</i> attribute. */
    private static final String SHIPPING_PRICE = "shippingPrice";

    /** The subtotal value of the purchase. */
    @NonNull public final BigDecimal subtotal;
    /** The tax pricevalue of the purchase. */
    @NonNull public final BigDecimal taxPrice;
    /** The total price value of the purchase. */
    @NonNull public final BigDecimal totalPrice;
    /** The shipping price value of the purchase. */
    @Nullable public final BigDecimal shippingPrice;

    /**
     * Creates a new instance of this class.
     *
     * @param subtotal the subtotal value of the purchase
     * @param taxPrice the tax price of the purchase
     * @param totalPrice the total price of the purchase
     * @param shippingPrice the shipping price of the purchase
     */
    public PricingLineItems(@NonNull BigDecimal subtotal,
                            @NonNull BigDecimal taxPrice,
                            @NonNull BigDecimal totalPrice,
                            @Nullable BigDecimal shippingPrice) {
        this.totalPrice = totalPrice.setScale(2, RoundingMode.HALF_EVEN);
        this.subtotal = subtotal.setScale(2, RoundingMode.HALF_EVEN);
        this.taxPrice = taxPrice.setScale(2, RoundingMode.HALF_EVEN);
        if (shippingPrice != null) {
            this.shippingPrice = shippingPrice.setScale(2, RoundingMode.HALF_EVEN);
        } else {
            this.shippingPrice = null;
        }
    }


    /**
     * Parses a JSON string into a {@code PricingLineItems} object.
     *
     * @param jsonString a {@code PricingLineItems} represented as a string
     * @return an {@code PricingLineItems} representation of {@code jsonString}
     * @throws JSONException if {@code jsonString} does not match the object definition
     * of {@code PricingLineItems}
     */
    public static PricingLineItems fromJsonString(String jsonString) throws JSONException {
        return fromJson(new JSONObject(jsonString));
    }

    /**
     * Parses a JSON object into a {@code PricingLineItems} object.
     *
     * @param json a {@code PricingLineItems} represented as a string
     * @return a {@code PricingLineItems} representation of {@code jsonString}
     * @throws JSONException if {@code json} does not match the object definition
     * of {@code PricingLineItems}
     */
    public static PricingLineItems fromJson(JSONObject json) throws JSONException {
        final BigDecimal subtotal = BigDecimalConverter.decimalPropertyFromJson(json, SUBTOTAL);
        final BigDecimal taxPrice = BigDecimalConverter.decimalPropertyFromJson(json, TAX_PRICE);
        final BigDecimal totalPrice = BigDecimalConverter.decimalPropertyFromJson(json, TOTAL_PRICE);

        BigDecimal shippingPrice = null;
        if (json.has(SHIPPING_PRICE)) {
            shippingPrice = BigDecimalConverter.nullableDecimalPropertyFromJson(json, SHIPPING_PRICE);
        }

        return new PricingLineItems(subtotal, taxPrice, totalPrice, shippingPrice);
    }

    @Override
    public JSONObject toJson() {
        JSONObject json = new JSONObject();
        try {
            json.put(SUBTOTAL, subtotal.toString())
                .put(TAX_PRICE, taxPrice.toString())
                .put(TOTAL_PRICE, totalPrice.toString());
            if (shippingPrice != null) {
                json.put(SHIPPING_PRICE, shippingPrice.doubleValue());
            }
        } catch (JSONException e) {
            final String className = getClass().getSimpleName();
            Logger.error("Failed to convert " + className + " into a JSON String.");
            e.printStackTrace();
        }
        return json;
    }
}
