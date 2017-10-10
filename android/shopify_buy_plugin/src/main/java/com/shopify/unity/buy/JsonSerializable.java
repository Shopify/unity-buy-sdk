package com.shopify.unity.buy;

import org.json.JSONException;

public interface JsonSerializable {
    String toJsonString() throws JSONException;
}
