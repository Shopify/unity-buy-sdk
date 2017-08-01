package com.shopify.unity.buy;

import org.junit.Test;
import static junit.framework.Assert.*;

public class UnityBuyPluginTest {
    @Test
    public void testAdd() {
        assertEquals(2, UnityBuyPlugin.add(1, 1));
    }
}
