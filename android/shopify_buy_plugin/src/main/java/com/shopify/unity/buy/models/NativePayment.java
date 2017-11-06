package com.shopify.unity.buy.models;

import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import com.shopify.unity.buy.utils.Logger;

import org.json.JSONException;
import org.json.JSONObject;

/**
 * Model class that wraps all information needed to complete a payment.
 */

public final class NativePayment implements JsonSerializable {

    /** JSON name for the <i>billing contact</i> attribute. */
    private static final String BILLING_CONTACT = "BillingContact";
    /** JSON name for the <i>shipping contact</i> attribute. */
    private static final String SHIPPING_CONTACT = "ShippingContact";
    /** JSON name for the <i>identifier</i> attribute. */
    private static final String IDENTIFIER = "Identifier";
    /** JSON name for the <i>token data</i> attribute. */
    private static final String TOKEN_DATA = "TokenData";

    /** The billing contact. */
    @NonNull public final MailingAddressInput billingContact;
    /** The shipping contact. */
    @Nullable public final ShippingContact shippingContact;
    /** The identifier needed to complete Android Pay purchases. */
    @NonNull public final String identifier;
    /** The token information needed to process the payment. */
    @NonNull public final TokenData tokenData;

    /**
     * Creates a new instance of this class.
     *
     * @param builder that holds all fields as a single input argument
     */
    private NativePayment(Builder builder) {
        if (builder.billingContact == null) {
            throw new IllegalArgumentException("Missing billingContact field");
        }
        if (builder.identifier == null) {
            throw new IllegalArgumentException("Missing identifier field");
        }
        if (builder.tokenData == null) {
            throw new IllegalArgumentException("Missing tokenData field");
        }
        billingContact = builder.billingContact;
        shippingContact = builder.shippingContact;
        identifier = builder.identifier;
        tokenData = builder.tokenData;
    }

    /**
     * Creates a new builder that will allow the construction of a new {@link NativePayment} object.
     *
     * @return a new {@code NativePayment} builder
     */
    public static Builder newBuilder() {
        return new Builder();
    }

    @Override
    public JSONObject toJson() {
        final JSONObject json = new JSONObject();
        try {
            json.put(BILLING_CONTACT, billingContact.toJson());
            if (shippingContact != null) {
                json.put(SHIPPING_CONTACT, shippingContact.toJson());
            }
            json.put(IDENTIFIER, identifier)
                .put(TOKEN_DATA, tokenData.toJson());
        } catch (JSONException e) {
            final String className = getClass().getSimpleName();
            Logger.error("Failed to convert " + className + " into a JSON String.");
            e.printStackTrace();
        }
        return json;
    }

    /**
     * Builder used to easily create {@link NativePayment} objects by chaining
     * setter calls.
     */
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
