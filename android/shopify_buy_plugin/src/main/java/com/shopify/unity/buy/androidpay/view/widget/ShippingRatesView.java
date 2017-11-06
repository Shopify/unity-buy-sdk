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
import android.support.annotation.Nullable;
import android.support.annotation.VisibleForTesting;
import android.support.constraint.ConstraintLayout;
import android.util.AttributeSet;
import android.view.View;
import android.widget.TextView;

import com.shopify.unity.buy.R;
import com.shopify.unity.buy.androidpay.view.viewmodel.ShippingRatesViewModel;
import com.shopify.unity.buy.models.ShippingMethod;

import java.text.NumberFormat;

/**
 * Custom view that shows shipping information.
 */

public final class ShippingRatesView extends ConstraintLayout {

    /** Localized currency formatter. */
    private static final NumberFormat CURRENCY_FORMAT = NumberFormat.getCurrencyInstance();

    /** Shipping line description label. */
    private TextView shippingLineView;
    /** Shipping price label. */
    private TextView priceView;
    /** Change shipping method label. */
    private View changeView;

    public ShippingRatesView(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();
        shippingLineView = findViewById(R.id.shipping_line);
        priceView = findViewById(R.id.price);
        changeView = findViewById(R.id.change);
    }

    public void setListener(@Nullable final Listener listener) {
        if (listener != null) {
            changeView.setOnClickListener(new OnClickListener() {
                @Override public void onClick(View view) {
                    listener.onShippingRateChangeClick();
                }
            });
        } else {
            changeView.setOnClickListener(null);
        }
    }

    /**
     * Updates this view with the passed view model.
     *
     * @param shippingRatesViewModel the view model with data to render this view
     */
    public void update(ShippingRatesViewModel shippingRatesViewModel) {
        final ShippingMethod currentShippingMethod =
                shippingRatesViewModel.getCurrentShippingMethod();
        shippingLineView.setText(currentShippingMethod.label);
        priceView.setText(CURRENCY_FORMAT.format(currentShippingMethod.amount));
    }

    @VisibleForTesting
    TextView getShippingLineView() {
        return shippingLineView;
    }

    @VisibleForTesting
    TextView getPriceView() {
        return priceView;
    }

    /**
     * Listener interface to observe clicks on the shipping rate selection view.
     */
    public interface Listener {

        /**
         * Invoked when the user clicks on the shipping rate selection view.
         */
        void onShippingRateChangeClick();
    }
}
