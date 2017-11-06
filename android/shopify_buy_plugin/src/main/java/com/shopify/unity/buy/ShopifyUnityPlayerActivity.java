package com.shopify.unity.buy;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import com.google.android.gms.wallet.MaskedWallet;
import com.shopify.buy3.pay.PayCart;
import com.shopify.unity.buy.androidpay.AndroidPayCheckout;
import com.shopify.unity.buy.androidpay.view.viewmodel.ConfirmationViewModel;
import com.shopify.unity.buy.androidpay.view.widget.ConfirmationView;
import com.shopify.unity.buy.androidpay.view.widget.ShippingMethodSelectDialog;
import com.shopify.unity.buy.androidpay.view.widget.WalletFragmentInstaller;
import com.shopify.unity.buy.models.CheckoutInfo;
import com.shopify.unity.buy.models.ShippingMethod;
import com.shopify.unity.buy.models.ShopifyError;
import com.unity3d.player.UnityPlayerActivity;

/**
 * {@link Activity} implementation that serves as the main mediator
 * between {@link AndroidPayCheckout} and {@code Activity} APIs,
 * such as {@link Activity#onActivityResult(int, int, Intent)}.
 */
public class ShopifyUnityPlayerActivity extends UnityPlayerActivity
        implements AndroidPayCheckout.Listener, ConfirmationView.Listener {

    /** Objects that hold checkout logic and forwards checkout changes to Unity. */
    @Nullable private AndroidPayCheckout checkout;
    /** Confirmation view shown over the game graphics to allow the user to review the checkout */
    @Nullable private ConfirmationView confirmationView;
    /** View container that will host the {@link #confirmationView} when it's shown. */
    private ViewGroup root;

    @Override
    protected void onCreate(Bundle bundle) {
        super.onCreate(bundle);
        final ViewGroup contentView = findViewById(android.R.id.content);
        LayoutInflater
                .from(this)
                .inflate(R.layout.shopify_unity_player_activity_overlay, contentView);
        root = findViewById(R.id.root);
    }

    @Override
    public void onStart() {
        super.onStart();
        if (checkout != null) {
            checkout.resume();
        }
    }

    @Override
    public void onStop() {
        if (checkout != null) {
            checkout.suspend();
        }
        super.onStop();
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (checkout != null) {
            checkout.handleWalletResponse(requestCode, resultCode, data);
        }
    }

    /**
     * Starts the checkout flow.
     *
     * @param cart the checkout cart
     * @param publicKey the Base64-encoded public key used by Android Pay
     *         to encrypt checkout information
     */
    public void startAndroidPayCheckout(@NonNull AndroidPayCheckout checkout,
                                        @NonNull PayCart cart,
                                        @NonNull String publicKey) {
        this.checkout = checkout;
        checkout.startCheckout(cart, publicKey, this);
    }

    @Override
    public void onConnectionLost() {
        closeConfirmationScreen();
    }

    @Override
    public void onUpdateShippingAddressFail(@NonNull ShopifyError error) {
        if (checkout != null) {
            updateView(checkout.getCheckoutInfo(), true);
        }
        closeConfirmationScreen(); // Can't revert address on screen, payment flow reset required
        Toast.makeText(this, R.string.confirmation_address_error, Toast.LENGTH_SHORT).show();
    }

    @Override
    public void onUpdateShippingMethodFail(@NonNull ShopifyError error) {
        if (checkout != null) {
            updateView(checkout.getCheckoutInfo(), true);
        }
        Toast.makeText(this, R.string.confirmation_method_error, Toast.LENGTH_SHORT).show();
    }

    @Override
    public void onSynchronizeCheckoutInfo(@NonNull CheckoutInfo checkoutInfo) {
        if (confirmationView == null) {
            final View v = LayoutInflater.from(this).inflate(R.layout.view_confirmation, root);
            confirmationView = v.findViewById(R.id.confirmation);
            confirmationView.setListener(this);
        }
        updateView(checkoutInfo, true);
    }

    /**
     * Creates a new view model and updates the confirmation view.
     *
     * @param checkoutInfo the checkout state metadata to extract user-facing information from
     * @param buttonEnabled determines if the CONFIRM button must be shown as enabled or not
     */
    private void updateView(@NonNull CheckoutInfo checkoutInfo, boolean buttonEnabled) {
        if (confirmationView != null) {
            final ConfirmationViewModel viewModel = ConfirmationViewModel.of(
                    checkoutInfo, buttonEnabled
            );
            if (viewModel != null) {
                confirmationView.update(newWalletFragmentInstaller(), viewModel);
            }
        }
    }

    /**
     * Shows the shipping methods list dialog.
     */
    private void showShippingDialog() {
        if (checkout == null) {
            return;
        }
        final ShippingMethodSelectDialog.OnShippingMethodSelectListener listener =
                new ShippingMethodSelectDialog.OnShippingMethodSelectListener() {
            @Override
            public void onShippingMethodSelected(ShippingMethod shippingMethod, int position) {
                checkout.updateShippingMethod(shippingMethod);
                updateView(checkout.getCheckoutInfo(), false);
            }
        };
        new ShippingMethodSelectDialog(this).show(
                checkout.getCheckoutInfo().getShippingMethods(),
                listener
        );
    }

    /**
     * Creates a {@link WalletFragmentInstaller} to be provided
     * to the {@link ConfirmationView}.
     *
     * @return the created {@code WalletFragmentInstaller}
     */
    private WalletFragmentInstaller newWalletFragmentInstaller() {
        @SuppressWarnings("ConstantConditions")
        final MaskedWallet maskedWallet = checkout.getMaskedWallet();
        return new WalletFragmentInstaller(getFragmentManager(), maskedWallet);
    }

    @Override
    public void onShippingRateChangeClick() {
        showShippingDialog();
    }

    @Override
    public void onCloseButtonClick() {
        closeConfirmationScreen();
    }

    @Override
    public void onConfirmOrderClick() {
        if (checkout != null) {
            checkout.confirmCheckout();
        }
        closeConfirmationScreen();
    }

    /**
     * Dismisses the confirmation screen.
     */
    private void closeConfirmationScreen() {
        root.removeView(confirmationView);
        confirmationView = null;
    }
}
