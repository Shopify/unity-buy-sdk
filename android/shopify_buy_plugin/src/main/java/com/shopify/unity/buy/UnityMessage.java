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
