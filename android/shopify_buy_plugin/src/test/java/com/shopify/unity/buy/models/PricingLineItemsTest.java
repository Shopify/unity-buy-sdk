package com.shopify.unity.buy.models;

import org.junit.Test;

import java.io.IOException;
import java.math.BigDecimal;
import java.math.MathContext;

import static junit.framework.Assert.assertEquals;

public class PricingLineItemsTest {
    private static final String jsonString =
            "{\"subtotal\":\"5.23\",\"taxPrice\":\"1.23\",\"totalPrice\":\"6.46\"}";

    @Test
    public void testFromJsonString() throws IOException {
        PricingLineItems items = PricingLineItems.fromJsonString(jsonString);

        assertEquals(items.subtotal, BigDecimal.valueOf(5.23));
        assertEquals(items.taxPrice, BigDecimal.valueOf(1.23));
        assertEquals(items.totalPrice, BigDecimal.valueOf(6.46));
    }

    @Test
    public void testToJson() throws IOException {
        PricingLineItems items = PricingLineItems.fromJsonString(jsonString);

        assertEquals(items.subtotal, BigDecimal.valueOf(5.23));
        assertEquals(items.taxPrice, BigDecimal.valueOf(1.23));
        assertEquals(items.totalPrice, BigDecimal.valueOf(6.46));

        String resultJsonString = items.toJsonString();
        assertEquals(resultJsonString, jsonString);
    }
}
