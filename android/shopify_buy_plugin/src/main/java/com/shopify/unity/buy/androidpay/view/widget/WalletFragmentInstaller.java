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
 * Class the wraps the logic of attaching a {@link WalletFragment} to a {@link FragmentManager}.
 */

public final class WalletFragmentInstaller {

    /** The {@link FragmentManager} to attach a {@link WalletFragment} to. */
    private final FragmentManager fragmentManager;
    /** The {@link MaskedWallet} used to create the {@link WalletFragment}. */
    private final MaskedWallet maskedWallet;

    /**
     * Creates a new instance of this class.
     *
     * @param fragmentManager to attach a {@link WalletFragment} to
     * @param maskedWallet used rto create the {@code WalletFragment}
     */
    public WalletFragmentInstaller(FragmentManager fragmentManager, MaskedWallet maskedWallet) {
        this.fragmentManager = fragmentManager;
        this.maskedWallet = maskedWallet;
    }

    /**
     * Creates and attaches the {@link WalletFragment} to the {@link FragmentManager}.
     */
    void install() {
        final WalletFragment walletFragment = (WalletFragment) fragmentManager
                .findFragmentById(R.id.android_pay_layout);

        if (walletFragment == null) {
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

    /**
     * Detaches the {@link WalletFragment} from the {@link FragmentManager}.
     */
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
