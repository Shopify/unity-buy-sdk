package com.shopify.unity.buy;

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

public class ShopifyUnityPlayerActivity extends UnityPlayerActivity
        implements AndroidPayCheckout.Listener, ConfirmationView.Listener {

    @Nullable private AndroidPayCheckout checkout;
    @Nullable private ConfirmationView confirmationView;
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

    private void closeConfirmationScreen() {
        root.removeView(confirmationView);
        confirmationView = null;
    }
}
