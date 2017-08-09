package com.shopify.unity.buy.models;

import org.junit.Test;

import java.math.BigDecimal;

import static junit.framework.Assert.assertEquals;

public class BigDecimalAdapterTest {
    @Test
    public void testToJson() {
        BigDecimalAdapter adapter = new BigDecimalAdapter();
        String result = adapter.toJson(BigDecimal.valueOf(1.23));
        assertEquals(result, "1.23");
    }

    @Test
    public void testFromJson() {
        BigDecimalAdapter adapter = new BigDecimalAdapter();
        BigDecimal result = adapter.fromJson("1.23");
        assertEquals(result, BigDecimal.valueOf(1.23));
    }
}
