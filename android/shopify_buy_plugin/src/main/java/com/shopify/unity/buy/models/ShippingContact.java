package com.shopify.unity.buy.models;

import com.google.android.gms.identity.intents.model.UserAddress;
import com.shopify.unity.buy.utils.Logger;

import org.json.JSONException;
import org.json.JSONObject;

/**
 * Model class that adds an extra email field to the {@link MailingAddressInput} class.
 */
public class ShippingContact extends MailingAddressInput {

    /** JSON name for the <i>email</i> attribute. */
    private static final String EMAIL = "email";
    /** The email address of the shipping contact. */
    public final String email;

    /**
     * Creates a new instance of this class on top of an existing {@link MailingAddressInput}
     *
     * @param mailingAddressInput to copy fields from
     * @param email the email address fo the shipping contact
     */
    public ShippingContact(MailingAddressInput mailingAddressInput, String email) {
        super(mailingAddressInput.address1, mailingAddressInput.address2, mailingAddressInput.city,
                mailingAddressInput.country, mailingAddressInput.firstName,
                mailingAddressInput.lastName, mailingAddressInput.phone,
                mailingAddressInput.province, mailingAddressInput.zip);
        this.email = email;
    }

    /**
     * Creates a new instance of this class based on a {@link UserAddress}.
     *
     * @param userAddress the {@code UserAddress} to copy fields from
     * @param email the email address fo the shipping contact
     */
    public ShippingContact(UserAddress userAddress, String email) {
        super(userAddress);
        this.email = email;
    }
    
    /**
     * Parses a JSON object into a {@code ShippingContact} object.
     *
     * @param json a {@code ShippingContact} represented as a string
     * @return a {@code ShippingContact} representation of {@code jsonString}
     * @throws JSONException if {@code json} does not match the object definition
     * of {@code ShippingContact}
     */
    static ShippingContact fromJson(JSONObject json) throws JSONException {
        final MailingAddressInput input = MailingAddressInput.fromJson(json);
        return new ShippingContact(input, json.getString(EMAIL));
    }

    public JSONObject toJson() {
        JSONObject json = super.toJson();
        try {
            json.put(EMAIL, email);
        } catch (JSONException e) {
            final String className = getClass().getSimpleName();
            Logger.error("Failed to convert " + className + " into a JSON String.");
            e.printStackTrace();
        }
        return json;
    }
}
