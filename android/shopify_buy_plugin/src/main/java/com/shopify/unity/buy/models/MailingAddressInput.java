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

    static MailingAddressInput fromJson(JSONObject json) throws JSONException {
        return newBuilder()
                .address1(json.getString("address1"))
                .address2(json.getString("address2"))
                .city(json.getString("city"))
                .country(json.getString("country"))
                .firstName(json.getString("firstName"))
                .lastName(json.getString("lastName"))
                .phone(json.getString("phone"))
                .province(json.getString("province"))
                .zip(json.getString("zip"))
                .build();
    }

    /**
     * Creates a new instance of this class.
     *
     * @param builder that holds all fields as a single input argument
     */
    MailingAddressInput(Builder builder) {
        address1 = builder.address1;
        address2 = builder.address2;
        city = builder.city;
        country = builder.country;
        firstName = builder.firstName;
        lastName = builder.lastName;
        phone = builder.phone;
        province = builder.province;
        zip = builder.zip;
    }

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

    /**
     * Creates a new builder that will allow the construction of a new
     * {@link MailingAddressInput} object.
     *
     * @return a new {@code MailingAddressInput} builder
     */
    public static Builder newBuilder() {
        return new Builder();
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

    /**
     * Builder used to easily create {@link MailingAddressInput} objects by chaining
     * setter calls.
     */
    public static class Builder {

        private String address1;
        private String address2;
        private String city;
        private String country;
        private String firstName;
        private String lastName;
        private String phone;
        private String province;
        private String zip;

        Builder() {
        }

        public Builder address1(String val) {
            address1 = val;
            return this;
        }

        public Builder address2(String val) {
            address2 = val;
            return this;
        }

        public Builder city(String val) {
            city = val;
            return this;
        }

        public Builder country(String val) {
            country = val;
            return this;
        }

        public Builder firstName(String val) {
            firstName = val;
            return this;
        }

        public Builder lastName(String val) {
            lastName = val;
            return this;
        }

        public Builder phone(String val) {
            phone = val;
            return this;
        }

        public Builder province(String val) {
            province = val;
            return this;
        }

        public Builder zip(String val) {
            zip = val;
            return this;
        }

        public MailingAddressInput build() {
            return new MailingAddressInput(this);
        }
    }
}
