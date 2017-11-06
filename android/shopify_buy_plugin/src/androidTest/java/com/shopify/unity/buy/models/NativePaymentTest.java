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
import org.junit.Before;
import org.junit.Test;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;

/**
 * @author Flavio Faria
 */
public class NativePaymentTest {

    private static MailingAddressInput billingContact;
    private static ShippingContact shippingContact;

    @Before
    public void setUp() throws Exception {
        billingContact = MailingAddressInput.fromJson(new JSONObject("{" +
                "    \"address1\": \"billingAddress1\", " +
                "    \"address2\": \"billingAddress2\", " +
                "    \"city\": \"billingCity\", " +
                "    \"country\": \"billingCcountry\", " +
                "    \"firstName\": \"billingFirstName\", " +
                "    \"lastName\": \"billingLastName\", " +
                "    \"phone\": \"billingPhone\", " +
                "    \"province\": \"billingProvince\", " +
                "    \"zip\": \"billingZip\"" +
                "}"));
        shippingContact = ShippingContact.fromJson(new JSONObject("{" +
                "    \"address1\": \"shippingAddress1\", " +
                "    \"address2\": \"shippingAddress2\", " +
                "    \"city\": \"shippingCity\", " +
                "    \"country\": \"shippinCcountry\", " +
                "    \"firstName\": \"shippingFirstName\", " +
                "    \"lastName\": \"shippingLastName\", " +
                "    \"phone\": \"shippingPhone\", " +
                "    \"province\": \"shippingProvince\", " +
                "    \"zip\": \"shippingZip\", " +
                "    \"email\": \"shippingEmail\"" +
                "}"));
    }

    @Test
    public void newBuilder() throws Exception {
        TokenData tokenData = new TokenData("token");
        NativePayment nativePayment = NativePayment
                .newBuilder()
                .billingContact(billingContact)
                .shippingContact(shippingContact)
                .identifier("identifier")
                .tokenData(tokenData)
                .build();
        assertEquals(billingContact, nativePayment.billingContact);
        assertEquals(shippingContact, nativePayment.shippingContact);
        assertEquals("identifier", nativePayment.identifier);
        assertEquals(tokenData, nativePayment.tokenData);
    }

    @Test(expected = IllegalArgumentException.class)
    public void newValidatesBillingContact() throws Exception {
        TokenData tokenData = new TokenData("token");
         NativePayment
                .newBuilder()
                .shippingContact(shippingContact)
                .identifier("identifier")
                .tokenData(tokenData)
                .build();
    }

    @Test(expected = IllegalArgumentException.class)
    public void newValidatesIdentifier() throws Exception {
        TokenData tokenData = new TokenData("token");
        NativePayment
                .newBuilder()
                .billingContact(billingContact)
                .shippingContact(shippingContact)
                .tokenData(tokenData)
                .build();
    }

    @Test(expected = IllegalArgumentException.class)
    public void newValidatesTokenData() throws Exception {
        NativePayment
                .newBuilder()
                .billingContact(billingContact)
                .shippingContact(shippingContact)
                .identifier("identifier")
                .build();
    }

    @Test
    public void toJson() throws Exception {
        NativePayment nativePayment = NativePayment
                .newBuilder()
                .billingContact(billingContact)
                .shippingContact(shippingContact)
                .identifier("identifier")
                .tokenData(new TokenData("token"))
                .build();
        JSONObject json = nativePayment.toJson();
        assertNotNull(json.getJSONObject("ShippingContact"));
        assertNotNull(json.getJSONObject("ShippingContact"));
        assertEquals("identifier", json.getString("Identifier"));
        assertNotNull(json.getJSONObject("TokenData"));
    }
}
