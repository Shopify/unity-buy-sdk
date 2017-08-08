package com.shopify.unity.buy;

import org.json.JSONException;
import org.json.JSONObject;
import org.junit.Test;

import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertFalse;
import static junit.framework.Assert.assertTrue;

public class UnityMessageTest {

    @Test
    public void testSerialize() throws JSONException {

        final String identifierKey = UnityMessage.IDENTIFIER_KEY;
        final String contentKey = UnityMessage.CONTENT_KEY;
        final String expectedContent = "expectedContent";

        UnityMessage messageA = UnityMessage.fromAndroid(expectedContent);
        String messageAJsonString = messageA.toJsonString();
        JSONObject jsonA = new JSONObject(messageAJsonString);

        UnityMessage messageB = UnityMessage.fromAndroid(expectedContent);
        String messageBJsonString = messageB.toJsonString();
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
