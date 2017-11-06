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
