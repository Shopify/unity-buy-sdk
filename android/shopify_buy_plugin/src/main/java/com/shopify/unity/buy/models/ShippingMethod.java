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

import android.os.Parcel;
import android.os.Parcelable;

import com.shopify.unity.buy.utils.BigDecimalConverter;
import com.shopify.unity.buy.utils.Logger;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Objects;

/**
 * Model class that represents a shipping method in the checkout process.
 */

public final class ShippingMethod implements JsonSerializable, Parcelable {

    /** JSON name for the <i>identifier</i> attribute. */
    public static final String IDENTIFIER = "Identifier";
    /** JSON name for the <i>detail</i> attribute. */
    public static final String DETAIL = "Detail";
    /** JSON name for the <i>label</i> attribute. */
    public static final String LABEL = "Label";
    /** JSON name for the <i>amount</i> attribute. */
    public static final String AMOUNT = "Amount";

    /** {@link Creator} static member required by the {@link Parcelable} interface. */
    public static final Creator<ShippingMethod> CREATOR = new Creator<ShippingMethod>() {

        @Override
        public ShippingMethod createFromParcel(Parcel parcel) {
            return new ShippingMethod(parcel);
        }

        @Override
        public ShippingMethod[] newArray(int size) {
            return new ShippingMethod[size];
        }
    };

    /** Internal field. */
    public final String identifier;
    /** Internal field. */
    public final String detail;
    /** The user-facing label that describes this shipping method. */
    public final String label;
    /** How much the user will pay if they choose this shipping method. */
    public final BigDecimal amount;

    public ShippingMethod(String identifier, String detail, String label, BigDecimal amount) {
        this.identifier = identifier;
        this.detail = detail;
        this.label = label;
        this.amount = amount.setScale(2, BigDecimal.ROUND_HALF_EVEN);
    }

    private ShippingMethod(Parcel in) {
        identifier = in.readString();
        detail = in.readString();
        label = in.readString();
        amount = new BigDecimal(in.readString());
    }

    /**
     * Parses a JSON string into a {@code ShippingMethod} object.
     *
     * @param jsonString a {@code ShippingMethod} represented as a string
     * @return a {@code ShippingMethod} representation of {@code jsonString}
     * @throws JSONException if {@code jsonString} does not match the object definition
     * of {@code ShippingMethod}
     */
    public static ShippingMethod fromJsonString(String jsonString) throws JSONException {
        return fromJson(new JSONObject(jsonString));
    }

    /**
     * Parses a JSON object into a {@code ShippingMethod} object.
     *
     * @param json a {@code ShippingMethod} represented as a string
     * @return a {@code ShippingMethod} representation of {@code jsonString}
     * @throws JSONException if {@code json} does not match the object definition
     * of {@code ShippingMethod}
     */
    public static ShippingMethod fromJson(JSONObject json) throws JSONException {
        final String identifier = json.getString(IDENTIFIER);
        final String detail = json.getString(DETAIL);
        final String label = json.getString(LABEL);
        final BigDecimal amount = BigDecimalConverter.decimalPropertyFromJson(json, AMOUNT);
        return new ShippingMethod(identifier, detail, label, amount);
    }

    /**
     * Parses a JSON string into a {@code List<ShippingMethod>} object.
     *
     * @param jsonString a {@code List<ShippingMethod>} represented as a string
     * @return a {@code List<ShippingMethod>} representation of {@code jsonString}
     * @throws JSONException if {@code jsonString} does not match the object definition
     * of {@code List<ShippingMethod>}
     */
    public static List<ShippingMethod> listFromJsonString(String jsonString) throws JSONException {
        final JSONArray array = new JSONArray(jsonString);
        final int len = array.length();
        final List<ShippingMethod> list = new ArrayList<>(len);
        for (int i = 0; i < len; i++) {
            list.add(fromJson(array.getJSONObject(i)));
        }
        return Collections.unmodifiableList(list);
    }

    @Override
    public JSONObject toJson() {
        final JSONObject json = new JSONObject();
        try {
            json.put(IDENTIFIER, identifier)
                .put(DETAIL, detail)
                .put(LABEL, label)
                .put(AMOUNT, amount);
        } catch (JSONException e) {
            final String className = getClass().getSimpleName();
            Logger.error("Failed to convert " + className + " into a JSON String.");
            e.printStackTrace();
        }
        return json;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel parcel, int flags) {
        parcel.writeString(identifier);
        parcel.writeString(detail);
        parcel.writeString(label);
        parcel.writeString(amount.toString());
    }

    @Override
    public int hashCode() {
        return Objects.hash(identifier, detail, label, amount);
    }

    @Override
    public boolean equals(Object obj) {
        if (!(obj instanceof ShippingMethod)) {
            return false;
        }
        ShippingMethod other = (ShippingMethod) obj;
        return Objects.equals(this.identifier, other.identifier)
                && Objects.equals(this.detail, other.detail)
                && Objects.equals(this.label, other.label)
                && Objects.equals(this.amount, other.amount);
    }
}
