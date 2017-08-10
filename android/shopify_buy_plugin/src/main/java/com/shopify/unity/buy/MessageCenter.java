package com.shopify.unity.buy;

import com.unity3d.player.UnityPlayer;

import java.util.HashMap;
import java.util.Map;

public class MessageCenter {
    private static Map<String, MessageCallbacks> callbacksInWaiting = new HashMap<>();

    private MessageCenter() { }

    // Fire and forget sending of messages.
    static void sendMessageTo(final UnityMessage msg, final UnityMessageReceiver receiver) {
        UnityPlayer.UnitySendMessage(receiver.unityDelegateObjectName, receiver.method.name, msg.toJsonString());
    }

    // Send message with callbacks to invoke when complete.
    static void sendMessageTo(final UnityMessage msg, final UnityMessageReceiver receiver, final MessageCallbacks callbacks) {
        callbacksInWaiting.put(msg.identifier, callbacks);
        UnityPlayer.UnitySendMessage(receiver.unityDelegateObjectName, receiver.method.name, msg.toJsonString());
    }

    @SuppressWarnings("unused")
    public static void onUnityResponse(final String result) {
        UnityMessage msg = UnityMessage.fromUnity(result);
        MessageCallbacks callbacks = callbacksInWaiting.remove(msg.identifier);
        if (callbacks != null) {
            callbacks.onResponse(msg.content);
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

    interface MessageCallbacks {
        void onResponse(String jsonResponse);
    }
}
