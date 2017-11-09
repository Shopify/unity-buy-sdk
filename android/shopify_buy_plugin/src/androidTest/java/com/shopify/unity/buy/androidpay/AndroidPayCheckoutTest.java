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
package com.shopify.unity.buy.androidpay;

import android.app.Activity;
import android.content.Intent;
import android.support.test.filters.MediumTest;
import android.support.test.runner.AndroidJUnit4;

import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.identity.intents.model.UserAddress;
import com.google.android.gms.wallet.MaskedWallet;
import com.google.android.gms.wallet.WalletConstants;
import com.shopify.buy3.pay.PayCart;
import com.shopify.unity.buy.MessageCenter;
import com.shopify.unity.buy.UnityMessage;
import com.shopify.unity.buy.models.AndroidPayEventResponse;
import com.shopify.unity.buy.models.CheckoutInfo;
import com.shopify.unity.buy.models.PricingLineItems;
import com.shopify.unity.buy.models.ShippingMethod;
import com.shopify.unity.buy.utils.TestHelpers;

import org.json.JSONObject;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.ArgumentCaptor;
import org.mockito.Mock;
import org.mockito.invocation.InvocationOnMock;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;
import org.mockito.stubbing.Answer;

import java.math.BigDecimal;

import static com.google.android.gms.wallet.WalletConstants.ERROR_CODE_AUTHENTICATION_FAILURE;
import static com.shopify.buy3.pay.PayHelper.REQUEST_CODE_CHANGE_MASKED_WALLET;
import static com.shopify.unity.buy.MessageCenter.MessageCallback;
import static com.shopify.unity.buy.MessageCenter.Method;
import static com.shopify.unity.buy.MessageCenter.Method.ON_UPDATE_SHIPPING_ADDRESS;
import static com.shopify.unity.buy.androidpay.AndroidPayCheckout.Listener;
import static org.junit.Assert.assertEquals;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.doAnswer;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;
import static org.mockito.internal.verification.VerificationModeFactory.atMost;

@MediumTest
@RunWith(AndroidJUnit4.class)
public class AndroidPayCheckoutTest {
    @Rule public MockitoRule mockitoRule = MockitoJUnit.rule();

    // System under test
    private AndroidPayCheckout checkout;

    // SUT dependencies
    @Mock private GoogleApiClient mockGoogleClient;
    @Mock private MessageCenter messageCenter;
    @Mock private Listener listener;
    private PayCart payCart;

    @Before
    public void setUp() {
        GoogleApiClientFactory factory = mock(GoogleApiClientFactory.class);
        when(factory.newGoogleApiClient())
                .thenReturn(mockGoogleClient);
        checkout = new AndroidPayCheckout(factory, messageCenter);

        payCart = PayCart
                .builder()
                .merchantName("Test Co.")
                .currencyCode("CAD")
                .countryCode("CAD")
                .shippingAddressRequired(false)
                .subtotal(BigDecimal.valueOf(4.57))
                .shippingPrice(BigDecimal.valueOf(0))
                .taxPrice(BigDecimal.valueOf(1.23))
                .totalPrice(BigDecimal.valueOf(5.80))
                .build();
    }

    @Test
    public void testGoogleClientConnectsOnResume() {
        checkout.resume();
        verify(mockGoogleClient, atMost(1)).connect();
    }

    @Test
    public void testGoogleClientDisconnectsOnSuspend() {
        checkout.suspend();
        verify(mockGoogleClient, atMost(1)).disconnect();
    }

    @Test
    public void testUpdateShippingMethod() throws Exception {
        ShippingMethod shippingMethod = new ShippingMethod(
                "identifier",
                "detail",
                "label",
                BigDecimal.ZERO
        );
        checkout.updateShippingMethod(shippingMethod);
        ArgumentCaptor<UnityMessage> msgCaptor = ArgumentCaptor.forClass(UnityMessage.class);
        verify(messageCenter).sendMessageTo(
                eq(Method.ON_UPDATE_SHIPPING_LINE),
                msgCaptor.capture(),
                any(MessageCallback.class)
        );
        JSONObject json = new JSONObject(msgCaptor.getValue().content);
        assertEquals("identifier", json.getString("Identifier"));
        assertEquals("detail", json.getString("Detail"));
        assertEquals("label", json.getString("Label"));
        assertEquals(BigDecimal.ZERO, BigDecimal.valueOf(json.getInt("Amount")));
    }

