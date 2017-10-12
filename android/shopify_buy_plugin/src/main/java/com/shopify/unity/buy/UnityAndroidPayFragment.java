package com.shopify.unity.buy;

import android.app.Fragment;
import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.Nullable;

import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.wallet.FullWallet;
import com.google.android.gms.wallet.MaskedWallet;
import com.google.android.gms.wallet.Wallet;
import com.google.android.gms.wallet.WalletConstants;
import com.shopify.buy3.pay.PayCart;
import com.shopify.buy3.pay.PayHelper;
import com.shopify.unity.buy.models.AndroidPayEventResponse;
import com.shopify.unity.buy.models.MailingAddressInput;
import com.shopify.unity.buy.models.PricingLineItems;
import com.shopify.unity.buy.utils.Logger;
import com.shopify.unity.buy.utils.WalletErrorFormatter;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class UnityAndroidPayFragment extends Fragment implements GoogleApiClient.ConnectionCallbacks {

    private enum CheckoutState {
        READY,
        REQUESTING_MASKED_WALLET,
        RECEIVED_MASKED_WALLET,
    }

    public static final String EXTRA_PAY_CART = "payCart";
    public static final String EXTRA_COUNTRY_CODE = "countryCode";
    public static final String EXTRA_ANDROID_PAY_ENVIRONMENT = "androidPayEnvironment";
    public static final String EXTRA_PUBLIC_KEY = "publicKey";

    private PayCart cart;
    private String countryCode;
    private String publicKey;
    private int androidPayEnvironment;
    private GoogleApiClient googleApiClient;
    private AndroidPaySessionCallback sessionCallbacks;

    private CheckoutState currentCheckoutState;
    private MaskedWallet maskedWallet;

    static final class UnityAndroidPayFragmentBuilder {
        private static final String[] requiredExtras = {
            EXTRA_PAY_CART,
            EXTRA_COUNTRY_CODE,
            EXTRA_ANDROID_PAY_ENVIRONMENT,
            EXTRA_PUBLIC_KEY
        };

        private Bundle bundle;
        private AndroidPaySessionCallback callbacks;

        UnityAndroidPayFragmentBuilder() {
            bundle = new Bundle();
        }

        UnityAndroidPayFragmentBuilder setPayCart(PayCart cart) {
            bundle.putParcelable(EXTRA_PAY_CART, cart);
            return this;
        }

        UnityAndroidPayFragmentBuilder setCountryCode(String countryCode) {
            bundle.putString(EXTRA_COUNTRY_CODE, countryCode);
            return this;
        }

        UnityAndroidPayFragmentBuilder setEnvironment(int environment) {
            bundle.putInt(EXTRA_ANDROID_PAY_ENVIRONMENT, environment);
            return this;
        }

        UnityAndroidPayFragmentBuilder setPublicKey(String key) {
            bundle.putString(EXTRA_PUBLIC_KEY, key);
            return this;
        }

        UnityAndroidPayFragmentBuilder setSessionCallbacks(AndroidPaySessionCallback callbacks) {
            this.callbacks = callbacks;
            return this;
        }

        UnityAndroidPayFragment build() {
            checkBundleContainsRequiredExtras();
            UnityAndroidPayFragment fragment = new UnityAndroidPayFragment();
            fragment.setArguments(bundle);
            fragment.setSessionCallbacks(callbacks);
            return fragment;
        }

        private void checkBundleContainsRequiredExtras() {
            List<String> missingExtras = new ArrayList<>();
            for (String extra : requiredExtras) {
                if (bundle.get(extra) == null) {
                    missingExtras.add(extra);
                }
            }

            if (missingExtras.size() > 0) {
                throw new IllegalArgumentException("Missing required bundle extras: " + missingExtras.toString());
            }
        }
    }

    public static UnityAndroidPayFragmentBuilder builder() {
        return new UnityAndroidPayFragmentBuilder();
    }

    @Override
    public void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Bundle bundle = getArguments();
        if (bundle != null) {
            cart = bundle.getParcelable(EXTRA_PAY_CART);
            countryCode = bundle.getString(EXTRA_COUNTRY_CODE);
            androidPayEnvironment = bundle.getInt(EXTRA_ANDROID_PAY_ENVIRONMENT,
                    WalletConstants.ENVIRONMENT_SANDBOX);
            publicKey = bundle.getString(EXTRA_PUBLIC_KEY);
        }

        // Don't recreate this fragment during configuration change.
        setRetainInstance(true);

        currentCheckoutState = CheckoutState.READY;
        googleApiClient = new GoogleApiClient.Builder(this.getActivity())
            .addApi(Wallet.API, new Wallet.WalletOptions.Builder()
                    .setEnvironment(androidPayEnvironment)
                    .setTheme(WalletConstants.THEME_DARK)
                    .build())
            .addConnectionCallbacks(this)
            .build();
    }


    @Override
    public void onStart() {
        super.onStart();
        googleApiClient.connect();
    }

    @Override
    public void onStop() {
        googleApiClient.disconnect();
        super.onStop();
    }

    @Override
    public void onConnected(@Nullable Bundle bundle) {
        if (currentCheckoutState == CheckoutState.READY) {
            Logger.d("Google API Client connected");
            currentCheckoutState = CheckoutState.REQUESTING_MASKED_WALLET;
            PayHelper.requestMaskedWallet(googleApiClient, cart, publicKey);
        }
    }

    @Override
    public void onConnectionSuspended(int i) { }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        PayHelper.handleWalletResponse(requestCode, resultCode, data, new PayHelper.WalletResponseHandler() {

            @Override
            public void onMaskedWallet(final MaskedWallet maskedWallet) {
                super.onMaskedWallet(maskedWallet);
                UnityAndroidPayFragment.this.maskedWallet = maskedWallet;
                currentCheckoutState = CheckoutState.RECEIVED_MASKED_WALLET;

                MailingAddressInput input = new MailingAddressInput(maskedWallet.getBuyerShippingAddress());

                Logger.d("Masked wallet received");

                if (sessionCallbacks != null) {
                    sessionCallbacks.onUpdateShippingAddress(input, new MessageCenter.MessageCallback() {
                        @Override
                        public void onResponse(String jsonResponse) {
                            UnityAndroidPayFragment.this.onUpdateShippingAddress(jsonResponse);
                        }
                    });
                }
            }

            @Override
            public void onFullWallet(FullWallet fullWallet) {
                super.onFullWallet(fullWallet);
                Logger.d("Full wallet received.");
            }

            @Override
            public void onWalletError(int requestCode, int errorCode) {
                final String msg = "Wallet error: " + WalletErrorFormatter.errorStringFromCode(errorCode);
                Logger.d(msg);
                if (sessionCallbacks != null) {
                    sessionCallbacks.onError(WalletErrorFormatter.errorStringFromCode(errorCode));
                }
            }

            @Override
            public void onWalletRequestCancel(int requestCode) {
                super.onWalletRequestCancel(requestCode);
                Logger.d("Wallet canceled");
                if (sessionCallbacks != null) {
                    sessionCallbacks.onCancel();
                }
            }
        });
    }

    /**
     * Unity callback method that runs whenever the shipping address is updated on the Unity side.
     *
     * @param jsonResponse the {@link AndroidPayEventResponse} represented as a JSON string
     */
    private void onUpdateShippingAddress(String jsonResponse) {
        // TODO: Create a new pay cart with the updated shipping address and request full wallet
        Logger.d("New cart data from Unity: " + jsonResponse);
        try {
            cart = payCartFromEventResponse(AndroidPayEventResponse.fromJsonString(jsonResponse));
            requestFullWallet(cart);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    /**
     * Creates a new {@link PayCart} populated with data from {@link AndroidPayEventResponse}.
     *
     * @param response the {@code AndroidPayEventResponse} with data to populate the cart
     * @return a new {@code PayCart} built based on the {@code response} argument
     */
    private PayCart payCartFromEventResponse(AndroidPayEventResponse response) {
        final PricingLineItems items = response.pricingLineItems;
        return PayCart.builder()
                .merchantName(response.merchantName)
                .currencyCode(response.currencyCode)
                .shippingAddressRequired(response.requiresShipping)
                .subtotal(items.subtotal)
                .shippingPrice(items.shippingPrice)
                .taxPrice(items.taxPrice)
                .totalPrice(items.totalPrice)
                .build();
    }

    /**
     * Requests the full wallet to Google Pay API.
     *
     * @param cart the {@link PayCart} to request a full wallet for
     */
    private void requestFullWallet(PayCart cart) {
        PayHelper.requestFullWallet(googleApiClient, cart, maskedWallet);
    }

    public void setSessionCallbacks(AndroidPaySessionCallback callbacks) {
        sessionCallbacks = callbacks;
    }

    protected void setGoogleClient(GoogleApiClient client) {
        googleApiClient = client;
    }
}
