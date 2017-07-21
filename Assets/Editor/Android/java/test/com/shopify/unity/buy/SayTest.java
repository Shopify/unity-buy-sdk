package com.shopify.unity.buy;

import org.junit.Test;
import static org.junit.Assert.*;

public class SayTest {

    @Test
    public void testSayHelloSaysHello() throws Exception {
        assertEquals(Say.sayHello(), "Hello");
    }
}

