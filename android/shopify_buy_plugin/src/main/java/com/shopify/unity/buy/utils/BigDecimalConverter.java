package com.shopify.unity.buy.utils;

import android.support.annotation.NonNull;

import org.json.JSONException;
import org.json.JSONObject;

import java.math.BigDecimal;

/**
 * Utility class that provides methods for handling JSON-encoded {@link BigDecimal} properties.
 */

public final class BigDecimalConverter {

    private BigDecimalConverter() {

    }

    /**
     * Reads a {@link BigDecimal} representation from a {@link JSONObject}.
     *
     * @param obj the {@code JSONObject} to extract the decimal from
     * @param property the JSON property name to read the decimal from
     * @return a {@code BigDecimal} representation of the JSON-encoded decimal number
     * @throws JSONException if the {@code property} is absent or not encoded as {@link String}
     */
    public static BigDecimal decimalPropertyFromJson(@NonNull JSONObject obj,
                                                     @NonNull String property)
            throws JSONException {
        return new BigDecimal(obj.getString(property)).setScale(2, BigDecimal.ROUND_FLOOR);
    }


    /**
     * Reads nullable a {@link BigDecimal} representation from a {@link JSONObject}.
     *
     * @param obj the {@code JSONObject} to extract the decimal from
     * @param property the JSON property name to read the decimal from
     * @return a {@code BigDecimal} representation of the JSON-encoded decimal number or
     * {@code null} if such representatoin is absent from the JSON object
     * @throws JSONException if the {@code property} is not encoded as {@link String}
     */
    public static BigDecimal nullableDecimalPropertyFromJson(@NonNull JSONObject obj,
                                                             @NonNull String property)
            throws JSONException {
        if (obj.has(property)) {
            return decimalPropertyFromJson(obj, property);
        }
        return null;
    }
}
