package com.shopify.unity.buy;

import android.content.Intent;

import com.google.android.gms.wallet.MaskedWallet;
import com.shopify.unity.buy.androidpay.ConfirmationActivity;
import com.shopify.unity.buy.androidpay.UnityAndroidPayFragment;

/**
 * Class that holds a {@link MaskedWallet} instance to be shared across
 * {@link UnityAndroidPayFragment} and {@link ConfirmationActivity}.
 *
 * The masked wallet instance cannot be shared by {@link Intent}, since both components
 * need to use the same object, not just a copy.
 */

public final class MaskedWalletHolder {

    /**
     * Unique shared {@link MaskedWallet} object.
     */
    public static MaskedWallet maskedWallet;

    private MaskedWalletHolder() {

    }
}
