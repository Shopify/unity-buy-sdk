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
