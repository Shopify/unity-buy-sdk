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
                    listener.onClose();
                }
            });
        } else {
            toolbar.setNavigationOnClickListener(null);
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

    public interface Listener {
        void onClose();
    }
}
