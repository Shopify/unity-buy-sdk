package com.shopify.unity.buy.models;

import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import com.shopify.unity.buy.JsonSerializable;
import com.squareup.moshi.JsonAdapter;
import com.squareup.moshi.Moshi;

import java.io.IOException;
import java.math.BigDecimal;

public final class PricingLineItems implements JsonSerializable {
    @NonNull public final BigDecimal subtotal;
    @NonNull public final BigDecimal taxPrice;
    @NonNull public final BigDecimal totalPrice;

    @Nullable public final BigDecimal shippingPrice;

    private static final JsonAdapter<PricingLineItems> jsonAdapter =
            new Moshi.Builder().add(new BigDecimalAdapter()).build().adapter(PricingLineItems.class);

    private PricingLineItems() {
        this.totalPrice = BigDecimal.ZERO;
        this.subtotal = BigDecimal.ZERO;
        this.taxPrice = BigDecimal.ZERO;
        this.shippingPrice = null;
    }

    public static PricingLineItems fromJsonString(String jsonString) throws IOException {
        return jsonAdapter.fromJson(jsonString);
    }

    @Override
    public String toJsonString() {
        return jsonAdapter.toJson(this);
    }
}
