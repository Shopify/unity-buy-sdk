package com.shopify.unity.buy;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.UUID;

public final class UnityMessage implements JsonSerializable {

    public static final String CONTENT_KEY = "Content";
    public static final String IDENTIFIER_KEY = "Identifier";

    public final String content;
    public final String identifier;

    private UnityMessage(String content) {
        this(UUID.randomUUID().toString(), content);
    }

    private UnityMessage(String identifier, String content) {
        this.identifier = identifier;
        this.content = content;
    }

    public static UnityMessage fromUnity(String identifier, String message) {
        return new UnityMessage(identifier, message);
    }

    public static UnityMessage fromAndroid(String content) {
        return new UnityMessage(content);
    }

    @Override
    public String toJsonString() {

        JSONObject json = new JSONObject();

        try {
            json.put(IDENTIFIER_KEY, identifier);
            json.put(CONTENT_KEY, content);
        } catch (JSONException e) {
            throw new IllegalStateException("UnityMessage is not JsonSerializable");
        }
        return json.toString();
    }
}
