package com.shopify.unity.buy;

import android.app.Fragment;

import com.google.android.gms.wallet.WalletConstants;
import com.shopify.buy3.pay.PayCart;
import com.shopify.buy3.pay.PayHelper;
import com.shopify.unity.buy.models.MailingAddressInput;
import com.shopify.unity.buy.models.PricingLineItems;
import com.shopify.unity.buy.utils.AndroidLogger;
import com.shopify.unity.buy.utils.ILogger;
import com.unity3d.player.UnityPlayer;

import java.io.IOException;

public final class AndroidPayCheckoutSession implements AndroidPaySessionCallbacks {
    private static final String PAY_FRAGMENT_TAG = "payFragment";

    private ILogger logger;
    private String unityDelegateObjectName;

    public AndroidPayCheckoutSession() {
        this.logger = new AndroidLogger();
    }

    AndroidPayCheckoutSession(ILogger logger) {
        this.logger = logger;
    }

    //CHECKSTYLE:OFF
    public boolean checkoutWithAndroidPay(
            String unityDelegateObjectName,
            String merchantName,
            String publicKey,
            String pricingLineItemsString,
            String currencyCode,
            String countryCode,
            boolean requiresShipping,
            boolean testing
    ) {
        //CHECKSTYLE:ON
        if (!PayHelper.isAndroidPayEnabledInManifest(UnityPlayer.currentActivity)) {
            // TODO: Send unsupported error to Unity
            return false;
        }

        try {
            PayCart cart = cartFromUnity(merchantName, pricingLineItemsString, currencyCode, countryCode,
                    requiresShipping);

            this.unityDelegateObjectName = unityDelegateObjectName;

            addPayFragment(cart, countryCode, publicKey, testing);

            return true;
        } catch (IOException e) {
            logger.error("ShopifyBuyPlugin", "Failed to parse summary items from Unity!");
            return false;
        }
    }

    PayCart cartFromUnity(
        String merchantName,
        String pricingLineItemsString,
        String currencyCode,

        // TODO: Pull in updated buy3 SDK which will use the countryCode.
        String countryCode,
        boolean requiresShipping
    ) throws IOException {
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
                WalletConstants.ENVIRONMENT_TEST : WalletConstants.ENVIRONMENT_PRODUCTION)
            .setPublicKey(publicKey)
            .setSessionCallbacks(this)
            .build();

        UnityPlayer.currentActivity.getFragmentManager()
            .beginTransaction()
            .add(payFragment, PAY_FRAGMENT_TAG)
            .commit();
    }

    private void removePayFragment() {
        Fragment fragment = UnityPlayer.currentActivity.getFragmentManager().findFragmentByTag(PAY_FRAGMENT_TAG);
        if (fragment == null)  {
            return;
        }

        UnityPlayer.currentActivity.getFragmentManager()
            .beginTransaction()
            .remove(fragment)
            .commit();
    }

    public void onUpdateShippingAddress(MailingAddressInput address, MessageCenter.MessageCallbacks messageCallbacks) {
        UnityMessage msg = UnityMessage.fromAndroid(address.toJsonString());
        MessageCenter.UnityMessageReceiver receiver = new MessageCenter.UnityMessageReceiver(
            unityDelegateObjectName,
            MessageCenter.Method.ON_UPDATE_SHIPPING_ADDRESS
        );
        MessageCenter.sendMessageTo(msg, receiver, messageCallbacks);
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
