package com.shopify.unity.buy;

import com.google.gson.Gson;
import java.util.UUID;

public class UnityMessage implements JsonSerializable {

    // Using capital case here, so when it is serialized the keys are capitalized
    public String Content;
    private String Identifier;

    public UnityMessage(String content) {
        Identifier = UUID.randomUUID().toString();
        this.Content = content;
    }

    @Override
    public String toJsonString() {
        Gson gson = new Gson();
        return gson.toJson(this);
    }
}
