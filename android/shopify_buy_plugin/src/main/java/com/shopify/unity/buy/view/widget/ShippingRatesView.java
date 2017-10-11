package com.shopify.unity.buy.view.widget;

import android.content.Context;
import android.support.annotation.VisibleForTesting;
import android.support.constraint.ConstraintLayout;
import android.util.AttributeSet;
import android.view.View;
import android.widget.TextView;

import com.shopify.unity.buy.R;
import com.shopify.unity.buy.view.viewmodel.ShippingRatesViewModel;

import java.text.NumberFormat;

/**
 * Custom view that shows shipping information.
 */

public final class ShippingRatesView extends ConstraintLayout
        implements Updatable<ShippingRatesViewModel> {

    /** Localized currency formatter. */
    private static final NumberFormat CURRENCY_FORMAT = NumberFormat.getCurrencyInstance();

    /** Shipping line description label. */
    @VisibleForTesting TextView shippingLine;
    /** Shipping price label. */
    @VisibleForTesting TextView price;

    public ShippingRatesView(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();
        shippingLine = findViewById(R.id.shipping_line);
        price = findViewById(R.id.price);
        findViewById(R.id.change).setOnClickListener(new OnClickListener() {
            @Override public void onClick(View view) {
                onChangeClick();
            }
        });
    }

    @Override
    public void update(ShippingRatesViewModel shippingRatesViewModel) {
        shippingLine.setText(shippingRatesViewModel.shippingLine);
        price.setText(CURRENCY_FORMAT.format(shippingRatesViewModel.price));
    }

    private void onChangeClick() {
        // TODO show shipping method selection dialog
    }
}
