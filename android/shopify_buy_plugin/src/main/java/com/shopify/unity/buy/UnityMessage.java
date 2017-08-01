package com.shopify.unity.buy;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.UUID;

public final class UnityMessage implements JsonSerializable {

    public static final String CONTENT_KEY = "Content";
    public static final String IDENTIFIER_KEY = "Identifier";

    public final String content;
    private final String identifier;

    public UnityMessage(String content) {
        identifier = UUID.randomUUID().toString();
        this.content = content;
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
