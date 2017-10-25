package com.shopify.unity.buy.models;

import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import com.shopify.unity.buy.utils.BigDecimalConverter;
import com.shopify.unity.buy.utils.Logger;

import org.json.JSONException;
import org.json.JSONObject;

import java.math.BigDecimal;
import java.math.RoundingMode;

public final class PricingLineItems implements JsonSerializable {
    private static final String SUBTOTAL = "subtotal";
    private static final String TAX_PRICE = "taxPrice";
    private static final String TOTAL_PRICE = "totalPrice";
    private static final String SHIPPING_PRICE = "shippingPrice";

    @NonNull public final BigDecimal subtotal;
    @NonNull public final BigDecimal taxPrice;
    @NonNull public final BigDecimal totalPrice;

    @Nullable public final BigDecimal shippingPrice;

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


    public static PricingLineItems fromJsonString(String jsonString) throws JSONException {
        return fromJson(new JSONObject(jsonString));
    }

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
