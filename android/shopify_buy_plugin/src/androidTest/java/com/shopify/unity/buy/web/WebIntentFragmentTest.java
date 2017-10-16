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
