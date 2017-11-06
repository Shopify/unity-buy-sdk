package com.shopify.unity.buy.models;

import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import org.json.JSONException;
import org.json.JSONObject;

/**
 * Model class that represents an error that happens on the Unity side.
 */
public final class ShopifyError {

    /** JSON name for the <i>error type</i> attribute. */
    private static final String ERROR_TYPE = "ErrorType";
    /** JSON name for the <i>description</i> attribute. */
    private static final String DESCRIPTION = "Description";

    public enum ErrorType {
        HTTP("HTTP"),
        GRAPH_QL("GraphQL"),
        USER_ERROR("UserError"),
        NATIVE_PAYMENT_PROCESSING_ERROR("NativePaymentProcessingError");

        private final String name;

        ErrorType(String name) {
            this.name = name;
        }

        @Nullable
        private static ErrorType ofName(@NonNull String name) {
            for (ErrorType errorType : ErrorType.values()) {
                if (errorType.name.equals(name)) {
                    return errorType;
                }
            }
            return null;
        }
    }

    /** The error type. */
    public final ErrorType errorType;
    /** The description of the error. */
    public final String description;

    /**
     * Creates an instance of this class.
     *
     * @param errorType the {@link ErrorType}
     * @param description the description of the error
     */
    public ShopifyError(ErrorType errorType, String description) {
        this.errorType = errorType;
        this.description = description;
    }

    /**
     * Parses a JSON object into a {@code ShopifyError} object.
     *
     * @param json a {@code ShopifyError} represented as a string
     * @return a {@code ShopifyError} representation of {@code jsonString}
     * @throws JSONException if {@code json} does not match the object definition
     * of {@code ShopifyError}
     */
    public static ShopifyError fromJson(JSONObject json) throws JSONException {
        final ErrorType errorType = ErrorType.ofName(json.getString(ERROR_TYPE));
        final String description = json.getString(DESCRIPTION);
        return new ShopifyError(errorType, description);
    }

    @Override
    public String toString() {
        return "ShopifyError(errorType = " + errorType + ", description = " + description + ")";
    }
}
