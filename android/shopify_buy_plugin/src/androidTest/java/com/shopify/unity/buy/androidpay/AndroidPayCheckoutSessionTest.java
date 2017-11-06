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
package com.shopify.unity.buy.androidpay;

import android.app.Fragment;
import android.app.FragmentManager;
import android.app.FragmentTransaction;
import android.content.pm.PackageManager;
import android.support.test.InstrumentationRegistry;
import android.support.test.filters.MediumTest;
import android.support.test.runner.AndroidJUnit4;

import com.shopify.buy3.pay.PayCart;
import com.shopify.unity.buy.ShopifyUnityPlayerActivity;

import org.json.JSONException;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mockito;

import static junit.framework.Assert.assertNotNull;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.Mockito.when;

@MediumTest
@RunWith(AndroidJUnit4.class)
public class AndroidPayCheckoutSessionTest {
    private ShopifyUnityPlayerActivity mockActivity;

    @Before
    public void setUp() throws Exception {
        this.mockActivity = Mockito.mock(ShopifyUnityPlayerActivity.class);
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
    public void testCartFromUnityWithValidParams() throws JSONException {
        AndroidPayCheckoutSession session = new AndroidPayCheckoutSession(mockActivity, "", true);
        PayCart payCart = session.cartFromUnity(
            "merchantName",
            "{\"totalPrice\":\"6.46\",\"subtotal\":\"5.23\",\"taxPrice\":\"1.23\"}",
            "CAD",
            "CA",
            false);
        assertNotNull(payCart);
    }

    @Test(expected = NullPointerException.class)
    public void testCartFromUnityWithInvalidParams() throws JSONException {
        AndroidPayCheckoutSession session = new AndroidPayCheckoutSession(mockActivity, "", true);
        PayCart payCart = session.cartFromUnity(
                null,
                "{\"totalPrice\":\"6.46\",\"subtotal\":\"5.23\",\"taxPrice\":\"1.23\"}",
                "CAD",
                "CA",
                false);
        assertNotNull(payCart);
    }
}
