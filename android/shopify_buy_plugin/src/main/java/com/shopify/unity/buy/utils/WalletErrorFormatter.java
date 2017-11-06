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
package com.shopify.unity.buy.utils;

import com.google.android.gms.wallet.WalletConstants;

public final class WalletErrorFormatter {
    private WalletErrorFormatter() { }

    public static String errorStringFromCode(int errorCode) {
        switch (errorCode) {
            case WalletConstants.ERROR_CODE_AUTHENTICATION_FAILURE:
                return "AUTHENTICATION_FAILURE";
            case WalletConstants.ERROR_CODE_BUYER_ACCOUNT_ERROR:
                return "BUYER_ACCOUNT_ERROR";
            case WalletConstants.ERROR_CODE_INVALID_PARAMETERS:
                return "INVALID_PARAMETERS";
            case WalletConstants.ERROR_CODE_INVALID_TRANSACTION:
                return "INVALID_TRANSACTION";
            case WalletConstants.ERROR_CODE_MERCHANT_ACCOUNT_ERROR:
                return "MERCHANT_ACCOUNT_ERROR";
            case WalletConstants.ERROR_CODE_SERVICE_UNAVAILABLE:
                return "SERVICE_UNAVAILABLE";
            case WalletConstants.ERROR_CODE_SPENDING_LIMIT_EXCEEDED:
                return "SPENDING_LIMIT_EXCEEDED";
            case WalletConstants.ERROR_CODE_UNKNOWN:
                return "UNKNOWN_WALLET_ERROR";
            case WalletConstants.ERROR_CODE_UNSUPPORTED_API_VERSION:
                return "UNSUPPORTED_API_VERSION";
            default:
                return "UNKNOWN_NON_WALLET_ERROR";
        }
    }
}
