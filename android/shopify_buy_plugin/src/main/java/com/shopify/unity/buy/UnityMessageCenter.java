/**
 * The MIT License (MIT)
 * Copyright (c) 2017 Shopify
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */
package com.shopify.unity.buy;

import android.os.Handler;
import android.os.Looper;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import com.shopify.unity.buy.models.AndroidPayEventResponse;
import com.shopify.unity.buy.models.MailingAddressInput;
import com.shopify.unity.buy.models.NativePayment;
import com.shopify.unity.buy.models.ShippingMethod;
import com.shopify.unity.buy.models.ShopifyError;
import com.shopify.unity.buy.models.WalletError;
import com.shopify.unity.buy.utils.Logger;
import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

/**
 * Android to Unity message gateway.
 */
public class UnityMessageCenter implements MessageCenter {

    private static final String NATIVE_WEB_DELEGATE_METHOD_CONTENT = "dismissed";
    /** {@link Handler} used to deliver callbacks on the main thread. */
    private static final Handler MAIN_THREAD_HANDLER = new Handler(Looper.getMainLooper());
    /**
     * Maps message identifiers with {@link UnityCall UnityCalls} so that
     * they can be invoked upon response.
     */
    private static final Map<String, UnityCall> CALLS_IN_WAITING = new HashMap<>();
    /** Maps class types to their respective {@link TypeConverter TypeConverters}. */
    private static final Map<Class<?>, TypeConverter<?>> TYPE_CONVERTERS = new HashMap<>();
    /** Unity identifier used to send messages across the Android-Unity boundary. */
    private String unityDelegateObjectName;

    static {
        TYPE_CONVERTERS.put(AndroidPayEventResponse.class,
                new TypeConverter<AndroidPayEventResponse>() {
                    @Override @NonNull
                    public AndroidPayEventResponse parse(String json) throws JSONException {
                        return AndroidPayEventResponse.fromJsonString(json);
                    }
                });
        TYPE_CONVERTERS.put(Void.class,
                new TypeConverter<Void>() {
                    @Override @NonNull
                    public Void parse(String json) throws JSONException {
                        return null;
                    }
                });
    }

    /**
     * Reserved for Unity.
     */
    private UnityMessageCenter() {
    }

    /**
     * Creates an instance of this class.
     *
     * @param unityDelegateObjectName Unity identifier used to send messages across
     *         the Android-Unity boundary
     */
    public UnityMessageCenter(String unityDelegateObjectName) {
        this.unityDelegateObjectName = unityDelegateObjectName;
    }

    /**
     * Sends a message to Unity.
     *
     * @param method the method to be invoked on the Unity side
     * @param msg the message to be sent to Unity
     */
    private void sendMessageTo(@NonNull final Method method, @NonNull UnityMessage msg) {
        sendMessageTo(method, msg, null);
    }

