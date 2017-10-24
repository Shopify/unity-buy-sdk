package com.shopify.unity.buy.models;

import org.json.JSONObject;
import org.junit.Test;

import static org.junit.Assert.assertEquals;

/**
 * @author Flavio Faria
 */
public class TokenDataTest {

    @Test
    public void toJson() throws Exception {
        TokenData tokenData = new TokenData("token");
        JSONObject json = tokenData.toJson();
        assertEquals("token", json.getString("TransactionIdentifier"));
        assertEquals("token", json.getString("PaymentData"));
    }
}
