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

import org.junit.Test;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertNotNull;

/**
 * @author Flavio Faria
 */
public class AndroidPayEventResponseTest {

    @Test
    public void fromJsonString() throws Exception {
        String json = "{" +
                "\"merchantName\": \"Merchant Name\"," +
                "\"pricingLineItems\": {\"subtotal\": \"5.23\", \"taxPrice\": \"1.23\", \"totalPrice\": \"6.46\", \"shippingPrice\": \"0.28\"}," +
                "\"currencyCode\": \"CAD\"," +
                "\"countryCode\": \"CA\"," +
                "\"requiresShipping\": true," +
                "\"shippingMethods\": [{\"Identifier\": \"Identifier\", \"Detail\": \"Detail\", \"Label\": \"Label\", \"Amount\": \"3.45\"}]" +
                "}";
        AndroidPayEventResponse response = AndroidPayEventResponse.fromJsonString(json);
        assertEquals("Merchant Name", response.merchantName);
        assertNotNull(response.pricingLineItems);
        assertEquals("CAD", response.currencyCode);
        assertEquals("CA", response.countryCode);
        assertEquals(true, response.requiresShipping);
        assertFalse(response.shippingMethods.isEmpty());
    }

}
