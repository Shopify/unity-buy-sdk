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
     * {@code null} if such representation is absent from the JSON object
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
