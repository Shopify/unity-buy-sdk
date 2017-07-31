package com.shopify.unity.buy;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import org.junit.Test;

import java.lang.reflect.Type;
import java.util.Map;

import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertFalse;
import static junit.framework.Assert.assertTrue;

public class UnityMessageTest {

    @Test
    public void test_serialize() {

        final String identifierKey = "Identifier";
        final String contentKey = "Content";
        final String expectedContent = "expectedContent";

        Type type = new TypeToken<Map<String, String>>(){}.getType();
        Gson gson = new Gson();

        UnityMessage messageA = new UnityMessage(expectedContent);
        String messageAJsonString = messageA.toJsonString();
        Map<String, String> jsonA = gson.fromJson(messageAJsonString, type);

        UnityMessage messageB = new UnityMessage(expectedContent);
        String messageBJsonString = messageB.toJsonString();
        Map<String, String> jsonB = gson.fromJson(messageBJsonString, type);

        assertTrue(jsonA.containsKey(identifierKey));
        assertTrue(jsonA.containsKey(contentKey));
        assertEquals(jsonA.get(contentKey), expectedContent);

        String indentifierA = jsonA.get(identifierKey);
        String indentifierB = jsonB.get(identifierKey);

        assertFalse(indentifierA.equals(indentifierB));
        assertEquals(jsonA.get(contentKey), jsonB.get(contentKey));
    }
}
