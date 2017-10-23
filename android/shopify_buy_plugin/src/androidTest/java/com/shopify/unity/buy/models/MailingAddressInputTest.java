package com.shopify.unity.buy.models;

import android.support.test.filters.SmallTest;
import android.support.test.runner.AndroidJUnit4;

import com.google.android.gms.identity.intents.model.UserAddress;

import org.json.JSONException;
import org.junit.Test;
import org.junit.runner.RunWith;

import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;

import static junit.framework.Assert.assertEquals;

@RunWith(AndroidJUnit4.class)
@SmallTest
public class MailingAddressInputTest {
    @Test
    public void testToJsonString() throws NoSuchMethodException, IllegalAccessException, InvocationTargetException, InstantiationException, JSONException {

        final Constructor<UserAddress> constructor = UserAddress.class.getDeclaredConstructor(int.class, String.class, String.class,
                String.class, String.class, String.class, String.class, String.class, String.class, String.class, String.class, String.class,
                String.class, boolean.class, String.class, String.class);
        constructor.setAccessible(true);
        UserAddress userAddress = constructor.newInstance(1, "firstName lastName", "address1", "address2", "address3", "address4", "address5",
                "administrativeArea", "locality", "countryCode", "postalCode", "sortingCode", "phoneNumber", false, "companyName", "emailAddress");

        MailingAddressInput mailingAddress = new MailingAddressInput(userAddress);
        String jsonString = mailingAddress.toJson().toString();
        MailingAddressInput outputMailingAddress = MailingAddressInput.fromJsonString(jsonString);

        assertEquals(mailingAddress.address1, outputMailingAddress.address1);
        assertEquals(mailingAddress.address2, outputMailingAddress.address2);
        assertEquals(mailingAddress.city, outputMailingAddress.city);
        assertEquals(mailingAddress.country, outputMailingAddress.country);
        assertEquals(mailingAddress.firstName, outputMailingAddress.firstName);
        assertEquals(mailingAddress.lastName, outputMailingAddress.lastName);
        assertEquals(mailingAddress.phone, outputMailingAddress.phone);
        assertEquals(mailingAddress.province, outputMailingAddress.province);
        assertEquals(mailingAddress.zip, outputMailingAddress.zip);
    }
}
