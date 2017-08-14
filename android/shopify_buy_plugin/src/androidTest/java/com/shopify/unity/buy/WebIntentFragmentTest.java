package com.shopify.unity.buy;

import android.support.test.filters.MediumTest;
import android.support.test.rule.ActivityTestRule;
import android.support.test.runner.AndroidJUnit4;

import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;

import static junit.framework.Assert.assertFalse;
import static junit.framework.Assert.assertTrue;

@MediumTest
@RunWith(AndroidJUnit4.class)
public class WebIntentFragmentTest implements WebIntentListener {

    @Rule
    public ActivityTestRule<MockUnityActivity> rule =
        new ActivityTestRule<>(MockUnityActivity.class);

    private boolean hasClosed = false;

    @Test
    public void testListener() {
        WebIntentFragment webFragment = WebIntentFragment.newInstance("http://shopify.com");
        webFragment.setListener(this);
        rule.getActivity().getFragmentManager().beginTransaction().add(webFragment, "").commit();
        assertFalse(hasClosed);

        webFragment.onPause();
        webFragment.onResume();
        assertTrue(hasClosed);
    }

    @Override
    public void onWebIntentClose() {
        hasClosed = true;
    }
}