    @Test
    public void testRunsCallbackOnMaskedWalletRequest() throws Exception {
        doAnswer(new Answer() {
            @Override public Object answer(InvocationOnMock invocationOnMock) throws Throwable {
                MessageCallback callback = invocationOnMock.getArgument(2);
                callback.onResponse(TestHelpers.buildAndroidPayEventResponseJson());
                return null;
            }
        }).when(messageCenter).sendMessageTo(
                eq(ON_UPDATE_SHIPPING_ADDRESS),
                any(UnityMessage.class),
                any(MessageCallback.class)
        );

        Intent mockIntent = new Intent();
        UserAddress fakedAddress = TestHelpers.buildMockUserAddress();

        MaskedWallet maskedWallet = TestHelpers.createMaskedWallet(fakedAddress, fakedAddress,
                "fakeemail@email.com", "googleTransactionId");
        mockIntent.putExtra(WalletConstants.EXTRA_MASKED_WALLET, maskedWallet);

        checkout.startCheckout(payCart, "key", listener);
        checkout.handleWalletResponse(REQUEST_CODE_CHANGE_MASKED_WALLET, Activity.RESULT_OK, mockIntent);

        assertEquals(maskedWallet, checkout.getMaskedWallet());
        verify(listener).onSynchronizeCheckoutInfo(any(CheckoutInfo.class));
    }

    @Test
    public void testSendsErrorToUnity() {
        Intent intent = new Intent();
        intent.putExtra(WalletConstants.EXTRA_ERROR_CODE, ERROR_CODE_AUTHENTICATION_FAILURE);
        checkout.handleWalletResponse(REQUEST_CODE_CHANGE_MASKED_WALLET, Activity.RESULT_OK, intent);
        ArgumentCaptor<Method> methodCapture = ArgumentCaptor.forClass(Method.class);
        ArgumentCaptor<UnityMessage> messageCapture = ArgumentCaptor.forClass(UnityMessage.class);
        verify(messageCenter).sendMessageTo(methodCapture.capture(), messageCapture.capture());
        assertEquals(Method.ON_ERROR, methodCapture.getValue());
        assertEquals(messageCapture.getValue().content, "AUTHENTICATION_FAILURE");
    }

    @Test
    public void testSendsCancellationToSessionCallbacks() {
        checkout.handleWalletResponse(REQUEST_CODE_CHANGE_MASKED_WALLET, Activity.RESULT_CANCELED, null);
        ArgumentCaptor<Method> methodCapture = ArgumentCaptor.forClass(Method.class);
        ArgumentCaptor<UnityMessage> messageCapture = ArgumentCaptor.forClass(UnityMessage.class);
        verify(messageCenter).sendMessageTo(methodCapture.capture(), messageCapture.capture());
        assertEquals(Method.ON_CANCEL, methodCapture.getValue());
        assertEquals(messageCapture.getValue().content, "");
    }

    @Test
    public void testPayCartFromEventResponse() {
        PricingLineItems items = new PricingLineItems(
                BigDecimal.valueOf(28.11), // subtotal
                BigDecimal.valueOf(4.76),  // taxPrice
                BigDecimal.valueOf(33.48), // totalPrice
                BigDecimal.valueOf(1.22)   // shippingPrice
        );
        AndroidPayEventResponse response = new AndroidPayEventResponse(
                "merchant name",
                items,
                "CAD",
                "CA",
                true,
                null
        );
        PayCart cart = checkout.payCartFromEventResponse(response);
        assertEquals("merchant name", cart.merchantName);
        assertEquals("CAD", cart.currencyCode);
        assertEquals(true, cart.shippingAddressRequired);
        assertEquals(BigDecimal.valueOf(28.11), cart.subtotal);
        assertEquals(BigDecimal.valueOf(1.22), cart.shippingPrice);
        assertEquals(BigDecimal.valueOf(4.76), cart.taxPrice);
        assertEquals(BigDecimal.valueOf(33.48), cart.totalPrice);
    }
}
