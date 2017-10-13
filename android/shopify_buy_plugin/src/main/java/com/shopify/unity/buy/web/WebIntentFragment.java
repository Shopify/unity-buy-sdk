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

