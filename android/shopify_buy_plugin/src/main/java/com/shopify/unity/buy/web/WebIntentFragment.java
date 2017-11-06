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

import android.app.Fragment;
import android.net.Uri;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.customtabs.CustomTabsIntent;

public final class WebIntentFragment extends Fragment {

    private static final String ARGS_URL = "url";
    private boolean onPauseIssued = false;
    private WebIntentListener listener;

    public static WebIntentFragment newInstance(String url) {
        Bundle args = new Bundle();
        args.putString(ARGS_URL, url);

        WebIntentFragment fragment = new WebIntentFragment();
        fragment.setArguments(args);
        return fragment;
    }

    @Override
    public void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        String url = getArguments().getString(ARGS_URL);
        if (url != null) {
            CustomTabsIntent.Builder builder = new CustomTabsIntent.Builder();
            CustomTabsIntent customTabsIntent = builder.build();
            customTabsIntent.launchUrl(getActivity(), Uri.parse(url));
        }
    }

    @Override
    public void onPause() {
        super.onPause();
        onPauseIssued = true;
    }

    @Override
    public void onResume() {
        super.onResume();
        if (onPauseIssued) {
            notifyListenerOnClose();
        }
        onPauseIssued = false;
    }

    public void setListener(WebIntentListener listener) {
        this.listener = listener;
    }

    private void notifyListenerOnClose() {
        if (listener != null) {
            listener.onWebIntentClose();
        }
    }
}

