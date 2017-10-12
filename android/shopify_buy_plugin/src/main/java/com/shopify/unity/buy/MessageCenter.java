package com.shopify.unity.buy;

import com.shopify.unity.buy.utils.Logger;
import com.unity3d.player.UnityPlayer;

import java.util.HashMap;
import java.util.Map;

public class MessageCenter {
    private static Map<String, MessageCallback> callbacksInWaiting = new HashMap<>();

    private MessageCenter() { }

    // Fire and forget sending of messages.
    static void sendMessageTo(final UnityMessage msg, final UnityMessageReceiver receiver) {
        UnityPlayer.UnitySendMessage(receiver.unityDelegateObjectName, receiver.method.name, msg.toJsonString());
    }

    // Send message with callbacks to invoke when complete.
    static void sendMessageTo(final UnityMessage msg, final UnityMessageReceiver receiver, final MessageCallback callbacks) {
        callbacksInWaiting.put(msg.identifier, callbacks);
        UnityPlayer.UnitySendMessage(receiver.unityDelegateObjectName, receiver.method.name, msg.toJsonString());
    }

    @SuppressWarnings("unused")
    public static void onUnityResponse(final String identifier, final String content) {
        Logger.d("New message: identifier = " + identifier + ", content = " + content);
        MessageCallback callbacks = callbacksInWaiting.remove(identifier);
        if (callbacks != null) {
            callbacks.onResponse(content);
        }
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
        ON_NATIVE_MESSAGE("OnNativeMessage"),
        ON_UPDATE_SHIPPING_ADDRESS("OnUpdateShippingAddress"),
        ON_ERROR("OnError"),
        ON_CANCEL("OnCancel"),
        ON_COMPLETE("OnComplete");

        private final String name;

        Method(String name) {
            this.name = name;
        }
    }

    interface MessageCallback {
        void onResponse(String jsonResponse);
    }
}
