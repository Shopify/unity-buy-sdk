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
