package com.shopify.unity.buy.utils;

import android.util.Log;

/**
 * @author Flavio Faria
 */

public final class Logger {

    private static boolean enabled;

    private Logger() {

    }

    public static void setEnabled(boolean enabled) {
        Logger.enabled = enabled;
    }

    public static void d(String msg) {
        if (enabled) {
            Log.d(buildTag(), msg);
        }
    }

    public static void e(String msg) {
        if (enabled) {
            Log.e(buildTag(), msg);
        }
    }

    private static String buildTag() {
        final StackTraceElement element = Thread.currentThread().getStackTrace()[4];
        final String className = element.getClassName();
        final int classNameStartPosition = className.lastIndexOf('.') + 1;
        final String simpleClassName = className.substring(classNameStartPosition);
        return simpleClassName + ":" + element.getLineNumber();
    }
}
