package com.shopify.unity.buy.androidpay;

import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.annotation.VisibleForTesting;

import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.identity.intents.model.UserAddress;
import com.google.android.gms.wallet.FullWallet;
import com.google.android.gms.wallet.MaskedWallet;
import com.shopify.buy3.pay.CardNetworkType;
import com.shopify.buy3.pay.PayCart;
import com.shopify.buy3.pay.PayHelper;
import com.shopify.buy3.pay.PaymentToken;
import com.shopify.unity.buy.MessageCenter;
import com.shopify.unity.buy.UnityMessage;
import com.shopify.unity.buy.models.AndroidPayEventResponse;
import com.shopify.unity.buy.models.CheckoutInfo;
import com.shopify.unity.buy.models.MailingAddressInput;
import com.shopify.unity.buy.models.NativePayment;
import com.shopify.unity.buy.models.PricingLineItems;
import com.shopify.unity.buy.models.ShippingContact;
import com.shopify.unity.buy.models.ShippingMethod;
import com.shopify.unity.buy.models.ShopifyError;
import com.shopify.unity.buy.models.TokenData;
import com.shopify.unity.buy.utils.Logger;
import com.shopify.unity.buy.utils.WalletErrorFormatter;

import org.json.JSONException;

import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Queue;
import java.util.Set;

import static com.shopify.buy3.pay.PayHelper.AndroidPayReadyCallback;
import static com.shopify.buy3.pay.PayHelper.WalletResponseHandler;
import static com.shopify.buy3.pay.PayHelper.extractPaymentToken;
import static com.shopify.buy3.pay.PayHelper.requestMaskedWallet;
import static com.shopify.unity.buy.MessageCenter.MessageCallback;
import static com.shopify.unity.buy.MessageCenter.Method;

/**
 * @author Flavio Faria
 */

public final class AndroidPayCheckout implements GoogleApiClient.ConnectionCallbacks {

    @VisibleForTesting enum CheckoutState {
        READY,
        REQUESTING_MASKED_WALLET,
        RECEIVED_MASKED_WALLET,
        UPDATED_SHIPPING_ADDRESS,
    }

    // Android Pay stuff
    @NonNull private final GoogleApiClient googleApiClient;
    @Nullable private String publicKey;
    @Nullable private MaskedWallet maskedWallet;
    private Queue<Runnable> connectedTasksQueue = new LinkedList<>();

    // Unity stuff
    @NonNull private final MessageCenter messageCenter;

    // Checkout stuff
    @NonNull private CheckoutState currentCheckoutState = CheckoutState.READY;
    @NonNull private CheckoutInfo checkoutInfo = CheckoutInfo.fresh();

    @Nullable private Listener listener;

