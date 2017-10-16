package com.shopify.unity.buy;

import com.shopify.unity.buy.models.JsonSerializable;

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

    public static UnityMessage fromUnity(String message) {
        try {
            JSONObject json = new JSONObject(message);
            return new UnityMessage(json.getString(IDENTIFIER_KEY), json.getString(CONTENT_KEY));
        } catch (JSONException e) {
            throw new IllegalArgumentException("UnityMessage from Unity is invalid -- Message: " + message);
        }
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
