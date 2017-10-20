package com.shopify.unity.buy.androidpay.view.viewmodel;

import android.support.annotation.NonNull;

import com.shopify.unity.buy.androidpay.view.widget.TotalSummaryView;

import java.math.BigDecimal;
import java.math.RoundingMode;

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
        this.subtotal = subtotal.setScale(2, RoundingMode.HALF_EVEN);
        this.shipping = shipping.setScale(2, RoundingMode.HALF_EVEN);
        this.tax = tax.setScale(2, RoundingMode.HALF_EVEN);
        this.total = total.setScale(2, RoundingMode.HALF_EVEN);
    }
}
