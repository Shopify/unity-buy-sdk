/**
 * The MIT License (MIT)
 * Copyright (c) 2017 Shopify
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */
package com.shopify.unity.buy;

import com.shopify.unity.buy.models.JsonSerializable;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.UUID;

/**
 * Class that represents a message exchanged between Android and Unity.
 */
public final class UnityMessage implements JsonSerializable {

    /** JSON name for the <i>content</i> attribute. */
    public static final String CONTENT_KEY = "Content";
    /** JSON name for the <i>identifier</i> attribute. */
    public static final String IDENTIFIER_KEY = "Identifier";

    /** The message content, usually represented as a JSON string. */
    public final String content;
    /** A unique identifier for this message. */
    public final String identifier;

    /**
     * Creates a new instance of this class with a random {@link UUID}
     * for the {@code identifier} field.
     *
     * @param content the message content
     */
    private UnityMessage(String content) {
        this(UUID.randomUUID().toString(), content);
    }

    /**
     * Creates a new instance of this class.
     *
     * @param identifier the message identifier
     * @param content the message content
     */
    private UnityMessage(String identifier, String content) {
        this.identifier = identifier;
        this.content = content;
    }

    /**
     * Creates a {@link UnityMessage} with an auto-generated {@code identifier}.
     *
     * @param content the content to be carried by this message
     * @return a new {@code UnityMessage} object wrapping the content
     */
    public static UnityMessage fromContent(String content) {
        return new UnityMessage(content);
    }

    @Override
    public JSONObject toJson() {
        final JSONObject json = new JSONObject();
        try {
            json.put(IDENTIFIER_KEY, identifier)
                .put(CONTENT_KEY, content);
        } catch (JSONException e) {
            e.printStackTrace();
        }
        return json;
    }
}
