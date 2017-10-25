package com.shopify.unity.buy.models;

import org.json.JSONObject;
import org.junit.Test;

import static org.junit.Assert.assertEquals;

/**
 * @author Flavio Faria
 */
public class ShippingContactTest {

    @Test
    public void fromJson() throws Exception {
        String json = "{" +
                "    \"address1\": \"address1\"," +
                "    \"address2\": \"address2\"," +
                "    \"city\": \"city\"," +
                "    \"country\": \"country\"," +
                "    \"firstName\": \"firstName\"," +
                "    \"lastName\": \"lastName\"," +
                "    \"phone\": \"phone\"," +
                "    \"province\": \"province\"," +
                "    \"zip\": \"zip\"," +
                "    \"email\": \"email\"" +
                "}";
        ShippingContact shippingContact = ShippingContact.fromJson(new JSONObject(json));
        assertEquals("address1", shippingContact.address1);
        assertEquals("address2", shippingContact.address2);
        assertEquals("city", shippingContact.city);
        assertEquals("country", shippingContact.country);
        assertEquals("firstName", shippingContact.firstName);
        assertEquals("lastName", shippingContact.lastName);
        assertEquals("phone", shippingContact.phone);
        assertEquals("province", shippingContact.province);
        assertEquals("zip", shippingContact.zip);
        assertEquals("email", shippingContact.email);
    }

    @Test
    public void toJson() throws Exception {
        ShippingContact shippingContact = new ShippingContact(
                new MailingAddressInput(
                        "address1",
                        "address2",
                        "city",
                        "country",
                        "firstName",
                        "lastName",
                        "phone",
                        "province",
                        "zip"
                ),
                "email"
        );
        JSONObject json = shippingContact.toJson();
        assertEquals("address1", json.getString("address1"));
        assertEquals("address2", json.getString("address2"));
        assertEquals("city", json.getString("city"));
        assertEquals("country", json.getString("country"));
        assertEquals("firstName", json.getString("firstName"));
        assertEquals("lastName", json.getString("lastName"));
        assertEquals("phone", json.getString("phone"));
        assertEquals("province", json.getString("province"));
        assertEquals("zip", json.getString("zip"));
        assertEquals("email", json.getString("email"));
    }
}
