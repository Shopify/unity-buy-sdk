package com.shopify.unity.buy.androidpay.view.viewmodel;

import android.support.annotation.NonNull;

import com.shopify.unity.buy.androidpay.view.widget.ShippingRatesView;

import java.math.BigDecimal;

/**
 * View model class that is used to populate a {@link ShippingRatesView}.
 */

public final class ShippingRatesViewModel {

    /** Shipping line description. */
    public final String shippingLine;
    /** Shipping price. */
    public final BigDecimal price;

    public ShippingRatesViewModel(@NonNull String shippingLine, @NonNull BigDecimal price) {
        this.shippingLine = shippingLine;
        this.price = price;
    }
}
