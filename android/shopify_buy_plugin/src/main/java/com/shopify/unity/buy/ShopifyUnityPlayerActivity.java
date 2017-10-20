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
import com.shopify.unity.buy.androidpay.view.widget.WalletFragmentInstaller;
import com.shopify.unity.buy.models.ShippingMethod;
import com.unity3d.player.UnityPlayerActivity;

import java.util.List;

public class ShopifyUnityPlayerActivity extends UnityPlayerActivity
        implements AndroidPayCheckout.Listener {

    @Nullable private AndroidPayCheckout androidPayCheckout;
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
        if (androidPayCheckout != null) {
            androidPayCheckout.resume();
        }
    }

    @Override
    public void onStop() {
        if (androidPayCheckout != null) {
            androidPayCheckout.suspend();
        }
        super.onStop();
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (androidPayCheckout != null) {
            androidPayCheckout.handleWalletResponse(requestCode, resultCode, data);
        }
    }

    public void startAndroidPayCheckout(@NonNull PayCart cart, @NonNull String publicKey,
                                        boolean testing) {
        if (androidPayCheckout == null) {
            final GoogleApiClientFactory factory = GoogleApiClientFactory.of(this, testing);
            androidPayCheckout = new AndroidPayCheckout(factory, new MessageCenter(), this);
        }
        androidPayCheckout.startCheckout(cart, publicKey);
    }

    @Override
    public void onUpdateShippingAddress(@NonNull PayCart payCart,
                                        @NonNull List<ShippingMethod> shippingMethods) {
        if (confirmationView != null) {
            confirmationView.update(
                    newWalletFragmentInstaller(),
                    ConfirmationViewModel.of(payCart, shippingMethods, false)
            );
        }
    }

    @Override
    public void onSynchronizeShippingAddress(@NonNull PayCart payCart,
                                             @NonNull List<ShippingMethod> shippingMethods) {
        if (confirmationView == null) {
            final View v = LayoutInflater.from(this).inflate(R.layout.view_confirmation, root);
            confirmationView = v.findViewById(R.id.confirmation);
            //noinspection ConstantConditions
            confirmationView.setListener(new ConfirmationView.Listener() {
                @Override public void onClose() {
                    root.removeView(confirmationView);
                    confirmationView = null;
                }
            });
        }
        confirmationView.update(
                newWalletFragmentInstaller(),
                ConfirmationViewModel.of(payCart, shippingMethods, true)
        );
    }

    private WalletFragmentInstaller newWalletFragmentInstaller() {
        @SuppressWarnings("ConstantConditions")
        final MaskedWallet maskedWallet = androidPayCheckout.getMaskedWallet();
        return new WalletFragmentInstaller(getFragmentManager(), maskedWallet);
    }
}
