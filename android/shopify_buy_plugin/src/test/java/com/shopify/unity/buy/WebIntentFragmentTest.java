package com.shopify.unity.buy;

import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.android.controller.FragmentController;
import org.robolectric.annotation.Config;


import buy.unity.shopfiy.com.unitybuyplugin.BuildConfig;

import static junit.framework.Assert.assertFalse;
import static junit.framework.Assert.assertTrue;


@RunWith(RobolectricTestRunner.class)
@Config(constants = BuildConfig.class)
public class WebIntentFragmentTest implements WebIntentListener {

    private boolean hasClosed = false;

    @Override
    public void onWebIntentClose() {
        hasClosed = true;
    }

    @Test
    public void testListener() {
        WebIntentFragment webFragment = WebIntentFragment.newInstance("com.test.url");
        webFragment.setListener(this);

        FragmentController controller = FragmentController.of(webFragment).create(null).resume();
        assertFalse(hasClosed);
        controller.pause().resume();
        assertTrue(hasClosed);
    }
}
