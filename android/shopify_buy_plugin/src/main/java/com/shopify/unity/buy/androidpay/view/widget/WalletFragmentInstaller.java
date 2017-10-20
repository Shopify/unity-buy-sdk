package com.shopify.unity.buy.androidpay.view.widget;

import android.app.FragmentManager;

import com.google.android.gms.wallet.MaskedWallet;
import com.google.android.gms.wallet.WalletConstants;
import com.google.android.gms.wallet.fragment.WalletFragment;
import com.google.android.gms.wallet.fragment.WalletFragmentInitParams;
import com.google.android.gms.wallet.fragment.WalletFragmentMode;
import com.google.android.gms.wallet.fragment.WalletFragmentOptions;
import com.google.android.gms.wallet.fragment.WalletFragmentStyle;
import com.shopify.buy3.pay.PayHelper;
import com.shopify.unity.buy.R;

/**
 * @author Flavio Faria
 */

public final class WalletFragmentInstaller {

    private final FragmentManager fragmentManager;
    private final MaskedWallet maskedWallet;

    public WalletFragmentInstaller(FragmentManager fragmentManager, MaskedWallet maskedWallet) {
        this.fragmentManager = fragmentManager;
        this.maskedWallet = maskedWallet;
    }

    void install() {
        final WalletFragment walletFragment = (WalletFragment) fragmentManager
                .findFragmentById(R.id.android_pay_layout);

        if (walletFragment != null) {
            walletFragment.updateMaskedWallet(maskedWallet);
        } else {
            final WalletFragment newWalletFragment = newWalletFragment();

            // TODO https://github.com/Shopify/mobile-buy-sdk-android/pull/464
            // PayHelper.initializeWalletFragment(newWalletFragment, maskedWallet);

            final WalletFragmentInitParams initParams = WalletFragmentInitParams.newBuilder()
                    .setMaskedWallet(maskedWallet)
                    .setMaskedWalletRequestCode(PayHelper.REQUEST_CODE_CHANGE_MASKED_WALLET)
                    .build();
            newWalletFragment.initialize(initParams);

            fragmentManager
                    .beginTransaction()
                    .replace(R.id.android_pay_layout, newWalletFragment)
                    .commit();
        }
    }

    void uninstall() {
        fragmentManager
                .beginTransaction()
                .remove(fragmentManager.findFragmentById(R.id.android_pay_layout))
                .commit();
    }

    /**
     * @return a stylized {@link WalletFragment} to be attached as a subview of this one.
     */
    private WalletFragment newWalletFragment() {
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

        return WalletFragment.newInstance(walletFragmentOptions);
    }
}
