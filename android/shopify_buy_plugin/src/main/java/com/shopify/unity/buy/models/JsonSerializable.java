package com.shopify.unity.buy.models;

import org.json.JSONException;

public interface JsonSerializable {
    String toJsonString() throws JSONException;
}
