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
package com.shopify.unity.buy.androidpay.view.widget;

import android.content.Context;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.annotation.VisibleForTesting;
import android.support.constraint.ConstraintLayout;
import android.util.AttributeSet;
import android.widget.TextView;

import com.shopify.unity.buy.R;
import com.shopify.unity.buy.androidpay.view.viewmodel.TotalSummaryViewModel;

import java.text.NumberFormat;

/**
 * Custom view that shows broken down prices for the current checkout, such as subtotal,
 * shipping, tax, and total.
 */

public final class TotalSummaryView extends ConstraintLayout {

    /** Localized currency formatter. */
    private static final NumberFormat CURRENCY_FORMAT = NumberFormat.getCurrencyInstance();

    /** Cart subtotal price label. */
    private TextView subtotal;
    /** Cart shipping price label. */
    private TextView shipping;
    /** Cart tax price label. */
    private TextView tax;
    /** Cart total price label. */
    private TextView total;

    public TotalSummaryView(@NonNull Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();
        subtotal = findViewById(R.id.subtotal);
        shipping = findViewById(R.id.shipping);
        tax = findViewById(R.id.tax);
        total = findViewById(R.id.total);
    }

    /**
     * Updates this view with the passed view model.
     *
     * @param totalSummaryViewModel the view model with data to render this view
     */
    public void update(TotalSummaryViewModel totalSummaryViewModel) {
        subtotal.setText(CURRENCY_FORMAT.format(totalSummaryViewModel.subtotal));
        shipping.setText(CURRENCY_FORMAT.format(totalSummaryViewModel.shipping));
        tax.setText(CURRENCY_FORMAT.format(totalSummaryViewModel.tax));

        final Context context = getContext();
        final String totalValue = CURRENCY_FORMAT.format(totalSummaryViewModel.total);
        final String totalLabel = context.getString(R.string.confirmation_summary_total, totalValue);
        total.setText(totalLabel);
    }

    @VisibleForTesting
    TextView getSubtotal() {
        return subtotal;
    }

    @VisibleForTesting
    TextView getShipping() {
        return shipping;
    }

    @VisibleForTesting
    TextView getTax() {
        return tax;
    }

    @VisibleForTesting
    TextView getTotal() {
        return total;
    }
}
