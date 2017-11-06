/**
 * The MIT License (MIT)
 * Copyright (c) 2017 Shopify
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */
package com.shopify.unity.buy.models;

import android.support.test.filters.SmallTest;
import android.support.test.runner.AndroidJUnit4;

import com.google.android.gms.identity.intents.model.UserAddress;

import org.json.JSONException;
import org.json.JSONObject;
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
        MailingAddressInput outputMailingAddress = MailingAddressInput.fromJson(new JSONObject(jsonString));

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
