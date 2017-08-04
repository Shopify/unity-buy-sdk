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
