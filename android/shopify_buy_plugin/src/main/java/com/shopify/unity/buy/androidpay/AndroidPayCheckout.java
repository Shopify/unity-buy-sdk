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
package com.shopify.unity.buy.androidpay;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.annotation.VisibleForTesting;

import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.identity.intents.model.UserAddress;
import com.google.android.gms.wallet.FullWallet;
import com.google.android.gms.wallet.MaskedWallet;
import com.google.android.gms.wallet.WalletConstants;
import com.shopify.buy3.pay.CardNetworkType;
import com.shopify.buy3.pay.PayCart;
import com.shopify.buy3.pay.PayHelper;
import com.shopify.buy3.pay.PaymentToken;
import com.shopify.unity.buy.MessageCenter;
import com.shopify.unity.buy.models.AndroidPayEventResponse;
import com.shopify.unity.buy.models.CheckoutInfo;
import com.shopify.unity.buy.models.MailingAddressInput;
import com.shopify.unity.buy.models.NativePayment;
import com.shopify.unity.buy.models.PricingLineItems;
import com.shopify.unity.buy.models.ShippingContact;
import com.shopify.unity.buy.models.ShippingMethod;
import com.shopify.unity.buy.models.ShopifyError;
import com.shopify.unity.buy.models.TokenData;
import com.shopify.unity.buy.models.WalletError;
import com.shopify.unity.buy.utils.Logger;

import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Queue;
import java.util.Set;

import static com.shopify.buy3.pay.PayHelper.AndroidPayReadyCallback;
import static com.shopify.buy3.pay.PayHelper.WalletResponseHandler;
import static com.shopify.buy3.pay.PayHelper.extractPaymentToken;
import static com.shopify.buy3.pay.PayHelper.requestMaskedWallet;

import com.shopify.unity.buy.MessageCallback;

/**
 * This class keeps track of the checkout state and wraps all the communication
 * between Android Pay and the platform code by exposing a simple API.
 */

public final class AndroidPayCheckout implements GoogleApiClient.ConnectionCallbacks {

    /** Enum that contains a list of possible checkout states. */
    @VisibleForTesting enum CheckoutState {
        READY,
        REQUESTING_MASKED_WALLET,
        RECEIVED_MASKED_WALLET,
        UPDATED_SHIPPING_ADDRESS,
    }

    // Android Pay stuff

    /** Client used to communicate with Android Pay services. */
    @NonNull private final GoogleApiClient googleApiClient;
    /** Base64-encoded public key used by Android Pay to encrypt checkout information. */
    @Nullable private String publicKey;
    /** Partial wallet obtained after the user confirms the payment method and shipping address. */
    @Nullable private MaskedWallet maskedWallet;
    /** List of tasks to have their executions postponed until a connection has been established
     *  with google API client. */
    private Queue<Runnable> connectedTasksQueue = new LinkedList<>();

    // Platform stuff

    /** Messaging API that allows communicating with the platform code. */
    @NonNull private final MessageCenter messageCenter;

    // Checkout stuff

    /** Current state of the checkout. */
    @NonNull private CheckoutState currentCheckoutState = CheckoutState.READY;
    /** Checkout state metadata holder. */
    @NonNull private CheckoutInfo checkoutInfo = CheckoutInfo.fresh();
    /** Propagates to the client a few state changes. */
    @Nullable private Listener listener;

    AndroidPayCheckout(@NonNull GoogleApiClientFactory googleApiClientFactory,
                       @NonNull MessageCenter messageCenter) {
        googleApiClient = googleApiClientFactory.newGoogleApiClient();
        this.messageCenter = messageCenter;
    }

    /**
     * Resets this checkout to a fresh state.
     */
    private void reset() {
        googleApiClient.unregisterConnectionCallbacks(this);
        googleApiClient.disconnect();
        publicKey = null;
        maskedWallet = null;
        currentCheckoutState = CheckoutState.READY;
        checkoutInfo = CheckoutInfo.fresh();
    }

