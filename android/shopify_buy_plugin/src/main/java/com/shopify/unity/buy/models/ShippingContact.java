package com.shopify.unity.buy.models;

import com.google.android.gms.identity.intents.model.UserAddress;
import com.shopify.unity.buy.utils.Logger;

import org.json.JSONException;
import org.json.JSONObject;

public class ShippingContact extends MailingAddressInput {

    private static final String EMAIL = "email";

    public final String email;

    public ShippingContact(MailingAddressInput mailingAddressInput, String email) {
        super(mailingAddressInput.address1, mailingAddressInput.address2, mailingAddressInput.city,
                mailingAddressInput.country, mailingAddressInput.firstName,
                mailingAddressInput.lastName, mailingAddressInput.phone,
                mailingAddressInput.province, mailingAddressInput.zip);
        this.email = email;
    }

    public ShippingContact(UserAddress userAddress, String email) {
        super(userAddress);
        this.email = email;
    }

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
