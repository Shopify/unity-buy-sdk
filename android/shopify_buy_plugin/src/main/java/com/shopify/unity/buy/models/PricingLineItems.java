package com.shopify.unity.buy.models;

import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import org.json.JSONException;
import org.json.JSONObject;

import java.math.BigDecimal;

public final class PricingLineItems implements JsonSerializable {
    private static final String SUBTOTAL = "subtotal";
    private static final String TAX_PRICE = "taxPrice";
    private static final String TOTAL_PRICE = "totalPrice";
    private static final String SHIPPING_PRICE = "shippingPrice";

    @NonNull public final BigDecimal subtotal;
    @NonNull public final BigDecimal taxPrice;
    @NonNull public final BigDecimal totalPrice;

    @Nullable public final BigDecimal shippingPrice;

    private PricingLineItems(@NonNull BigDecimal subtotal,
                             @NonNull BigDecimal taxPrice,
                             @NonNull BigDecimal totalPrice,
                             @Nullable BigDecimal shippingPrice) {
        this.totalPrice = totalPrice;
        this.subtotal = subtotal;
        this.taxPrice = taxPrice;
        this.shippingPrice = shippingPrice;
    }


    public static PricingLineItems fromJsonString(String jsonString) throws JSONException {
        return fromJson(new JSONObject(jsonString));
    }

    public static PricingLineItems fromJson(JSONObject json) throws JSONException {
        BigDecimal subtotal = decimalPropertyFromJson(json, SUBTOTAL);
        BigDecimal taxPrice = decimalPropertyFromJson(json, TAX_PRICE);
        BigDecimal totalPrice = decimalPropertyFromJson(json, TOTAL_PRICE);

        BigDecimal shippingPrice = null;
        if (json.has(SHIPPING_PRICE)) {
            shippingPrice = nullableDecimalPropertyFromJson(json, SHIPPING_PRICE);
        }

        return new PricingLineItems(subtotal, taxPrice, totalPrice, shippingPrice);
    }

    @Override
    public String toJsonString() throws JSONException {
        JSONObject json = new JSONObject();
        json.put(SUBTOTAL, subtotal.toString());
        json.put(TAX_PRICE, taxPrice.toString());
        json.put(TOTAL_PRICE, totalPrice.toString());

        if (shippingPrice != null) {
            json.put(SHIPPING_PRICE, shippingPrice.doubleValue());
        }

        return json.toString();
    }

    private static BigDecimal nullableDecimalPropertyFromJson(JSONObject obj, String property) throws JSONException {
        if (obj.has(property)) {
            return decimalPropertyFromJson(obj, property);
        }
        return null;
    }

    private static BigDecimal decimalPropertyFromJson(JSONObject obj, String property) throws JSONException {
        return new BigDecimal(obj.getString(property)).setScale(2, BigDecimal.ROUND_FLOOR);
    }
}
