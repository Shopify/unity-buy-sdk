package com.shopify.unity.buy;

import android.os.Handler;
import android.os.HandlerThread;

import com.unity3d.player.UnityPlayer;

import java.util.HashMap;
import java.util.Map;

public class MessageCenter {
    private static final HandlerThread handlerThread = new HandlerThread("UnityMessageThread");
    private static final Handler messageHandler = new Handler(handlerThread.getLooper());

    private static Map<String, MessageCallbacks> callbacksInWaiting = new HashMap<>();

    private MessageCenter() { }

    // Fire and forget sending of messages.
    static void sendMessageTo(final UnityMessage msg, final UnityMessageReceiver receiver) {
        messageHandler.post(new Runnable() {
            @Override
            public void run() {
                UnityPlayer.UnitySendMessage(receiver.unityDelegateObjectName, receiver.method.name, msg.toJsonString());
            }
        });
    }

    // Send message with callbacks to invoke when complete.
    static void sendMessageTo(final UnityMessage msg, final UnityMessageReceiver receiver, final MessageCallbacks callbacks) {
        messageHandler.post(new Runnable() {
            @Override
            public void run() {
                callbacksInWaiting.put(msg.identifier, callbacks);
                UnityPlayer.UnitySendMessage(receiver.unityDelegateObjectName, receiver.method.name, msg.toJsonString());
            }
        });
    }

    @SuppressWarnings("unused")
    public static void onUnityResponse(final String result) {
        messageHandler.post(new Runnable() {
            @Override
            public void run() {
                UnityMessage msg = UnityMessage.fromUnity(result);
                MessageCallbacks callbacks = callbacksInWaiting.remove(msg.identifier);
                if (callbacks != null) {
                    callbacks.onResponse(msg.content);
                }
            }
        });
    }

    static class UnityMessageReceiver {
        private final String unityDelegateObjectName;
        private final Method method;

        UnityMessageReceiver(String unityDelegateObjectName, Method method) {
            this.unityDelegateObjectName = unityDelegateObjectName;
            this.method = method;
        }
    }

    enum Method {
        ON_UPDATE_SHIPPING_ADDRESS("OnUpdateShippingAddress"),
        ON_ERROR("OnError"),
        ON_CANCEL("OnCancel");

        private final String name;

        Method(String name) {
            this.name = name;
        }
    }

    interface MessageCallbacks {
        void onResponse(String jsonResponse);
    }
}
