package com.shopify.unity.buy;

import android.app.FragmentManager;
import android.app.FragmentTransaction;

import com.unity3d.player.UnityPlayer;

public class WebCheckoutSession implements WebIntentListener {

    private static final String WEB_FRAGMENT_TAG = "webFragment";
    private static final String NATIVE_WEB_DELEGATE_METHOD_NAME = "OnNativeMessage";
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

        UnityMessage dismissedMessage = new UnityMessage(NATIVE_WEB_DELEGATE_METHOD_CONTENT);
        UnityPlayer.UnitySendMessage(unityDelegateObjectName, NATIVE_WEB_DELEGATE_METHOD_NAME, dismissedMessage.toJsonString());
    }

    private FragmentManager getFragmentManager() {
        return UnityPlayer.currentActivity.getFragmentManager();
    }

    private void launchWebIntent(final String checkoutUrl) {
        final FragmentManager manager = getFragmentManager();
        final FragmentTransaction transaction = manager.beginTransaction();

        WebIntentFragment fragment = WebIntentFragment.newInstance(checkoutUrl);
        fragment.setListener(this);
        transaction.add(fragment, WEB_FRAGMENT_TAG);

        final Runnable launchWebView = new Runnable() {
            public void run() {
                if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.N) {
                    transaction.commitNow();
                } else {
                    transaction.commit();
                    manager.executePendingTransactions();
                }
            }
        };

        UnityPlayer.currentActivity.runOnUiThread(launchWebView);
    }
}
