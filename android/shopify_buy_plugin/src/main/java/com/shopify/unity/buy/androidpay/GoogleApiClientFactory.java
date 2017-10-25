package com.shopify.unity.buy.androidpay;

import android.app.Activity;

import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.wallet.Wallet;
import com.google.android.gms.wallet.WalletConstants;

import static com.google.android.gms.common.api.GoogleApiClient.Builder;

/**
 * @author Flavio Faria
 */

public class GoogleApiClientFactory {

    private final Activity activity;
    private final boolean useTestingEnvironment;

    public static GoogleApiClientFactory of(Activity activity, boolean useTestingEnvironment) {
        return new GoogleApiClientFactory(activity, useTestingEnvironment);
    }

    private GoogleApiClientFactory(Activity activity, boolean useTestingEnvironment) {
        this.activity = activity;
        this.useTestingEnvironment = useTestingEnvironment;
    }

    GoogleApiClient newGoogleApiClient() {
        final int environment = useTestingEnvironment
                ? WalletConstants.ENVIRONMENT_SANDBOX
                : WalletConstants.ENVIRONMENT_PRODUCTION;
        // It requires a Context, but internally it is cast to Activity.
        return new Builder(activity)
                .addApi(Wallet.API, new Wallet.WalletOptions.Builder()
                        .setEnvironment(environment)
                        .setTheme(WalletConstants.THEME_DARK)
                        .build())
                .build();
    }
}