    /**
     * Sends a message to Unity.
     *
     * @param method the method to be invoked on the Unity side
     * @param msg the message to be sent to Unity
     * @param callback the callback to be invoked when a response is received
     */
    private <T> void sendMessageTo(@NonNull Method<T> method, @NonNull UnityMessage msg,
                                   @Nullable MessageCallback<T> callback) {
        if (callback != null) {
            final UnityCall<T> call = new UnityCall<>(method, callback);
            CALLS_IN_WAITING.put(msg.identifier, call);
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
        final UnityCall call = CALLS_IN_WAITING.remove(identifier);
        if (call != null) {
            MAIN_THREAD_HANDLER.post(new Runnable() {
                @Override public void run() {
                    deliverResult(call, content);
                }
            });
        }
    }

    /**
     * Invokes the proper callback method according to the {@code content}.
     * If the {@code content} contains an error, {@link MessageCallback#onError(ShopifyError)}
     * will be invoked, or {@link MessageCallback#onResponse(Object)} otherwise.
     *
     * @param call the {@link UnityCall} that this result corresponds to
     * @param content the JSON message sent from Unity
     */
    private static <T> void deliverResult(
            @NonNull UnityCall<T> call,
            @NonNull String content
    ) {
        final MessageCallback<T> callback = call.messageCallback;
        if (callback == null) {
            return; // Not result to be delivered.
        }
        try {
            Logger.debug(content);
            final JSONObject json = new JSONObject(content);
            call.messageCallback.onError(ShopifyError.fromJson(json));
        } catch (JSONException ignored) {
            // If exception thrown, this is not an error.
            final Class<?> responseType = call.method.responseType;
            @SuppressWarnings("unchecked") final TypeConverter<T> converter =
                    (TypeConverter<T>) TYPE_CONVERTERS.get(responseType);
            if (converter == null) {
                Logger.error("Missing converter for type " + responseType.getCanonicalName());
            } else {
                try {
                    call.messageCallback.onResponse(converter.parse(content));
                } catch (JSONException e) {
                    Logger.error("Error trying to parse json: " + content);
                    e.printStackTrace();
                }
            }
        }
    }

    @Override
    public void onNativeMessage() {
        final UnityMessage message = UnityMessage.fromContent(NATIVE_WEB_DELEGATE_METHOD_CONTENT);
        sendMessageTo(UnityMessageCenter.Method.ON_NATIVE_MESSAGE, message);
    }

    @Override
    public void onUpdateShippingAddress(
            @NonNull MailingAddressInput input,
            @NonNull MessageCallback<AndroidPayEventResponse> callback
    ) {
        final UnityMessage msg = UnityMessage.fromContent(input.toJson().toString());
        sendMessageTo(Method.ON_UPDATE_SHIPPING_ADDRESS, msg, callback);
    }

    @Override
    public void onUpdateShippingLine(
            @NonNull ShippingMethod shippingMethod,
            @NonNull MessageCallback<AndroidPayEventResponse> callback
    ) {
        final String jsonStr = shippingMethod.toJson().toString();
        final UnityMessage msg = UnityMessage.fromContent(jsonStr);
        sendMessageTo(Method.ON_UPDATE_SHIPPING_LINE, msg, callback);
    }

    @Override
    public void onConfirmCheckout(@NonNull NativePayment nativePayment) {
        final UnityMessage msg = UnityMessage.fromContent(nativePayment.toJson().toString());
        sendMessageTo(Method.ON_CONFIRM_CHECKOUT, msg);
    }

    @Override
    public void onError(@NonNull WalletError walletError) {
        final UnityMessage message = UnityMessage.fromContent(walletError.toString());
        sendMessageTo(Method.ON_ERROR, message);
    }

    @Override
    public void onCancel() {
        sendMessageTo(Method.ON_CANCEL, UnityMessage.fromContent(""));
    }

    @Override
    public void onCanCheckoutWithAndroidPayResult(boolean canCheckoutWithAndroidPay) {
        final String str = String.valueOf(canCheckoutWithAndroidPay);
        final UnityMessage msg = UnityMessage.fromContent(str);
        sendMessageTo(Method.ON_CAN_CHECKOUT_WITH_AP_RESULT, msg);
    }

    /**
     * Unity API definition with the methods that can be called from Android.
     */
    private static final class Method<T> {

        static final Method<Void> ON_NATIVE_MESSAGE = new Method<>(
                "OnNativeMessage",
                Void.class
        );
        static final Method<AndroidPayEventResponse> ON_UPDATE_SHIPPING_ADDRESS = new Method<>(
                "OnUpdateShippingAddress",
                AndroidPayEventResponse.class
        );
        static final Method<AndroidPayEventResponse> ON_UPDATE_SHIPPING_LINE = new Method<>(
                "OnUpdateShippingLine",
                AndroidPayEventResponse.class
        );
        static final Method<Void> ON_CONFIRM_CHECKOUT = new Method<>(
                "OnConfirmCheckout",
                Void.class
        );
        static final Method<Void> ON_ERROR = new Method<>(
                "OnError",
                Void.class
        );
        static final Method<Void> ON_CANCEL = new Method<>(
                "OnCancel",
                Void.class
        );
        static final Method<Void> ON_CAN_CHECKOUT_WITH_AP_RESULT = new Method<>(
                "OnCanCheckoutWithAndroidPayResult",
                Void.class
        );

        /** Unity method name. */
        private final String name;
        /** Type of the object responded by Unity. */
        private final Class<T> responseType;

        Method(String name, Class<T> responseType) {
            this.name = name;
            this.responseType = responseType;
        }
    }

    /**
     * Represents a Unity method call.
     *
     * @param <T> the response type expected for this call.
     */
    private static class UnityCall<T> {

        /** The Unity method to be called. */
        @NonNull private final Method<T> method;
        /** The callback to propagate the response back to. */
        @Nullable private final MessageCallback<T> messageCallback;

        UnityCall(@NonNull Method<T> method,
                  @Nullable MessageCallback<T> messageCallback) {

            this.method = method;
            this.messageCallback = messageCallback;
        }
    }

    /**
     * Callback interface that gets invoked when Unity responds back.
     */
    public interface MessageCallback<T> {
        /** Invoked when a valid response has been sent from Unity. */
        void onResponse(@NonNull T response);
        /** Invoked when an error occurs on the Unity side. */
        void onError(@NonNull ShopifyError error);
    }

    /**
     * Defines a type converter to parse JSON objects sent by Unity
     * to Java objects manipulated by this plugin.
     *
     * @param <T> the type that this {@code TypeConverter} converts to
     */
    public interface TypeConverter<T> {

        /**
         * Parses a JSON string containing an object of type {@code T}.
         *
         * @param json the JSON to be parsed into an object
         * @return an instance of the object {@code T} with the contents of the JSON string
         * @throws JSONException if some JSON error occurs
         */
        @NonNull T parse(String json) throws JSONException;
    }
}
