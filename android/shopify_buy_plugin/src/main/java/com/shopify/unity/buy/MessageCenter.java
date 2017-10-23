package com.shopify.unity.buy;

import android.os.Handler;
import android.os.Looper;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import com.shopify.unity.buy.utils.Logger;
import com.unity3d.player.UnityPlayer;

import java.util.HashMap;
import java.util.Map;

public class MessageCenter {

    private static final Handler MAIN_THREAD_HANDLER = new Handler(Looper.getMainLooper());
    private static Map<String, MessageCallback> callbacksInWaiting = new HashMap<>();
    private static String unityDelegateObjectName;

    public static void init(String unityDelegateObjectName) {
        MessageCenter.unityDelegateObjectName = unityDelegateObjectName;
    }

    // Fire and forget sending of messages.
    public void sendMessageTo(@NonNull final Method method, @NonNull UnityMessage msg) {
        sendMessageTo(method, msg, null);
    }

    // Send message with callbacks to invoke when complete.
    public void sendMessageTo(@NonNull Method method, @NonNull UnityMessage msg,
                                     @Nullable MessageCallback callbacks) {
        if (callbacks != null) {
            callbacksInWaiting.put(msg.identifier, callbacks);
        }
        UnityPlayer.UnitySendMessage(unityDelegateObjectName, method.name, msg.toJson().toString());
    }

    @SuppressWarnings("unused")
    public static void onUnityResponse(final String identifier, final String content) {
        final String msg = "New message from Unity: identifier = " +
                identifier + ", content = " + content;
        Logger.debug(msg);
        final MessageCallback callbacks = callbacksInWaiting.remove(identifier);
        if (callbacks != null) {
            MAIN_THREAD_HANDLER.post(new Runnable() {
                @Override public void run() {
                    callbacks.onResponse(content);
                }
            });
        }
    }

    public enum Method {
        ON_NATIVE_MESSAGE("OnNativeMessage"),
        ON_UPDATE_SHIPPING_ADDRESS("OnUpdateShippingAddress"),
        ON_UPDATE_SHIPPING_LINE("OnUpdateShippingLine"),
        ON_ERROR("OnError"),
        ON_CANCEL("OnCancel"),
        ON_COMPLETE("OnComplete");

        private final String name;

        Method(String name) {
            this.name = name;
        }
    }

    public interface MessageCallback {
        void onResponse(String jsonResponse);
    }
}
