package com.shopify.unity.buy.utils;

import android.util.Log;

/**
 * Utility class that intercepts the system logger and provides a little more control on top of it,
 * such being able to turn it on/off when needed.
 */

public final class Logger {

    /**
     * If {@code false}, all log calls in this class are ignored.
     */
    private static boolean enabled;

    private Logger() {

    }

    /**
     * Sets whether all log calls on this class must be ignored or not.
     *
     * @param enabled {@code false} to ignore all log calls, {@code true} otherwise
     */
    public static void setEnabled(boolean enabled) {
        Logger.enabled = enabled;
    }

    /**
     * If log is enabled, this will log a message at debug level. The log tag will be
     * in the format of {@code CallingClassName:CallingLine}, (error.g.: {@code MessageCenter:32}).
     *
     * @param msg the message to be logged
     */
    public static void debug(String msg) {
        if (enabled) {
            Log.d(buildTag(), msg);
        }
    }

    /**
     * If log is enabled, this will log a message at error level. The log tag will be
     * in the format of {@code CallingClassName:CallingLine}, (error.g.: {@code MessageCenter:32}).
     *
     * @param msg the message to be logged
     */
    public static void error(String msg) {
        if (enabled) {
            Log.e(buildTag(), msg);
        }
    }

    /**
     * Builds a log tag according to the class name and line that the logging method is
     * called from. For example:
     *
     * <pre><code>
     *     class MyClass {
     *         void myMethod() {
     *             Logger.debug("some message");
     *         }
     *     }
     * </code></pre>
     *
     * The tag generated for this call will be {@code MyClass:3} because
     * {@link Logger#debug(String)} is being called in {@code MyClass} at line {@code 3}.
     *
     * @return a log tag to be used by logging methods
     */
    private static String buildTag() {
        final StackTraceElement element = Thread.currentThread().getStackTrace()[4];
        final String className = element.getClassName();
        final int classNameStartPosition = className.lastIndexOf('.') + 1;
        final String simpleClassName = className.substring(classNameStartPosition);
        return simpleClassName + ":" + element.getLineNumber();
    }
}
