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
