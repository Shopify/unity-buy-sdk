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

import android.app.Activity;
import android.app.Fragment;
import android.app.FragmentManager;
import android.app.FragmentTransaction;
import android.support.test.filters.MediumTest;
import android.support.test.runner.AndroidJUnit4;

import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.ArgumentMatchers;
import org.mockito.Mockito;

import static junit.framework.Assert.assertFalse;
import static junit.framework.Assert.assertTrue;
import static org.mockito.Mockito.when;

@MediumTest
@RunWith(AndroidJUnit4.class)
public class WebIntentFragmentTest implements WebIntentListener {
    private boolean hasClosed = false;
    private Activity mockActivity;

    @Before
    public void setUp() throws Exception {
        this.mockActivity = Mockito.mock(Activity.class);
        when(this.mockActivity.getPackageName()).thenReturn("com.shopify.unity.buy.test");

        FragmentManager mockFragmentManager = Mockito.mock(FragmentManager.class);
        FragmentTransaction mockFragmentTransaction = Mockito.mock(FragmentTransaction.class);

        when(mockFragmentManager.beginTransaction()).thenReturn(mockFragmentTransaction);
        when(mockFragmentTransaction.add(ArgumentMatchers.any(Fragment.class), ArgumentMatchers.anyString())).thenReturn(mockFragmentTransaction);
        when(this.mockActivity.getFragmentManager()).thenReturn(mockFragmentManager);
    }

    @Test
    public void testListener() {
        WebIntentFragment webFragment = WebIntentFragment.newInstance("http://shopify.com");
        webFragment.setListener(this);
        mockActivity.getFragmentManager().beginTransaction().add(webFragment, "").commit();
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
