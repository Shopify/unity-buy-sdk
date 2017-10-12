package com.shopify.unity.buy.models;

import android.util.Log;

import com.google.android.gms.identity.intents.model.UserAddress;
import com.shopify.buy3.pay.PayAddress;
import com.shopify.unity.buy.JsonSerializable;

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

    public static MailingAddressInput fromJsonString(String json) throws JSONException {
        String expected = "{" +
            "\"address1\":\"address1\"," +
            "\"address2\":\"address2, address3, address4, address5\"," +
            "\"city\":\"locality\"," +
            "\"country\":\"countryCode\"," +
            "\"firstName\":\"firstName\"," +
            "\"lastName\":\"lastName\"," +
            "\"phone\":\"phoneNumber\"," +
            "\"province\":\"administrativeArea\"," +
            "\"zip\":\"postalCode\"" +
            "}";

        JSONObject obj = new JSONObject(json);
        String address1 = obj.getString("address1");
        String address2 = obj.getString("address2");
        String city = obj.getString("city");
        String country = obj.getString("country");
        String firstName = obj.getString("firstName");
        String lastName = obj.getString("lastName");
        String phone = obj.getString("phone");
        String province = obj.getString("province");
        String zip = obj.getString("zip");

        return new MailingAddressInput(address1, address2, city, country, firstName, lastName, phone, province, zip);
    }


    //CHECKSTYLE:OFF
    private MailingAddressInput(String address1, String address2, String city,
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

    public String toJsonString() {
        JSONObject obj = new JSONObject();
        try {
            obj.put(ADDRESS_1, address1);
            obj.put(ADDRESS_2, address2);
            obj.put(CITY, city);
            obj.put(COUNTRY, country);
            obj.put(FIRST_NAME, firstName);
            obj.put(LAST_NAME, lastName);
            obj.put(PHONE, phone);
            obj.put(PROVINCE, province);
            obj.put(ZIP, zip);
        } catch (JSONException e) {
            Log.e("ShopifyBuyPlugin", "Failed to convert MailingAddressInput into a JSON String.");
            e.printStackTrace();
        }
        return obj.toString();
    }
}
