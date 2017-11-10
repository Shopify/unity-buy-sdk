/**
 * The MIT License (MIT)
 * Copyright (c) 2017 Shopify
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */
package com.shopify.unity.buy.web;

import android.app.FragmentManager;
import android.app.FragmentTransaction;

import com.shopify.unity.buy.MessageCenter;
import com.shopify.unity.buy.UnityMessageCenter;
import com.unity3d.player.UnityPlayer;

public class WebCheckoutSession implements WebIntentListener {

    private static final String WEB_FRAGMENT_TAG = "webFragment";
    private final MessageCenter messageCenter;

    public WebCheckoutSession(String unityDelegateObjectName) {
        messageCenter = new UnityMessageCenter(unityDelegateObjectName);
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
        messageCenter.onNativeMessage();
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
