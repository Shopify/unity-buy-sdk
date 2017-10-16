package com.shopify.unity.buy;

import com.shopify.unity.buy.utils.Logger;
import com.unity3d.player.UnityPlayer;

import java.util.HashMap;
import java.util.Map;

public class MessageCenter {
    private static Map<String, MessageCallback> callbacksInWaiting = new HashMap<>();

    private MessageCenter() { }

    // Fire and forget sending of messages.
    public static void sendMessageTo(final UnityMessage msg, final UnityMessageReceiver receiver) {
        UnityPlayer.UnitySendMessage(receiver.unityDelegateObjectName, receiver.method.name, msg.toJsonString());
    }

    // Send message with callbacks to invoke when complete.
    public static void sendMessageTo(final UnityMessage msg, final UnityMessageReceiver receiver, final MessageCallback callbacks) {
        callbacksInWaiting.put(msg.identifier, callbacks);
        UnityPlayer.UnitySendMessage(receiver.unityDelegateObjectName, receiver.method.name, msg.toJsonString());
    }

    @SuppressWarnings("unused")
    public static void onUnityResponse(final String identifier, final String content) {
        Logger.debug("New message: identifier = " + identifier + ", content = " + content);
        MessageCallback callbacks = callbacksInWaiting.remove(identifier);
        if (callbacks != null) {
            callbacks.onResponse(content);
        }
    }

    public static class UnityMessageReceiver {
        private final String unityDelegateObjectName;
        private final Method method;

        public UnityMessageReceiver(String unityDelegateObjectName, Method method) {
            this.unityDelegateObjectName = unityDelegateObjectName;
            this.method = method;
        }
    }

    public enum Method {
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

    public interface MessageCallback {
        void onResponse(String jsonResponse);
    }
}
