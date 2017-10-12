package com.shopify.unity.buy.view.widget;

import android.content.Context;
import android.support.annotation.Nullable;
import android.support.v4.app.FragmentManager;
import android.support.v7.app.AppCompatActivity;
import android.util.AttributeSet;
import android.widget.LinearLayout;

import com.google.android.gms.wallet.MaskedWallet;
import com.google.android.gms.wallet.WalletConstants;
import com.google.android.gms.wallet.fragment.SupportWalletFragment;
import com.google.android.gms.wallet.fragment.WalletFragmentMode;
import com.google.android.gms.wallet.fragment.WalletFragmentOptions;
import com.google.android.gms.wallet.fragment.WalletFragmentStyle;
import com.shopify.buy3.pay.PayHelper;
import com.shopify.unity.buy.MaskedWalletHolder;
import com.shopify.unity.buy.R;
import com.shopify.unity.buy.view.viewmodel.ConfirmationViewModel;

/**
 * Custom view that shows a checkout confirmation screen with broken down prices,
 * shipping and payment information.
 */

public final class ConfirmationView extends LinearLayout
        implements Updatable<ConfirmationViewModel> {

    /** A subview that shows broken down prices. */
    private TotalSummaryView totalSummaryView;
    /** A subview that shows shipping rates. */
    private ShippingRatesView shippingRatesView;

    public ConfirmationView(Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();
        totalSummaryView = findViewById(R.id.total_summary);
        shippingRatesView = findViewById(R.id.shipping_rates);
    }

    @Override
    public void update(ConfirmationViewModel confirmationViewModel) {
        totalSummaryView.update(confirmationViewModel.totalSummaryViewModel);
        shippingRatesView.update(confirmationViewModel.shippingRatesViewModel);

        final AppCompatActivity activity = (AppCompatActivity) getContext();
        final FragmentManager fragmentManager = activity.getSupportFragmentManager();
        final SupportWalletFragment walletFragment = (SupportWalletFragment) fragmentManager
                .findFragmentById(R.id.android_pay_layout);

        final MaskedWallet maskedWallet = MaskedWalletHolder.maskedWallet;
        if (walletFragment != null) {
            walletFragment.updateMaskedWallet(maskedWallet);
        } else {
            final SupportWalletFragment newWalletFragment = newSupportWalletFragment();
            PayHelper.initializeWalletFragment(newWalletFragment, maskedWallet);
            activity.getSupportFragmentManager()
                    .beginTransaction()
                    .replace(R.id.android_pay_layout, newWalletFragment)
                    .commit();
        }
    }

    /**
     * @return a stylized {@link SupportWalletFragment} to be attached as a subview of this one.
     */
    private SupportWalletFragment newSupportWalletFragment() {
        final WalletFragmentStyle walletFragmentStyle = new WalletFragmentStyle()
                .setMaskedWalletDetailsHeaderTextAppearance(R.style.WalletDetailsHeaderTextAppearance)
                .setMaskedWalletDetailsTextAppearance(R.style.WalletDetailsTextAppearance)
                .setMaskedWalletDetailsBackgroundColor(android.R.color.transparent)
                .setMaskedWalletDetailsButtonBackgroundColor(android.R.color.transparent)
                .setMaskedWalletDetailsButtonTextAppearance(R.style.WalletDetailsButton);

        final WalletFragmentOptions walletFragmentOptions = WalletFragmentOptions.newBuilder()
                .setEnvironment(WalletConstants.ENVIRONMENT_SANDBOX) // TODO parametrize
                .setFragmentStyle(walletFragmentStyle)
                .setTheme(WalletConstants.THEME_LIGHT)
                .setMode(WalletFragmentMode.SELECTION_DETAILS)
                .build();

        return SupportWalletFragment.newInstance(walletFragmentOptions);
    }
}
