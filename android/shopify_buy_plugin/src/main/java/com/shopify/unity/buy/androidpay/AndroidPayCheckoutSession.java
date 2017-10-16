package com.shopify.unity.buy.androidpay;

import android.app.Activity;
import android.app.Fragment;
import android.app.FragmentManager;
import android.support.annotation.VisibleForTesting;

import com.google.android.gms.wallet.WalletConstants;
import com.shopify.buy3.pay.PayCart;
import com.shopify.buy3.pay.PayHelper;
import com.shopify.unity.buy.MessageCenter;
import com.shopify.unity.buy.UnityMessage;
import com.shopify.unity.buy.models.MailingAddressInput;
import com.shopify.unity.buy.models.PricingLineItems;
import com.shopify.unity.buy.utils.Logger;
import com.unity3d.player.UnityPlayer;

import org.json.JSONException;

public final class AndroidPayCheckoutSession implements AndroidPaySessionCallback {
    public static final String PAY_FRAGMENT_TAG = "payFragment";

    private final Activity rootActivity;
    private final boolean testing;
    private String unityDelegateObjectName;

    public AndroidPayCheckoutSession(boolean testing) {
        this(UnityPlayer.currentActivity, testing);
    }

    @VisibleForTesting
    AndroidPayCheckoutSession(Activity rootActivity, boolean testing) {
        this.rootActivity = rootActivity;
        this.testing = testing;
        Logger.setEnabled(testing);
    }

    //CHECKSTYLE:OFF
    public boolean checkoutWithAndroidPay(
            String unityDelegateObjectName,
            String merchantName,
            String publicKey,
            String pricingLineItemsString,
            String currencyCode,
            String countryCode,
            boolean requiresShipping
    ) {
        //CHECKSTYLE:ON
        if (!PayHelper.isAndroidPayEnabledInManifest(rootActivity)) {
            // TODO: Send unsupported error to Unity
            return false;
        }

        final String msg = "unityDelegateObjectName = " + unityDelegateObjectName + "\n" +
                "merchantName = " + merchantName + "\n" +
                "publicKey = " + publicKey + "\n" +
                "pricingLineItemsString = " + pricingLineItemsString + "\n" +
                "currencyCode = " + currencyCode + "\n" +
                "countryCode = " + countryCode + "\n" +
                "requiresShipping = " + requiresShipping + "\n";
        Logger.debug(msg);

        try {
            PayCart cart = cartFromUnity(merchantName, pricingLineItemsString, currencyCode, countryCode,
                    requiresShipping);

            this.unityDelegateObjectName = unityDelegateObjectName;

            addPayFragment(cart, countryCode, publicKey, testing);

            return true;
        } catch (JSONException e) {
            Logger.error("Failed to parse summary items from Unity!");
            return false;
        }
    }

    public PayCart cartFromUnity(
        String merchantName,
        String pricingLineItemsString,
        String currencyCode,

        // TODO: Pull in updated buy3 SDK which will use the countryCode.
        String countryCode,
        boolean requiresShipping
    ) throws JSONException {
        PricingLineItems items = PricingLineItems.fromJsonString(pricingLineItemsString);

        return PayCart.builder()
                .merchantName(merchantName)
                .currencyCode(currencyCode)
                .shippingAddressRequired(requiresShipping)
                .subtotal(items.subtotal)
                .shippingPrice(items.shippingPrice)
                .taxPrice(items.taxPrice)
                .totalPrice(items.totalPrice)
                .build();
    }

    private void addPayFragment(PayCart cart, String countryCode, String publicKey, boolean testing) {
        removePayFragment();

        Fragment payFragment = UnityAndroidPayFragment.builder()
            .setPayCart(cart)
            .setCountryCode(countryCode)
            .setEnvironment(testing ?
                WalletConstants.ENVIRONMENT_SANDBOX : WalletConstants.ENVIRONMENT_PRODUCTION)
            .setPublicKey(publicKey)
            .setSessionCallbacks(this)
            .build();

        getFragmentManager()
            .beginTransaction()
            .add(payFragment, PAY_FRAGMENT_TAG)
            .commit();
    }

    private void removePayFragment() {
        Fragment fragment = getFragmentManager().findFragmentByTag(PAY_FRAGMENT_TAG);
        if (fragment == null)  {
            return;
        }

        getFragmentManager()
            .beginTransaction()
            .remove(fragment)
            .commit();
    }

    private FragmentManager getFragmentManager() {
        return rootActivity.getFragmentManager();
    }

    public void onUpdateShippingAddress(MailingAddressInput address, MessageCenter.MessageCallback messageCallback) {
        UnityMessage msg = UnityMessage.fromAndroid(address.toJsonString());
        MessageCenter.UnityMessageReceiver receiver = new MessageCenter.UnityMessageReceiver(
            unityDelegateObjectName,
            MessageCenter.Method.ON_UPDATE_SHIPPING_ADDRESS
        );
        MessageCenter.sendMessageTo(msg, receiver, messageCallback);
    }

    public void onError(String error) {
        removePayFragment();
        UnityMessage msg = UnityMessage.fromAndroid(error);
        MessageCenter.UnityMessageReceiver receiver = new MessageCenter.UnityMessageReceiver(
            unityDelegateObjectName,
            MessageCenter.Method.ON_ERROR
        );
        MessageCenter.sendMessageTo(msg, receiver);
    }

    public void onCancel() {
        removePayFragment();
        UnityMessage msg = UnityMessage.fromAndroid("");
        MessageCenter.UnityMessageReceiver receiver = new MessageCenter.UnityMessageReceiver(
            unityDelegateObjectName,
            MessageCenter.Method.ON_CANCEL
        );
        MessageCenter.sendMessageTo(msg, receiver);
    }
}
