package com.shopify.unity.buy.models;

import org.json.JSONException;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.junit.runners.JUnit4;

import java.io.IOException;
import java.math.BigDecimal;
import java.math.MathContext;

import static junit.framework.Assert.assertEquals;

@RunWith(JUnit4.class)
public class PricingLineItemsTest {
    private static final String jsonString =
            "{\"totalPrice\":\"6.46\",\"subtotal\":\"5.23\",\"taxPrice\":\"1.23\"}";

    @Test
    public void testFromJsonString() throws JSONException {
        PricingLineItems items = PricingLineItems.fromJsonString(jsonString);

        assertEquals(items.subtotal, BigDecimal.valueOf(5.23));
        assertEquals(items.taxPrice, BigDecimal.valueOf(1.23));
        assertEquals(items.totalPrice, BigDecimal.valueOf(6.46));
    }

    @Test
    public void testToJson() throws JSONException {
        PricingLineItems items = PricingLineItems.fromJsonString(jsonString);

        assertEquals(items.subtotal, BigDecimal.valueOf(5.23));
        assertEquals(items.taxPrice, BigDecimal.valueOf(1.23));
        assertEquals(items.totalPrice, BigDecimal.valueOf(6.46));

        String resultJsonString = items.toJsonString();
        assertEquals(resultJsonString, jsonString);
    }
}
