package com.shopify.unity.buy.models;

import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import org.json.JSONException;
import org.json.JSONObject;

public final class ShopifyError {

    private static final String ERROR_TYPE = "ErrorType";
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

    public final ErrorType errorType;
    public final String description;

    public ShopifyError(ErrorType errorType, String description) {
        this.errorType = errorType;
        this.description = description;
    }

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