    /**
     * Checks if Android Pay is enabled and ready to use.
     * This check will not execute immediately if the connection with google API client
     * has not been established. In this case, it will be postponed to run after
     * {@link #onConnected(Bundle)} is called.
     * The response will be delivered straight to the platform code by calling
     * {@code AndroidNativeCheckout.OnCanCheckoutWithAndroidPayResult()}.
     *
     * @param supportedCardNetworks a list of cards that the shop accepts as payment methods
     */
    public void isReadyToPay(@NonNull final List<CardNetworkType> supportedCardNetworks) {
        if (googleApiClient.isConnected()) {
            isReadyToPayInternal(supportedCardNetworks);
        } else {
            googleApiClient.connect();
            connectedTasksQueue.offer(new Runnable() {
                @Override
                public void run() {
                    isReadyToPayInternal(supportedCardNetworks);
                }
            });
        }
    }

    /**
     * Checks if Android Pay is enabled and ready to use.
     * The response will be delivered straight to the platform code by calling
     * {@code AndroidNativeCheckout.OnCanCheckoutWithAndroidPayResult()}.
     *
     * @param supportedCardNetworks a list of cards that the shop accepts as payment methods
     */
    private void isReadyToPayInternal(@NonNull List<CardNetworkType> supportedCardNetworks) {
        final Set<CardNetworkType> cardNetworkTypes = new HashSet<>(supportedCardNetworks);
        PayHelper.isReadyToPay(googleApiClient.getContext(), googleApiClient, cardNetworkTypes,
                new AndroidPayReadyCallback() {
                    @Override public void onResult(boolean result) {
                        messageCenter.onCanCheckoutWithAndroidPayResult(result);
                        if (currentCheckoutState == CheckoutState.READY) {
                            // This connection has been started solely to check if it's ready to pay
                            googleApiClient.disconnect();
                        }
                    }
                });
    }

    /**
     * Starts the checkout flow.
     *
     * @param cart the checkout cart
     * @param publicKey the Base64-encoded public key used by Android Pay
     *         to encrypt checkout information
     * @param listener to get checkout-related updates
     */
    public void startCheckout(@NonNull PayCart cart, @NonNull String publicKey,
                              @NonNull Listener listener) {
        reset();
        this.listener = listener;
        googleApiClient.registerConnectionCallbacks(this);
        checkoutInfo = CheckoutInfo.from(checkoutInfo).payCart(cart).build();
        this.publicKey = publicKey;
        googleApiClient.connect();
    }

    /**
     * Updates the shipping method on the platform code.
     *
     * @param shippingMethod the shipping method to be updated on the platform code
     */
    public void updateShippingMethod(@NonNull ShippingMethod shippingMethod) {
        final ShippingMethod oldShippingMethod = checkoutInfo.getCurrentShippingMethod();
        checkoutInfo = CheckoutInfo
                .from(checkoutInfo)
                .currentShippingMethod(shippingMethod)
                .build();

        messageCenter.onUpdateShippingLine(
                shippingMethod,
                new MessageCallback<AndroidPayEventResponse>() {
                    @Override public void onResponse(@NonNull AndroidPayEventResponse response) {
                        AndroidPayCheckout.this.onResponseReceived(response);
                    }
                    @Override public void onError(@NonNull ShopifyError error) {
                        checkoutInfo = CheckoutInfo // Reverts back to the old shipping method
                                                    .from(checkoutInfo)
                                                    .currentShippingMethod(oldShippingMethod)
                                                    .build();
                        if (listener != null) {
                            listener.onUpdateShippingMethodFail(error);
                        }
                    }
                });
    }

    /**
     * Confirms the checkout and proceeds with the payment completion.
     */
    public void confirmCheckout() {
        Logger.debug("Confirming checkout...");
        requestFullWallet(checkoutInfo.getPayCart());
    }

    /**
     * Resumes the checkout flow. Must be called when checkout screen comes to foreground.
     */
    public void resume() {
        googleApiClient.connect();
    }

    /**
     * Suspends the checkout flow. Must be called when checkout screen goes to backgroundf.
     */
    public void suspend() {
        googleApiClient.disconnect();
    }

