package com.shopify.unity.buy;

import org.json.JSONException;
import org.json.JSONObject;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.junit.runners.JUnit4;

import java.util.UUID;

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

        UnityMessage messageA = UnityMessage.fromAndroid(expectedContent);
        String messageAJsonString = messageA.toJson().toString();
        JSONObject jsonA = new JSONObject(messageAJsonString);

        UnityMessage messageB = UnityMessage.fromAndroid(expectedContent);
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

    @Test
    public void testDeserializeFromUnity() throws JSONException {
        final String identifierKey = UnityMessage.IDENTIFIER_KEY;
        final String contentKey = UnityMessage.CONTENT_KEY;
        final String expectedContent = "expectedContent";
        final String identifier = UUID.randomUUID().toString();

        final String jsonFromUnity =
                "{ \"" + identifierKey + "\": \"" + identifier + "\", " +
                "  \"" + contentKey + "\": \"" + expectedContent + "\" }";

        UnityMessage message = UnityMessage.fromUnity(jsonFromUnity);
        assertEquals(identifier, message.identifier);
        assertEquals(expectedContent, message.content);
    }
}
