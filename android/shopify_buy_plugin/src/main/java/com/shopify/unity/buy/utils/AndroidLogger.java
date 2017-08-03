package com.shopify.unity.buy.utils;

import android.util.Log;

public class AndroidLogger implements ILogger {
    @Override
    public void error(String tag, String message) {
        Log.e(tag, message);
    }
}
