package com.shopify.unity.buy;

import android.app.Fragment;
import android.content.Intent;

import com.unity3d.player.UnityPlayerActivity;

public class ShopifyUnityPlayerActivity extends UnityPlayerActivity {

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        // Android Pay has an issue where the results of the wallet chooser do not get forwarded to child fragments
        // so we need to do this manually.
        Fragment androidPayFragment = getFragmentManager().findFragmentByTag(AndroidPayCheckoutSession.PAY_FRAGMENT_TAG);
        if (androidPayFragment != null) {
            androidPayFragment.onActivityResult(requestCode, resultCode, data);
        }
    }
}
