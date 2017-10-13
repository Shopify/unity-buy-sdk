package com.shopify.unity.buy.models;

import android.util.Log;

import com.google.android.gms.identity.intents.model.UserAddress;
import com.shopify.buy3.pay.PayAddress;

import org.json.JSONException;
import org.json.JSONObject;

// A wrapper around PayAddress to convert it to Json.
public class MailingAddressInput implements JsonSerializable {
    private final PayAddress payAddress;

    private static final String ADDRESS_1 = "address1";
    private static final String ADDRESS_2 = "address2";
    private static final String CITY = "city";
    private static final String COUNTRY = "country";
    private static final String FIRST_NAME = "firstName";
    private static final String LAST_NAME = "lastName";
    private static final String PHONE = "phone";
    private static final String PROVINCE = "province";
    private static final String ZIP = "zip";

    public MailingAddressInput(UserAddress userAddress) {
        payAddress = PayAddress.fromUserAddress(userAddress);
    }

    public String toJsonString() {
        JSONObject obj = new JSONObject();
        try {
            obj.put(ADDRESS_1, payAddress.address1);
            obj.put(ADDRESS_2, payAddress.address2);
            obj.put(CITY, payAddress.city);
            obj.put(COUNTRY, payAddress.country);
            obj.put(FIRST_NAME, payAddress.firstName);
            obj.put(LAST_NAME, payAddress.lastName);
            obj.put(PHONE, payAddress.phone);
            obj.put(PROVINCE, payAddress.province);
            obj.put(ZIP, payAddress.zip);
        } catch (JSONException e) {
            Log.e("ShopifyBuyPlugin", "Failed to convert MailingAddressInput into a JSON String.");
            e.printStackTrace();
        }
        return obj.toString();
    }
}