    public AndroidPayCheckout(@NonNull GoogleApiClientFactory googleApiClientFactory,
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

    public void isReadyToPay(@NonNull final List<CardNetworkType> supportedCardNetworks) {
        googleApiClient.connect();
        connectedTasksQueue.offer(new Runnable() {
            @Override public void run() {
                isReadyToPayInternal(supportedCardNetworks);
            }
        });
    }

    private void isReadyToPayInternal(@NonNull List<CardNetworkType> supportedCardNetworks) {
        final Set<CardNetworkType> cardNetworkTypes = new HashSet<>(supportedCardNetworks);
        PayHelper.isReadyToPay(googleApiClient.getContext(), googleApiClient, cardNetworkTypes,
                new AndroidPayReadyCallback() {
                    @Override public void onResult(boolean result) {
                        final UnityMessage msg = UnityMessage.fromAndroid(String.valueOf(result));
                        messageCenter.sendMessageTo(Method.ON_CAN_CHECKOUT_WITH_AP_RESULT, msg);
                        if (currentCheckoutState == CheckoutState.READY) {
                            // This connection has been started solely to check if it's ready to pay
                            googleApiClient.disconnect();
                        }
                    }
                });
    }

    public void startCheckout(@NonNull PayCart cart, @NonNull String publicKey,
                              @NonNull Listener listener) {
        reset();
        this.listener = listener;
        googleApiClient.registerConnectionCallbacks(this);
        checkoutInfo = CheckoutInfo.from(checkoutInfo).payCart(cart).build();
        this.publicKey = publicKey;
        googleApiClient.connect();
    }

    public void updateShippingMethod(@NonNull ShippingMethod shippingMethod) {
        final ShippingMethod oldShippingMethod = checkoutInfo.getCurrentShippingMethod();
        checkoutInfo = CheckoutInfo
                .from(checkoutInfo)
                .currentShippingMethod(shippingMethod)
                .build();
        final String jsonStr = shippingMethod.toJson().toString();
        final UnityMessage msg = UnityMessage.fromAndroid(jsonStr);
        messageCenter.sendMessageTo(Method.ON_UPDATE_SHIPPING_LINE, msg, new MessageCallback() {
            @Override public void onResponse(String jsonResponse) {
                AndroidPayCheckout.this.onResponseReceived(jsonResponse);
            }
            @Override public void onError(ShopifyError error) {
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

    public void confirmCheckout() {
        Logger.debug("Confirming checkout...");
        requestFullWallet(checkoutInfo.getPayCart());
    }

    public void resume() {
        googleApiClient.connect();
    }

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
                AndroidPayCheckout.this.onWalletError(requestCode, errorCode);
            }
            @Override public void onWalletRequestCancel(int requestCode) {
                super.onWalletRequestCancel(requestCode);
                AndroidPayCheckout.this.onWalletRequestCancel(requestCode);
            }
        });
    }

    private void onMaskedWallet(final MaskedWallet maskedWallet) {
        if (currentCheckoutState == CheckoutState.REQUESTING_MASKED_WALLET) {
            currentCheckoutState = CheckoutState.RECEIVED_MASKED_WALLET;
        }
        Logger.debug("Masked wallet received");
        this.maskedWallet = maskedWallet;
        updateShippingAddress(maskedWallet, new MessageCallback() {
            @Override public void onResponse(String jsonResponse) {
                AndroidPayCheckout.this.onResponseReceived(jsonResponse);
                currentCheckoutState = CheckoutState.UPDATED_SHIPPING_ADDRESS;
            }
            @Override public void onError(ShopifyError error) {
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
        final UnityMessage msg = UnityMessage.fromAndroid(paymentInput.toJson().toString());
        messageCenter.sendMessageTo(Method.ON_CONFIRM_CHECKOUT, msg);
        reset();
    }

    private void onWalletError(int requestCode, int errorCode) {
        Logger.debug("Wallet error: " + WalletErrorFormatter.errorStringFromCode(errorCode));
        final String errorString = WalletErrorFormatter.errorStringFromCode(errorCode);
        final UnityMessage message = UnityMessage.fromAndroid(errorString);
        messageCenter.sendMessageTo(Method.ON_ERROR, message);
    }

    private void onWalletRequestCancel(int requestCode) {
        Logger.debug("Wallet canceled");
        final UnityMessage message = UnityMessage.fromAndroid("");
        messageCenter.sendMessageTo(Method.ON_CANCEL, message);
    }

    @Nullable
    public MaskedWallet getMaskedWallet() {
        return maskedWallet;
    }

    private void updateShippingAddress(@NonNull MaskedWallet maskedWallet,
                                       @Nullable MessageCallback callback) {
        final UserAddress address = maskedWallet.getBuyerShippingAddress();
        final MailingAddressInput input = new MailingAddressInput(address);
        final UnityMessage msg = UnityMessage.fromAndroid(input.toJson().toString());
        messageCenter.sendMessageTo(Method.ON_UPDATE_SHIPPING_ADDRESS, msg, callback);
    }

    @NonNull
    public CheckoutInfo getCheckoutInfo() {
        return checkoutInfo;
    }

    /**
     * Unity callback method that runs whenever the checkout information
     * is updated on the Unity side.
     *
     * @param jsonResponse the {@link AndroidPayEventResponse} represented as a JSON string
     */
    private void onResponseReceived(String jsonResponse) {
        Logger.debug("New payCart data from Unity: " + jsonResponse);
        try {
            final AndroidPayEventResponse response =
                    AndroidPayEventResponse.fromJsonString(jsonResponse);

            checkoutInfo = CheckoutInfo.from(checkoutInfo)
                                       .payCart(payCartFromEventResponse(response))
                                       .shippingMethods(response.shippingMethods)
                                       .build();

            if (listener != null) {
                listener.onSynchronizeCheckoutInfo(checkoutInfo);
            }
        } catch (JSONException e) {
            e.printStackTrace();
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

    public interface Listener {
        void onConnectionLost();
        void onUpdateShippingAddressFail(@NonNull ShopifyError error);
        void onUpdateShippingMethodFail(@NonNull ShopifyError error);
        void onSynchronizeCheckoutInfo(@NonNull CheckoutInfo checkoutInfo);
    }
}
