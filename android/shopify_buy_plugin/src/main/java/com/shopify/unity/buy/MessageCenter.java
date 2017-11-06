package com.shopify.unity.buy;

import android.os.Handler;
import android.os.Looper;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import com.shopify.unity.buy.models.ShopifyError;
import com.shopify.unity.buy.utils.Logger;
import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

/**
 * Android to Unity message gateway.
 */
public class MessageCenter {

    /** {@link Handler} used to deliver callbacks on the main thread. */
    private static final Handler MAIN_THREAD_HANDLER = new Handler(Looper.getMainLooper());
    /** Maps message identifiers with callbacks so that they can be invoked upon response. */
    private static final Map<String, MessageCallback> CALLBACKS_IN_WAITING = new HashMap<>();
    /** Unity identifier used to send messages across the Android-Unity boundary. */
    private String unityDelegateObjectName;

    /**
     * Creates an instance of this class.
     *
     * @param unityDelegateObjectName Unity identifier used to send messages across
     *         the Android-Unity boundary
     */
    public MessageCenter(String unityDelegateObjectName) {
        this.unityDelegateObjectName = unityDelegateObjectName;
    }

    /**
     * Sends a message to Unity.
     *
     * @param method the method to be invoked on the Unity side
     * @param msg the message to be sent to Unity
     */
    public void sendMessageTo(@NonNull final Method method, @NonNull UnityMessage msg) {
        sendMessageTo(method, msg, null);
    }

    /**
     * Sends a message to Unity.
     *
     * @param method the method to be invoked on the Unity side
     * @param msg the message to be sent to Unity
     * @param callback the callback to be invoked when a response is received
     */
    public void sendMessageTo(@NonNull Method method, @NonNull UnityMessage msg,
                                     @Nullable MessageCallback callback) {
        if (callback != null) {
            CALLBACKS_IN_WAITING.put(msg.identifier, callback);
        }
        final String msgStr = msg.toJson().toString();
        UnityPlayer.UnitySendMessage(unityDelegateObjectName, method.name, msgStr);
    }

    /**
     * Processes a message sent from Unity in response to another one sent from Android.
     *
     * @param identifier the message identifier
     * @param content the message content represented in a JSON object
     */
    @SuppressWarnings("unused")
    public static void onUnityResponse(final String identifier, final String content) {
        final String msg = "New message from Unity: identifier = " +
                identifier + ", content = " + content;
        Logger.debug(msg);
        final MessageCallback callback = CALLBACKS_IN_WAITING.remove(identifier);
        if (callback != null) {
            MAIN_THREAD_HANDLER.post(new Runnable() {
                @Override public void run() {
                    deliverResult(callback, content);
                }
            });
        }
    }

    /**
     * Invokes the proper callback method according to the {@code content}.
     * If the {@code content} contains an error, {@link MessageCallback#onError(ShopifyError)}
     * will be invoked, or {@link MessageCallback#onResponse(String)} otherwise.
     *
     * @param callback the callback to be invoked according to the {@code content}
     * @param content the JSON message sent from Unity
     */
    private static void deliverResult(MessageCallback callback, String content) {
        try {
            Logger.debug(content);
            final JSONObject json = new JSONObject(content);
            callback.onError(ShopifyError.fromJson(json));
        } catch (JSONException ignored) {
            // If exception thrown, this is not an error.
            callback.onResponse(content);
        }
    }

    /**
     * Unity API definition with the methods that can be called from Android.
     */
    public enum Method {
        ON_NATIVE_MESSAGE("OnNativeMessage"),
        ON_UPDATE_SHIPPING_ADDRESS("OnUpdateShippingAddress"),
        ON_UPDATE_SHIPPING_LINE("OnUpdateShippingLine"),
        ON_CONFIRM_CHECKOUT("OnConfirmCheckout"),
        ON_ERROR("OnError"),
        ON_CANCEL("OnCancel"),
        ON_CAN_CHECKOUT_WITH_AP_RESULT("OnCanCheckoutWithAndroidPayResult");

        private final String name;

        Method(String name) {
            this.name = name;
        }
    }

    /**
     * Callback interface that gets invoked when Unity responds back.
     */
    public interface MessageCallback {
        /** Invoked when a valid response has been sent from Unity. */
        void onResponse(String jsonResponse);
        /** Invoked when an error occurs on the Unity side. */
        void onError(ShopifyError error);
    }
}
