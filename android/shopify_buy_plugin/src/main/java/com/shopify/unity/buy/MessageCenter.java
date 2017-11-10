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

import android.support.annotation.NonNull;

import com.shopify.unity.buy.UnityMessageCenter.MessageCallback;
import com.shopify.unity.buy.models.AndroidPayEventResponse;
import com.shopify.unity.buy.models.MailingAddressInput;
import com.shopify.unity.buy.models.NativePayment;
import com.shopify.unity.buy.models.ShippingMethod;
import com.shopify.unity.buy.models.WalletError;

/**
 * Java code to platform code message gateway.
 */

public interface MessageCenter {

    /**
     * Notifies the platform code that the web checkout form is no longer visible.
     */
    void onNativeMessage();

    /**
     * Notifies the platform code that the shipping address has changed.
     *
     * @param input the new shipping address
     * @param callback to be invoked when the address is updated
     */
    void onUpdateShippingAddress(
            @NonNull MailingAddressInput input,
            @NonNull MessageCallback<AndroidPayEventResponse> callback
    );

    /**
     * Notifies the platform code that the shipping line has changed.
     *
     * @param shippingMethod the new shipping method
     * @param callback to be invoked when the line is updated
     */
    void onUpdateShippingLine(
            @NonNull ShippingMethod shippingMethod,
            @NonNull MessageCallback<AndroidPayEventResponse> callback
    );

    /**
     * Notifies the platform code that the user has confirmed the checkout.
     *
     * @param nativePayment the object containing all data to complete the checkout
     */
    void onConfirmCheckout(@NonNull NativePayment nativePayment);

    /**
     * Notifies the platform code about an error that happened during the native payment.
     *
     * @param walletError the error that happened during the native payment
     */
    void onError(@NonNull WalletError walletError);

    /**
     * Notifies the platform code that the native checkout flow has been canceled.
     */
    void onCancel();

    /**
     * Delivers to the platform code the result of a previous request to check if
     * native pay is supported.
     *
     * @param canCheckoutWithAndroidPay {@code true} if native pay is supported,
     *         {@code false} otherwise
     */
    void onCanCheckoutWithAndroidPayResult(boolean canCheckoutWithAndroidPay);
}
