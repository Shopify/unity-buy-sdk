package com.shopify.unity.buy.models;

import org.json.JSONObject;

/**
 * Interface that defines a type that can be represented as a {@link JSONObject}.
 */
public interface JsonSerializable {
    /** @return the JSON representation of this object as a {@link JSONObject} */
    JSONObject toJson();
}
