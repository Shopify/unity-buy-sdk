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
package com.shopify.unity.buy;

import org.json.JSONException;
import org.json.JSONObject;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.junit.runners.JUnit4;

import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertFalse;
import static junit.framework.Assert.assertTrue;

@RunWith(JUnit4.class)
public class UnityMessageTest {

    @Test
    public void testSerializeFromAndroid() throws JSONException {

        final String identifierKey = UnityMessage.IDENTIFIER_KEY;
        final String contentKey = UnityMessage.CONTENT_KEY;
        final String expectedContent = "expectedContent";

        UnityMessage messageA = UnityMessage.fromContent(expectedContent);
        String messageAJsonString = messageA.toJson().toString();
        JSONObject jsonA = new JSONObject(messageAJsonString);

        UnityMessage messageB = UnityMessage.fromContent(expectedContent);
        String messageBJsonString = messageB.toJson().toString();
        JSONObject jsonB = new JSONObject(messageBJsonString);

        assertTrue(jsonA.has(identifierKey));
        assertTrue(jsonA.has(contentKey));
        assertEquals(jsonA.get(contentKey), expectedContent);

        String identifierA = jsonA.getString(identifierKey);
        String identifierB = jsonB.getString(identifierKey);

        assertFalse(identifierA.equals(identifierB));
        assertEquals(jsonA.get(contentKey), jsonB.get(contentKey));
    }
}
