package com.shopify.unity.buy.models;

import android.support.annotation.NonNull;

import com.shopify.unity.buy.utils.Logger;

import org.json.JSONException;
import org.json.JSONObject;

/**
 * @author Flavio Faria
 */

public final class TokenData implements JsonSerializable {

    private static final String TRANSACTION_IDENTIFIER = "TransactionIdentifier";
    private static final String PAYMENT_DATA = "PaymentData";

    @NonNull public final String token;

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
