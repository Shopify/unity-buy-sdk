package com.shopify.unity.buy.web;

import android.app.FragmentManager;
import android.app.FragmentTransaction;

import com.shopify.unity.buy.MessageCenter;
import com.shopify.unity.buy.UnityMessage;
import com.unity3d.player.UnityPlayer;

public class WebCheckoutSession implements WebIntentListener {

    private static final String WEB_FRAGMENT_TAG = "webFragment";
    private static final String NATIVE_WEB_DELEGATE_METHOD_CONTENT = "dismissed";
    private static final MessageCenter MESSAGE_CENTER = new MessageCenter();

    public WebCheckoutSession(String unityDelegateObjectName) {
        MessageCenter.init(unityDelegateObjectName);
    }

    public void checkout(String url) {
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
        MESSAGE_CENTER.sendMessageTo(MessageCenter.Method.ON_NATIVE_MESSAGE, dismissedMessage);
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
