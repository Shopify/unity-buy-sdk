package com.shopify.unity.buy;

import android.app.Fragment;
import android.net.Uri;
import android.support.customtabs.CustomTabsIntent;

public class WebIntentFragment extends Fragment {

    private boolean didShowIntent = false;
    private WebIntentListener listener;

    @Override
    public void onResume() {
        super.onResume();

        if (didShowIntent) {
            notifyListenerOnClose();
        }
    }

    public void setListener(WebIntentListener listener) {
        this.listener = listener;
    }

    public void launchUrl(String url) {
        CustomTabsIntent.Builder builder = new CustomTabsIntent.Builder();
        CustomTabsIntent customTabsIntent = builder.build();

        didShowIntent = true;
        customTabsIntent.launchUrl(getActivity().getApplicationContext(), Uri.parse(url));
    }

    private void notifyListenerOnClose() {
        didShowIntent = false;

        if (listener != null) {
            listener.onWebIntentClose();
        }
    }
}

