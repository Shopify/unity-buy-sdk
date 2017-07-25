package com.shopify.unity.buy;

import org.junit.Test;
import static junit.framework.Assert.*;

public class UnityBuyPluginTest {
    @Test
    public void test_add() {
        assertEquals(2, UnityBuyPlugin.add(1, 1));
    }
}
