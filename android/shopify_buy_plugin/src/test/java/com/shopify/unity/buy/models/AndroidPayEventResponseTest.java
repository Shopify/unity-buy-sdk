package com.shopify.unity.buy.models;

import org.junit.Test;

import static org.junit.Assert.assertEquals;
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
                "\"requiresShipping\": true" +
                "}";
        AndroidPayEventResponse response = AndroidPayEventResponse.fromJsonString(json);
        assertEquals("Merchant Name", response.merchantName);
        assertNotNull(response.pricingLineItems.subtotal);
        assertEquals("CAD", response.currencyCode);
        assertEquals("CA", response.countryCode);
        assertEquals(true, response.requiresShipping);
    }

}
