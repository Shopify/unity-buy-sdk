package com.shopify.unity.buy.androidpay.view.viewmodel;

import android.support.annotation.NonNull;

import com.shopify.unity.buy.androidpay.view.widget.TotalSummaryView;

import java.math.BigDecimal;

/**
 * View model class that is used to populate a {@link TotalSummaryView}.
 */

public final class TotalSummaryViewModel {

    /** Cart subtotal price. */
    public final BigDecimal subtotal;
    /** Cart shipping price. */
    public final BigDecimal shipping;
    /** Cart tax price. */
    public final BigDecimal tax;
    /** Cart total price. */
    public final BigDecimal total;

    public TotalSummaryViewModel(@NonNull BigDecimal subtotal, @NonNull BigDecimal shipping,
                                 @NonNull BigDecimal tax, @NonNull BigDecimal total) {
        this.subtotal = subtotal;
        this.shipping = shipping;
        this.tax = tax;
        this.total = total;
    }
}
