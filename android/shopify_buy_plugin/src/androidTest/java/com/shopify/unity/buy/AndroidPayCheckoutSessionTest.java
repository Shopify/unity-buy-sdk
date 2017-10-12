package com.shopify.unity.buy;

import android.app.Activity;
import android.app.Fragment;
import android.app.FragmentManager;
import android.app.FragmentTransaction;
import android.content.pm.PackageManager;
import android.support.test.InstrumentationRegistry;
import android.support.test.filters.MediumTest;
import android.support.test.runner.AndroidJUnit4;

import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mockito;

import static junit.framework.Assert.assertTrue;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.Mockito.when;

@MediumTest
@RunWith(AndroidJUnit4.class)
public class AndroidPayCheckoutSessionTest {
    private Activity mockActivity;

    @Before
    public void setUp() throws Exception {
        this.mockActivity = Mockito.mock(Activity.class);
        when(this.mockActivity.getPackageName()).thenReturn("com.shopify.unity.buy.test");

        FragmentManager mockFragmentManager = Mockito.mock(FragmentManager.class);
        FragmentTransaction mockFragmentTransaction = Mockito.mock(FragmentTransaction.class);

        when(mockFragmentManager.beginTransaction()).thenReturn(mockFragmentTransaction);
        when(mockFragmentTransaction.add(any(Fragment.class), anyString())).thenReturn(mockFragmentTransaction);
        when(this.mockActivity.getFragmentManager()).thenReturn(mockFragmentManager);

        PackageManager packageManager =
            InstrumentationRegistry.getTargetContext().getPackageManager();

        when(this.mockActivity.getPackageManager()).thenReturn(packageManager);
    }

    @Test
    public void testCreateSessionWithValidParams() {
        AndroidPayCheckoutSession session = new AndroidPayCheckoutSession(this.mockActivity, true);
        boolean result = session.checkoutWithAndroidPay(
            "test",
            "merchantName",
            "publicKey",
            "{\"totalPrice\":\"6.46\",\"subtotal\":\"5.23\",\"taxPrice\":\"1.23\"}",
            "CAD",
            "CA",
            false);

        assertTrue(result);
    }

    @Test(expected = NullPointerException.class)
    public void testCreateSessionWithInvalidParams() {
        AndroidPayCheckoutSession session = new AndroidPayCheckoutSession(this.mockActivity, true);
        session.checkoutWithAndroidPay(
            null,
            null,
            null,
            "{\"totalPrice\":\"6.46\",\"subtotal\":\"5.23\",\"taxPrice\":\"1.23\"}",
            "CAD",
            "CA",
            false);
    }
}