    @Override
    public void onConnected(@Nullable Bundle bundle) {
        Runnable task;
        while ((task = connectedTasksQueue.poll()) != null) {
            task.run();
        }
        if (currentCheckoutState == CheckoutState.READY) {
            Logger.debug("Google API Client connected");
            currentCheckoutState = CheckoutState.REQUESTING_MASKED_WALLET;
            requestMaskedWallet(googleApiClient, checkoutInfo.getPayCart(), publicKey);
        }
    }

    @Override
    public void onConnectionSuspended(int cause) {
        if (cause == CAUSE_NETWORK_LOST) {
            if (listener != null) {
                listener.onConnectionLost();
            }
        }
    }

    /**
     * Handles responses from Android Pay. Calls to
     * {@link Activity#onActivityResult(int, int, Intent)}
     * must be forwarded to this method.
     *
     * @param requestCode the same {@code requestCode} delivered to
     *         {@code onActivityResult(int, int, Intent)}
     * @param resultCode the same {@code resultCode} delivered to
     *         {@code onActivityResult(int, int, Intent)}
     * @param data the same {@code data} delivered to
     *         {@code onActivityResult(int, int, Intent)}
     */
    public void handleWalletResponse(int requestCode, int resultCode, Intent data) {
        PayHelper.handleWalletResponse(requestCode, resultCode, data, new WalletResponseHandler() {
            @Override public void onMaskedWallet(final MaskedWallet maskedWallet) {
                super.onMaskedWallet(maskedWallet);
                AndroidPayCheckout.this.onMaskedWallet(maskedWallet);
            }
            @Override public void onFullWallet(FullWallet fullWallet) {
                super.onFullWallet(fullWallet);
                AndroidPayCheckout.this.onFullWallet(fullWallet);
            }
            @Override public void onWalletError(int requestCode, int errorCode) {
                AndroidPayCheckout.this.onWalletError(errorCode);
            }
            @Override public void onWalletRequestCancel(int requestCode) {
                super.onWalletRequestCancel(requestCode);
                AndroidPayCheckout.this.onWalletRequestCancel();
            }
        });
    }

    /**
     * Callback that delivers a {@link MaskedWallet} requested to Android Pay.
     *
     * @param maskedWallet the {@code MaskedWallet} with partial payment information
     */
    private void onMaskedWallet(final MaskedWallet maskedWallet) {
        if (currentCheckoutState == CheckoutState.REQUESTING_MASKED_WALLET) {
            currentCheckoutState = CheckoutState.RECEIVED_MASKED_WALLET;
        }
        Logger.debug("Masked wallet received");
        this.maskedWallet = maskedWallet;
        updateShippingAddress(maskedWallet, new MessageCallback<AndroidPayEventResponse>() {
            @Override public void onResponse(@NonNull AndroidPayEventResponse androidPayEventResponse) {
                AndroidPayCheckout.this.onResponseReceived(androidPayEventResponse);
                currentCheckoutState = CheckoutState.UPDATED_SHIPPING_ADDRESS;
            }
            @Override public void onError(@NonNull ShopifyError error) {
                if (currentCheckoutState != CheckoutState.RECEIVED_MASKED_WALLET) {
                    // User explicitly updated the shipping address
                    if (listener != null) {
                        listener.onUpdateShippingAddressFail(error);
                    }
                }
                reset();
            }
        });
    }

    /**
     * Callback that delivers a {@link FullWallet} requested to Android Pay.
     *
     * @param fullWallet the {@code FullWallet} with full payment information
     */
    @SuppressWarnings("ConstantConditions")
    private void onFullWallet(FullWallet fullWallet) {
        Logger.debug("Full wallet received");
        final PaymentToken paymentToken = extractPaymentToken(fullWallet, publicKey);
        final UserAddress shippingAddress = fullWallet.getBuyerShippingAddress();
        final String email = fullWallet.getEmail();
        final NativePayment paymentInput = NativePayment
                .newBuilder()
                .billingContact(new MailingAddressInput(fullWallet.getBuyerBillingAddress()))
                .shippingContact(new ShippingContact(shippingAddress, email))
                .identifier(paymentToken.publicKeyHash)
                .tokenData(new TokenData(paymentToken.token))
                .build();
        messageCenter.onConfirmCheckout(paymentInput);
        reset();
    }

