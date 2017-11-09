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

import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import com.shopify.buy3.pay.CardNetworkType;

import org.json.JSONArray;
import org.json.JSONException;

import java.util.ArrayList;
import java.util.List;

/**
 * @author Flavio Faria
 */

public final class CardTypeConverter {

    private CardTypeConverter() {

    }

    public static List<CardNetworkType> convertCardBrandsToCardNetworks(
            @NonNull JSONArray cardBrands) throws JSONException {
        final List<CardNetworkType> cardNetworkTypes = new ArrayList<>(cardBrands.length());
        for (int i = 0; i < cardBrands.length(); i++) {
            final String cardBrand = cardBrands.getString(i);
            final CardNetworkType cardNetworkType = convertCardBrandToCardNetwork(cardBrand);
            if (cardNetworkType != null) {
                cardNetworkTypes.add(cardNetworkType);
            }
        }
        return cardNetworkTypes;
    }

    @Nullable
    private static CardNetworkType convertCardBrandToCardNetwork(@NonNull String cardBrand) {
        switch (cardBrand) {
            case "AmEx":
                return CardNetworkType.AMERICAN_EXPRESS;
            case "Discover":
                return CardNetworkType.DISCOVER;
            case "JCB":
                return CardNetworkType.JCB;
            case "MasterCard":
                return CardNetworkType.MASTERCARD;
            case "Visa":
                return CardNetworkType.VISA;
            default:
                return null;
        }
    }
}
