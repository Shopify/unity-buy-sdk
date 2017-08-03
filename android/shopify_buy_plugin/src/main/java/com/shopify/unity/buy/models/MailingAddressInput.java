package com.shopify.unity.buy.models;

import com.google.android.gms.identity.intents.model.UserAddress;
import com.shopify.buy3.pay.PayAddress;
import com.squareup.moshi.JsonAdapter;
import com.squareup.moshi.Moshi;

// A wrapper around PayAddress to convert it to Json.
public class MailingAddressInput {
    private final PayAddress payAddress;

    private static final JsonAdapter<PayAddress> jsonAdapter =
            new Moshi.Builder().build().adapter(PayAddress.class);

    public MailingAddressInput(UserAddress userAddress) {
        payAddress = PayAddress.fromUserAddress(userAddress);
    }

    public String toJsonString() {
        return jsonAdapter.toJson(payAddress);
    }
}
