package com.shopify.unity.buy.utils;

public class JUnitLogger implements ILogger {
    @Override
    public void error(String tag, String message) {
        System.out.println(tag + ":" + message);
    }
}
