package com.shopify.unity.buy;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.UUID;

public class UnityMessage implements JsonSerializable {

    public static String CONTENT_KEY = "Content";
    public static String IDENTIFIER_KEY = "Identifier";

    public String content;
    private String identifier;

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
