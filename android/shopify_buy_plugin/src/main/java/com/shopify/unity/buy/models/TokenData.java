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

import com.shopify.unity.buy.utils.Logger;

import org.json.JSONException;
import org.json.JSONObject;

/**
 * Model class required by the Unity side that wraps
 * the payment confirmation token string.
 */

public final class TokenData implements JsonSerializable {

    /** JSON name for the <i>transaction identifier</i> attribute. */
    private static final String TRANSACTION_IDENTIFIER = "TransactionIdentifier";
    /** JSON name for the <i>payment data</i> attribute. */
    private static final String PAYMENT_DATA = "PaymentData";

    /** The payment confirmation token provided by Android Pay. */
    @NonNull public final String token;

    /**
     * Creates a new instance of this class.
     *
     * @param token the string token obtained as a confirmation of payment from Android Pay
     */
    public TokenData(@NonNull String token) {
        this.token = token;
    }

    @Override
    public JSONObject toJson() {
        final JSONObject json = new JSONObject();
        try {
            json.put(TRANSACTION_IDENTIFIER, token)
                .put(PAYMENT_DATA, token);
        } catch (JSONException e) {
            final String className = getClass().getSimpleName();
            Logger.error("Failed to convert " + className + " into a JSON String.");
            e.printStackTrace();
        }
        return json;
    }
}
