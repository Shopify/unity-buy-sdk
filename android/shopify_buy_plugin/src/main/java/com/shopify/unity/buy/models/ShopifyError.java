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
