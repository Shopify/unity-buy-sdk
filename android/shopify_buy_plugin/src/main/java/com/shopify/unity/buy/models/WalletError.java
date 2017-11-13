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
import android.util.SparseArray;

import com.google.android.gms.wallet.WalletConstants;

/**
 * Holds a list of possible wallet errors in a type-safe manner.
 */

public enum WalletError {

    AUTHENTICATION_FAILURE("AUTHENTICATION_FAILURE"),
    BUYER_ACCOUNT_ERROR("BUYER_ACCOUNT_ERROR"),
    INVALID_PARAMETERS("INVALID_PARAMETERS"),
    INVALID_TRANSACTION("INVALID_TRANSACTION"),
    MERCHANT_ACCOUNT_ERROR("MERCHANT_ACCOUNT_ERROR"),
    SERVICE_UNAVAILABLE("SERVICE_UNAVAILABLE"),
    SPENDING_LIMIT_EXCEEDED("SPENDING_LIMIT_EXCEEDED"),
    UNKNOWN("UNKNOWN_WALLET_ERROR"),
    UNSUPPORTED_API_VERSION("UNSUPPORTED_API_VERSION"),
    NON_WALLET("UNKNOWN_NON_WALLET_ERROR");

    private final String name;
    private static final SparseArray<WalletError> ERROR_TABLE = new SparseArray<>();

    static {
        ERROR_TABLE.put(WalletConstants.ERROR_CODE_AUTHENTICATION_FAILURE, AUTHENTICATION_FAILURE);
        ERROR_TABLE.put(WalletConstants.ERROR_CODE_BUYER_ACCOUNT_ERROR, BUYER_ACCOUNT_ERROR);
        ERROR_TABLE.put(WalletConstants.ERROR_CODE_INVALID_PARAMETERS, INVALID_PARAMETERS);
        ERROR_TABLE.put(WalletConstants.ERROR_CODE_INVALID_TRANSACTION, INVALID_TRANSACTION);
        ERROR_TABLE.put(WalletConstants.ERROR_CODE_MERCHANT_ACCOUNT_ERROR, MERCHANT_ACCOUNT_ERROR);
        ERROR_TABLE.put(WalletConstants.ERROR_CODE_SERVICE_UNAVAILABLE, SERVICE_UNAVAILABLE);
        ERROR_TABLE.put(WalletConstants.ERROR_CODE_SPENDING_LIMIT_EXCEEDED, SPENDING_LIMIT_EXCEEDED);
        ERROR_TABLE.put(WalletConstants.ERROR_CODE_UNKNOWN, UNKNOWN);
        ERROR_TABLE.put(WalletConstants.ERROR_CODE_UNSUPPORTED_API_VERSION, UNSUPPORTED_API_VERSION);
    }

    /**
     * Gets a {@link WalletError} for a wallet error code, such as
     * {@link WalletConstants#ERROR_CODE_AUTHENTICATION_FAILURE}
     * or any other constant in {@link WalletConstants} that starts
     * with {@code ERROR_CODE_}.
     *
     * @param code the error constant defined in {@code WalletConstants}
     * @return the respective {@code WalletError} for such constant, or
     *         {@link #NON_WALLET} if the constant is not recognized
     */
    @NonNull
    public static WalletError forWalletErrorCode(int code) {
        final WalletError walletError = ERROR_TABLE.get(code);
        if (walletError == null) {
            return NON_WALLET;
        }
        return walletError;
    }

    WalletError(String name) {
        this.name = name;
    }

    @Override
    public String toString() {
        return name;
    }
}
