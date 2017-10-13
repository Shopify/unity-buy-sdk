package com.shopify.unity.buy.web;

import android.app.FragmentManager;
import android.app.FragmentTransaction;

import com.shopify.unity.buy.MessageCenter;
import com.shopify.unity.buy.UnityMessage;
import com.unity3d.player.UnityPlayer;

public class WebCheckoutSession implements WebIntentListener {

    private static final String WEB_FRAGMENT_TAG = "webFragment";
    private static final String NATIVE_WEB_DELEGATE_METHOD_CONTENT = "dismissed";
    private String unityDelegateObjectName;

    public void checkout(String unityDelegateObjectName, String url) {
        this.unityDelegateObjectName = unityDelegateObjectName;
        launchWebIntent(url);
    }

    @Override
    public void onWebIntentClose() {
        FragmentManager manager = getFragmentManager();
        WebIntentFragment fragment = (WebIntentFragment) manager.findFragmentByTag(WEB_FRAGMENT_TAG);
        if (fragment != null) {
            fragment.setListener(null);
            manager.beginTransaction().remove(fragment).commit();
        }

        UnityMessage dismissedMessage = UnityMessage.fromAndroid(NATIVE_WEB_DELEGATE_METHOD_CONTENT);
        MessageCenter.UnityMessageReceiver receiver =
            new MessageCenter.UnityMessageReceiver(unityDelegateObjectName,
                MessageCenter.Method.ON_NATIVE_MESSAGE);
        MessageCenter.sendMessageTo(dismissedMessage, receiver);
    }

    private FragmentManager getFragmentManager() {
        return UnityPlayer.currentActivity.getFragmentManager();
    }

    private void launchWebIntent(final String checkoutUrl) {
        WebIntentFragment fragment = WebIntentFragment.newInstance(checkoutUrl);
        fragment.setListener(this);

        FragmentTransaction transaction = getFragmentManager().beginTransaction();
        transaction.add(fragment, WEB_FRAGMENT_TAG);
        transaction.commit();
    }
}
