package com.shopify.unity.buy.androidpay;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.support.test.filters.MediumTest;
import android.support.test.runner.AndroidJUnit4;

import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.identity.intents.model.UserAddress;
import com.google.android.gms.wallet.MaskedWallet;
import com.google.android.gms.wallet.WalletConstants;
import com.shopify.buy3.pay.PayCart;
import com.shopify.unity.buy.MessageCenter;
import com.shopify.unity.buy.models.MailingAddressInput;
import com.shopify.unity.buy.utils.TestHelpers;

import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;

import java.math.BigDecimal;

import static com.google.android.gms.wallet.WalletConstants.ERROR_CODE_AUTHENTICATION_FAILURE;
import static com.shopify.buy3.pay.PayHelper.REQUEST_CODE_CHANGE_MASKED_WALLET;
import static com.shopify.unity.buy.utils.TestHelpers.mailingAddressEquals;
import static junit.framework.Assert.assertEquals;
import static junit.framework.Assert.assertNotNull;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.matches;
import static org.mockito.Mockito.verify;
import static org.mockito.internal.verification.VerificationModeFactory.atMost;

@MediumTest
@RunWith(AndroidJUnit4.class)
public class UnityAndroidPayFragmentTest {
    @Rule public MockitoRule mockitoRule = MockitoJUnit.rule();

    private PayCart mockCart;
    private UnityAndroidPayFragment fragment;

    @Mock
    private AndroidPaySessionCallback mockCallbacks;

    @Mock
    private GoogleApiClient mockGoogleClient;

    @Before
    public void setUp() {
        mockCart = PayCart.builder()
            .merchantName("Test Co.")
            .currencyCode("CAD")
            .shippingAddressRequired(false)
            .subtotal(BigDecimal.valueOf(4.57))
            .shippingPrice(BigDecimal.valueOf(0))
            .taxPrice(BigDecimal.valueOf(1.23))
            .totalPrice(BigDecimal.valueOf(5.80))
            .build();

        UnityAndroidPayFragment.UnityAndroidPayFragmentBuilder builder =
            new UnityAndroidPayFragment.UnityAndroidPayFragmentBuilder();

        fragment = builder
            .setPayCart(mockCart)
            .setCountryCode("CA")
            .setEnvironment(WalletConstants.ENVIRONMENT_TEST)
            .setPublicKey("abc")
            .setSessionCallbacks(mockCallbacks)
            .build();
    }

    @Test
    public void testFragmentBuilder() {
        UnityAndroidPayFragment.UnityAndroidPayFragmentBuilder builder =
            new UnityAndroidPayFragment.UnityAndroidPayFragmentBuilder();

        UnityAndroidPayFragment fragment = builder
            .setPayCart(mockCart)
            .setCountryCode("CA")
            .setEnvironment(WalletConstants.ENVIRONMENT_TEST)
            .setPublicKey("abc")
            .setSessionCallbacks(mockCallbacks)
            .build();

        assertNotNull(fragment);

        Bundle arguments = fragment.getArguments();
        assertEquals(arguments.getString(UnityAndroidPayFragment.EXTRA_COUNTRY_CODE), "CA");
        assertEquals(arguments.getString(UnityAndroidPayFragment.EXTRA_PUBLIC_KEY), "abc");
        assertEquals(arguments.getParcelable(UnityAndroidPayFragment.EXTRA_PAY_CART), mockCart);
        assertEquals(arguments.getInt(UnityAndroidPayFragment.EXTRA_ANDROID_PAY_ENVIRONMENT),
            WalletConstants.ENVIRONMENT_TEST);
    }

    @Test(expected = IllegalArgumentException.class)
    public void testFragmentBuilderWithMissingRequiredParams() {
        UnityAndroidPayFragment.UnityAndroidPayFragmentBuilder builder =
            new UnityAndroidPayFragment.UnityAndroidPayFragmentBuilder();

        // Missing Public Key and Country Code
        builder
            .setPayCart(mockCart)
            .setEnvironment(WalletConstants.ENVIRONMENT_TEST)
            .setSessionCallbacks(mockCallbacks)
            .build();
    }

    @Test
    public void testGoogleClientConnectsOnStart() {
        fragment.setGoogleClient(mockGoogleClient);
        verify(mockGoogleClient, atMost(1)).connect();
    }

    @Test
    public void testGoogleClientDisconnectsOnStop() {
        fragment.setGoogleClient(mockGoogleClient);
        fragment.onStop();
        verify(mockGoogleClient, atMost(1)).disconnect();
    }

    @Test
    public void testSendMailingAddressOnMaskedWalletRequest() throws Exception {
        Intent mockIntent = new Intent();
        UserAddress fakedAddress = TestHelpers.buildMockUserAddress();
        MailingAddressInput expected = new MailingAddressInput(fakedAddress);

        MaskedWallet maskedWallet = TestHelpers.createMaskedWallet(fakedAddress, fakedAddress,
            "fakeemail@email.com", "googleTransactionId");
        mockIntent.putExtra(WalletConstants.EXTRA_MASKED_WALLET, maskedWallet);

        fragment.onActivityResult(REQUEST_CODE_CHANGE_MASKED_WALLET, Activity.RESULT_OK, mockIntent);
        verify(mockCallbacks).onUpdateShippingAddress(mailingAddressEquals(expected),
            any(MessageCenter.MessageCallback.class));
    }

    @Test
    public void testSendErrorToSessionCallbacks() {
        Intent intent = new Intent();
        intent.putExtra(WalletConstants.EXTRA_ERROR_CODE, ERROR_CODE_AUTHENTICATION_FAILURE);
        fragment.onActivityResult(REQUEST_CODE_CHANGE_MASKED_WALLET, Activity.RESULT_OK, intent);

        verify(mockCallbacks).onError(matches("AUTHENTICATION_FAILURE"));
    }

    @Test
    public void testSendCancellationToSessionCallbacks() {
        fragment.onActivityResult(REQUEST_CODE_CHANGE_MASKED_WALLET, Activity.RESULT_CANCELED, null);
        verify(mockCallbacks).onCancel();
    }
}
