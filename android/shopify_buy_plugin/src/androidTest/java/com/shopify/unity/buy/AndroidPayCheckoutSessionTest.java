package com.shopify.unity.buy;

import android.support.test.filters.MediumTest;
import android.support.test.rule.ActivityTestRule;
import android.support.test.runner.AndroidJUnit4;

import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;

import static junit.framework.Assert.assertTrue;

@MediumTest
@RunWith(AndroidJUnit4.class)
public class AndroidPayCheckoutSessionTest {
    @Rule
    public ActivityTestRule<MockUnityActivity> rule = new ActivityTestRule<>(MockUnityActivity.class);

    @Test
    public void testCreateSessionWithValidParams() {
        final MockUnityActivity activity = rule.getActivity();
        AndroidPayCheckoutSession session = new AndroidPayCheckoutSession(activity);
        boolean result = session.checkoutWithAndroidPay(
                "test",
                "merchantName",
                "publicKey",
                "{}",
                "CAD",
                "CA",
                false,
                true);

        assertTrue(result);
    }

    @Test(expected = NullPointerException.class)
    public void testCreateSessionWithInvalidParams() {
        final MockUnityActivity activity = rule.getActivity();
        AndroidPayCheckoutSession session = new AndroidPayCheckoutSession(activity);
        boolean result = session.checkoutWithAndroidPay(
                null,
                null,
                null,
                "{}",
                "CAD",
                "CA",
                false,
                true);
    }
}
