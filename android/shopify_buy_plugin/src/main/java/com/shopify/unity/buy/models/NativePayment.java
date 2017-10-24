package com.shopify.unity.buy.models;

import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import com.shopify.unity.buy.utils.Logger;

import org.json.JSONException;
import org.json.JSONObject;

/**
 * @author Flavio Faria
 */

public final class NativePayment implements JsonSerializable {

    private static final String BILLING_CONTACT = "BillingContact";
    private static final String SHIPPING_CONTACT = "ShippingContact";
    private static final String IDENTIFIER = "Identifier";
    private static final String TOKEN_DATA = "TokenData";

    @NonNull public final MailingAddressInput billingContact;
    @NonNull public final ShippingContact shippingContact;
    @Nullable public final String identifier;
    @NonNull public final TokenData tokenData;

    private NativePayment(Builder builder) {
        billingContact = builder.billingContact;
        shippingContact = builder.shippingContact;
        identifier = builder.identifier;
        tokenData = builder.tokenData;
    }

    public static Builder newBuilder() {
        return new Builder();
    }

    @Override
    public JSONObject toJson() {
        final JSONObject json = new JSONObject();
        try {
            json.put(BILLING_CONTACT, billingContact.toJson())
                .put(SHIPPING_CONTACT, shippingContact.toJson())
                .put(IDENTIFIER, identifier)
                .put(TOKEN_DATA, tokenData.toJson());
        } catch (JSONException e) {
            final String className = getClass().getSimpleName();
            Logger.error("Failed to convert " + className + " into a JSON String.");
            e.printStackTrace();
        }
        return json;
    }

    public static final class Builder {

        private MailingAddressInput billingContact;
        private ShippingContact shippingContact;
        private String identifier;
        private TokenData tokenData;

        private Builder() {
        }

        public Builder billingContact(MailingAddressInput val) {
            billingContact = val;
            return this;
        }

        public Builder shippingContact(ShippingContact val) {
            shippingContact = val;
            return this;
        }

        public Builder identifier(String val) {
            identifier = val;
            return this;
        }

        public Builder tokenData(TokenData val) {
            tokenData = val;
            return this;
        }

        public NativePayment build() {
            return new NativePayment(this);
        }
    }
}
