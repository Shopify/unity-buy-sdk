package com.shopify.unity.buy.models;

import android.util.Log;

import com.google.android.gms.identity.intents.model.UserAddress;
import com.shopify.buy3.pay.PayAddress;

import org.json.JSONException;
import org.json.JSONObject;

// A wrapper around PayAddress to convert it to Json.
public class MailingAddressInput implements JsonSerializable {
    private static final String ADDRESS_1 = "address1";
    private static final String ADDRESS_2 = "address2";
    private static final String CITY = "city";
    private static final String COUNTRY = "country";
    private static final String FIRST_NAME = "firstName";
    private static final String LAST_NAME = "lastName";
    private static final String PHONE = "phone";
    private static final String PROVINCE = "province";
    private static final String ZIP = "zip";

    public final String address1;
    public final String address2;
    public final String city;
    public final String country;
    public final String firstName;
    public final String lastName;
    public final String phone;
    public final String province;
    public final String zip;

    public MailingAddressInput(UserAddress userAddress) {
        PayAddress payAddress = PayAddress.fromUserAddress(userAddress);
        this.address1 = payAddress.address1;
        this.address2 = payAddress.address2;
        this.city = payAddress.city;
        this.country = payAddress.country;
        this.firstName = payAddress.firstName;
        this.lastName = payAddress.lastName;
        this.phone = payAddress.phone;
        this.province = payAddress.province;
        this.zip = payAddress.zip;
    }

    static MailingAddressInput fromJson(JSONObject json) throws JSONException {
        String address1 = json.getString("address1");
        String address2 = json.getString("address2");
        String city = json.getString("city");
        String country = json.getString("country");
        String firstName = json.getString("firstName");
        String lastName = json.getString("lastName");
        String phone = json.getString("phone");
        String province = json.getString("province");
        String zip = json.getString("zip");

        return new MailingAddressInput(address1, address2, city, country, firstName, lastName, phone, province, zip);
    }

    //CHECKSTYLE:OFF
    protected MailingAddressInput(String address1, String address2, String city,
                                String country, String firstName, String lastName,
                                String phone, String province, String zip) {
        //CHECKSTLYE:ON
        this.address1 = address1;
        this.address2 = address2;
        this.city = city;
        this.country = country;
        this.firstName = firstName;
        this.lastName = lastName;
        this.phone = phone;
        this.province = province;
        this.zip = zip;
    }

    public JSONObject toJson() {
        JSONObject json = new JSONObject();
        try {
            json.put(ADDRESS_1, address1)
                .put(ADDRESS_2, address2)
                .put(CITY, city)
                .put(COUNTRY, country)
                .put(FIRST_NAME, firstName)
                .put(LAST_NAME, lastName)
                .put(PHONE, phone)
                .put(PROVINCE, province)
                .put(ZIP, zip);
        } catch (JSONException e) {
            Log.e("ShopifyBuyPlugin", "Failed to convert MailingAddressInput into a JSON String.");
            e.printStackTrace();
        }
        return json;
    }
}
