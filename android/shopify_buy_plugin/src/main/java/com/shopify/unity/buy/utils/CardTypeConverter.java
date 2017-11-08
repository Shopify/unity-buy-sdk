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
