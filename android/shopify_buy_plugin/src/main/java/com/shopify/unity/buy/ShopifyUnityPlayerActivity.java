package com.shopify.unity.buy;

import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.google.android.gms.wallet.MaskedWallet;
import com.shopify.buy3.pay.PayCart;
import com.shopify.unity.buy.androidpay.AndroidPayCheckout;
import com.shopify.unity.buy.androidpay.GoogleApiClientFactory;
import com.shopify.unity.buy.androidpay.view.viewmodel.ConfirmationViewModel;
import com.shopify.unity.buy.androidpay.view.widget.ConfirmationView;
import com.shopify.unity.buy.androidpay.view.widget.ShippingMethodSelectDialog;
import com.shopify.unity.buy.androidpay.view.widget.WalletFragmentInstaller;
import com.shopify.unity.buy.models.CheckoutInfo;
import com.shopify.unity.buy.models.ShippingMethod;
import com.unity3d.player.UnityPlayerActivity;

import static com.shopify.unity.buy.androidpay.view.widget.ShippingMethodSelectDialog.OnShippingMethodSelectListener;

public class ShopifyUnityPlayerActivity extends UnityPlayerActivity
        implements AndroidPayCheckout.Listener {

    @Nullable private AndroidPayCheckout checkout;
    private ConfirmationView confirmationView;
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

    public void startAndroidPayCheckout(@NonNull PayCart cart, @NonNull String publicKey,
                                        boolean testing) {
        if (checkout == null) {
            final GoogleApiClientFactory factory = GoogleApiClientFactory.of(this, testing);
            checkout = new AndroidPayCheckout(factory, new MessageCenter(), this);
        }
        checkout.startCheckout(cart, publicKey);
    }

    @Override
    public void onUpdateShippingAddress(@NonNull CheckoutInfo checkoutInfo) {
        updateView(checkoutInfo, false);
    }

    @Override
    public void onSynchronizeShippingAddress(@NonNull CheckoutInfo checkoutInfo) {
        if (confirmationView == null) {
            final View v = LayoutInflater.from(this).inflate(R.layout.view_confirmation, root);
            confirmationView = v.findViewById(R.id.confirmation);
            //noinspection ConstantConditions
            confirmationView.setListener(new ConfirmationView.Listener() {
                @Override public void onClose() {
                    root.removeView(confirmationView);
                    confirmationView = null;
                }
                @Override public void onShippingRateChangeClick() {
                    showShippingDialog();
                }
            });
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
        final OnShippingMethodSelectListener listener = new OnShippingMethodSelectListener() {
            @Override
            public void onShippingMethodSelected(ShippingMethod shippingMethod, int position) {
                updateView(checkout.getCheckoutInfo(), false);
                checkout.updateShippingMethod(shippingMethod);
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
}
