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
import android.support.v7.widget.Toolbar;
import android.util.AttributeSet;
import android.view.View;
import android.widget.Button;
import android.widget.LinearLayout;

import com.shopify.unity.buy.R;
import com.shopify.unity.buy.androidpay.view.viewmodel.ConfirmationViewModel;

/**
 * Custom view that shows a checkout confirmation screen with broken down prices,
 * shipping and payment information.
 */

public final class ConfirmationView extends LinearLayout {

    private Toolbar toolbar;
    /** A subview that shows broken down prices. */
    private TotalSummaryView totalSummaryView;
    /** A subview that shows shipping rates. */
    private ShippingRatesView shippingRatesView;

    private Button confirmView;

    @Nullable private WalletFragmentInstaller walletFragmentInstaller;

    public ConfirmationView(Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();
        toolbar = findViewById(R.id.toolbar);
        toolbar.setTitle(R.string.confirmation_title);
        toolbar.setNavigationIcon(R.drawable.ic_close);
        totalSummaryView = findViewById(R.id.total_summary);
        shippingRatesView = findViewById(R.id.shipping_rates);
        confirmView = findViewById(R.id.confirm);
    }

    public void setListener(@Nullable final Listener listener) {
        if (listener != null) {
            toolbar.setNavigationOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    listener.onCloseButtonClick();
                }
            });
            shippingRatesView.setListener(listener);
            confirmView.setOnClickListener(new OnClickListener() {
                @Override public void onClick(View v) {
                    listener.onConfirmOrderClick();
                }
            });
        } else {
            toolbar.setNavigationOnClickListener(null);
            shippingRatesView.setListener(null);
            confirmView.setOnClickListener(null);
        }
    }

    public void update(@NonNull WalletFragmentInstaller walletFragmentInstaller,
                       @NonNull ConfirmationViewModel confirmationViewModel) {
        this.walletFragmentInstaller = walletFragmentInstaller;
        totalSummaryView.update(confirmationViewModel.totalSummaryViewModel);
        shippingRatesView.update(confirmationViewModel.shippingRatesViewModel);
        walletFragmentInstaller.install();
        confirmView.setEnabled(confirmationViewModel.buttonEnabled);
    }

    @Override
    protected void onDetachedFromWindow() {
        super.onDetachedFromWindow();
        if (walletFragmentInstaller != null) {
            walletFragmentInstaller.uninstall();
            walletFragmentInstaller = null;
        }
    }

    public interface Listener extends ShippingRatesView.Listener {
        void onCloseButtonClick();
        void onConfirmOrderClick();
    }
}
