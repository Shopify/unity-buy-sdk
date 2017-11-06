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
import android.support.test.filters.SmallTest;
import android.support.test.runner.AndroidJUnit4;

import org.json.JSONObject;
import org.junit.Test;
import org.junit.runner.RunWith;

import java.math.BigDecimal;
import java.util.List;

import static org.junit.Assert.assertEquals;

/**
 * @author Flavio Faria
 */
@RunWith(AndroidJUnit4.class)
@SmallTest
public class ShippingMethodTest {

    private static final String JSON_STR = "{" +
            "\"Identifier\": \"Identifier\", " +
            "\"Detail\": \"Detail\", " +
            "\"Label\": \"Label\", " +
            "\"Amount\": \"3.95\"" +
            "}";

    @Test
    public void fromJsonString() throws Exception {
        ShippingMethod shippingMethod = ShippingMethod.fromJsonString(JSON_STR);
        assertEquals("Identifier", shippingMethod.identifier);
        assertEquals("Detail", shippingMethod.detail);
        assertEquals("Label", shippingMethod.label);
        assertEquals(BigDecimal.valueOf(3.95), shippingMethod.amount);
    }

    @Test
    public void fromJson() throws Exception {
        ShippingMethod shippingMethod = ShippingMethod.fromJson(new JSONObject(JSON_STR));
        assertEquals("Identifier", shippingMethod.identifier);
        assertEquals("Detail", shippingMethod.detail);
        assertEquals("Label", shippingMethod.label);
        assertEquals(BigDecimal.valueOf(3.95), shippingMethod.amount);
    }

    @Test
    public void listFromJsonString() throws Exception {
        String jsonArrayStr = "[" + JSON_STR + "]";
        List<ShippingMethod> shippingMethods = ShippingMethod.listFromJsonString(jsonArrayStr);
        assertEquals(1, shippingMethods.size());
        assertEquals("Identifier", shippingMethods.get(0).identifier);
        assertEquals("Detail", shippingMethods.get(0).detail);
        assertEquals("Label", shippingMethods.get(0).label);
        assertEquals(BigDecimal.valueOf(3.95), shippingMethods.get(0).amount);
    }

    @Test
    public void parcelable() {
        ShippingMethod input = new ShippingMethod("identifier", "detail",
                "label", BigDecimal.valueOf(1.00));

        Parcel parcel = Parcel.obtain();
        input.writeToParcel(parcel, input.describeContents());
        parcel.setDataPosition(0);

        ShippingMethod output = ShippingMethod.CREATOR.createFromParcel(parcel);
        assertEquals("identifier", output.identifier);
        assertEquals("detail", output.detail);
        assertEquals("label", output.label);
        assertEquals(BigDecimal.valueOf(1.00).stripTrailingZeros(), output.amount.stripTrailingZeros());
    }
}
