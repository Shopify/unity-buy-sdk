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

import org.json.JSONObject;
import org.junit.Test;

import static org.junit.Assert.assertEquals;

/**
 * @author Flavio Faria
 */
public class ShippingContactTest {

    @Test
    public void fromJson() throws Exception {
        String json = "{" +
                "    \"address1\": \"address1\"," +
                "    \"address2\": \"address2\"," +
                "    \"city\": \"city\"," +
                "    \"country\": \"country\"," +
                "    \"firstName\": \"firstName\"," +
                "    \"lastName\": \"lastName\"," +
                "    \"phone\": \"phone\"," +
                "    \"province\": \"province\"," +
                "    \"zip\": \"zip\"," +
                "    \"email\": \"email\"" +
                "}";
        ShippingContact shippingContact = ShippingContact.fromJson(new JSONObject(json));
        assertEquals("address1", shippingContact.address1);
        assertEquals("address2", shippingContact.address2);
        assertEquals("city", shippingContact.city);
        assertEquals("country", shippingContact.country);
        assertEquals("firstName", shippingContact.firstName);
        assertEquals("lastName", shippingContact.lastName);
        assertEquals("phone", shippingContact.phone);
        assertEquals("province", shippingContact.province);
        assertEquals("zip", shippingContact.zip);
        assertEquals("email", shippingContact.email);
    }

    @Test
    public void toJson() throws Exception {
        ShippingContact shippingContact = new ShippingContact(
                new MailingAddressInput(
                        "address1",
                        "address2",
                        "city",
                        "country",
                        "firstName",
                        "lastName",
                        "phone",
                        "province",
                        "zip"
                ),
                "email"
        );
        JSONObject json = shippingContact.toJson();
        assertEquals("address1", json.getString("address1"));
        assertEquals("address2", json.getString("address2"));
        assertEquals("city", json.getString("city"));
        assertEquals("country", json.getString("country"));
        assertEquals("firstName", json.getString("firstName"));
        assertEquals("lastName", json.getString("lastName"));
        assertEquals("phone", json.getString("phone"));
        assertEquals("province", json.getString("province"));
        assertEquals("zip", json.getString("zip"));
        assertEquals("email", json.getString("email"));
    }
}
