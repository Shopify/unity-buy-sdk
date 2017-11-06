package com.shopify.unity.buy.androidpay;

import android.app.Activity;
import android.content.Context;

import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.wallet.Wallet;
import com.google.android.gms.wallet.WalletConstants;

import static com.google.android.gms.common.api.GoogleApiClient.Builder;

/**
 * Factory class to encapsulate the creation of {@link GoogleApiClient} objects.
 */

public class GoogleApiClientFactory {

    /**
     * The {@link Activity} used to create the {@link GoogleApiClient}.
     * The {@code GoogleApiClient} actually requires a {@link Context} to
     * be created, but internally, this {@code Context} is cast to an
     * {@code Activity}, so we provide a type-safety layer here. */
    private final Activity activity;
    private final boolean useTestingEnvironment;

    /**
     * Creates an instance of this class.
     *
     * @param activity used to create new {@link GoogleApiClient} instances
     * @param useTestingEnvironment pass {@code true} if sandbox mode must be used,
     *         {@code false} otherwise
     * @return a new {@code GoogleApiClient}
     */
    public static GoogleApiClientFactory of(Activity activity, boolean useTestingEnvironment) {
        return new GoogleApiClientFactory(activity, useTestingEnvironment);
    }

    /**
     * Creates an instance of this class.
     *
     * @param activity used to create new {@link GoogleApiClient} instances
     * @param useTestingEnvironment pass {@code true} if sandbox mode must be used,
     *         {@code false} otherwise
     */
    private GoogleApiClientFactory(Activity activity, boolean useTestingEnvironment) {
        this.activity = activity;
        this.useTestingEnvironment = useTestingEnvironment;
    }

    /**
     * Creates a new {@link GoogleApiClient}.
     *
     * @return the created {@code GoogleApiClient}
     */
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
