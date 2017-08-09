package com.shopify.unity.buy.models;

import com.squareup.moshi.FromJson;
import com.squareup.moshi.ToJson;

import java.math.BigDecimal;

class BigDecimalAdapter {
    @FromJson
    BigDecimal fromJson(String number) {
        return new BigDecimal(number).setScale(2, BigDecimal.ROUND_FLOOR);
    }

    @ToJson
    String toJson(BigDecimal number) {
        return number.toString();
    }
}