    /**
     * Callback that delivers an error occurred during checkout.
     * @param errorCode one of the {@code ERROR}-prefixed {@link WalletConstants}
     */
    private void onWalletError(int errorCode) {
        final WalletError error = WalletError.forWalletErrorCode(errorCode);
        Logger.debug("Wallet error: " + error);
        messageCenter.onError(error);
    }

    /**
     * Callback that indicates that the user explicitly canceled the checkout flow.
     */
    private void onWalletRequestCancel() {
        Logger.debug("Wallet canceled");
        messageCenter.onCancel();
    }

    /**
     * Gets the current {@link MaskedWallet}.
     *
     * @return the current {@code MaskedWallet} or {@code null} if it has not been obtained yet
     */
    @Nullable
    public MaskedWallet getMaskedWallet() {
        return maskedWallet;
    }

    /**
     * Sends a message to the platform code in order to update the shipping address.
     *
     * @param maskedWallet a {@link MaskedWallet} instance to read the shipping address from
     * @param callback a {@link MessageCallback} to be notified when the update is completed
     */
    private void updateShippingAddress(
            @NonNull MaskedWallet maskedWallet,
            @NonNull MessageCallback<AndroidPayEventResponse> callback
    ) {
        final UserAddress address = maskedWallet.getBuyerShippingAddress();
        final MailingAddressInput input = new MailingAddressInput(address);
        messageCenter.onUpdateShippingAddress(input, callback);
    }

    /**
     * Gets the current {@link CheckoutInfo}.
     *
     * @return the current {@code CheckoutInfo} containing checkout state metadata
     */
    @NonNull
    public CheckoutInfo getCheckoutInfo() {
        return checkoutInfo;
    }

    /**
     * Platform code callback method that runs whenever the checkout information
     * is updated on the platform code side.
     *
     * @param response the {@link AndroidPayEventResponse} sent from the platform code side.
     */
    private void onResponseReceived(AndroidPayEventResponse response) {
        Logger.debug("New payCart data from platform code: " + response);
        checkoutInfo = CheckoutInfo.from(checkoutInfo)
                                   .payCart(payCartFromEventResponse(response))
                                   .shippingMethods(response.shippingMethods)
                                   .build();
        if (listener != null) {
            listener.onSynchronizeCheckoutInfo(checkoutInfo);
        }
    }

    /**
     * Creates a new {@link PayCart} populated with data from {@link AndroidPayEventResponse}.
     *
     * @param response the {@code AndroidPayEventResponse} with data to populate the payCart
     * @return a new {@code PayCart} built based on the {@code response} argument
     */
    @VisibleForTesting PayCart payCartFromEventResponse(AndroidPayEventResponse response) {
        final PricingLineItems items = response.pricingLineItems;
        return PayCart.builder()
                      .merchantName(response.merchantName)
                      .currencyCode(response.currencyCode)
                      .countryCode(response.countryCode)
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
        Logger.debug("Requesting full wallet...");
        if (maskedWallet != null) {
            PayHelper.requestFullWallet(googleApiClient, cart, maskedWallet);
        } else {
            Logger.error("Can't request full wallet before requesting masked wallet!");
        }
    }

    /**
     * Listener interface with a few checkout state change callbacks of interest.
     */
    public interface Listener {

        /**
         * Invoked whenever the connection with Google APIs is lost.
         */
        void onConnectionLost();

        /**
         * Invoked whenever the shipping address update fails.
         *
         * @param error a {@link ShopifyError} object with details about the error
         */
        void onUpdateShippingAddressFail(@NonNull ShopifyError error);

        /**
         * Invoked whenever the shipping method update fails.
         *
         * @param error a {@link ShopifyError} object with details about the error
         */
        void onUpdateShippingMethodFail(@NonNull ShopifyError error);

        /**
         * Invoked whenever the platform code responds back with up-to-date {@link CheckoutInfo}
         *
         * @param checkoutInfo up-to-date checkout state metadata
         */
        void onSynchronizeCheckoutInfo(@NonNull CheckoutInfo checkoutInfo);
    }
}
