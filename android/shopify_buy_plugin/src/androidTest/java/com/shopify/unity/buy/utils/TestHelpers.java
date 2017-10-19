package com.shopify.unity.buy.utils;

import com.google.android.gms.identity.intents.model.UserAddress;
import com.google.android.gms.wallet.MaskedWallet;
import com.shopify.unity.buy.models.MailingAddressInput;

import org.mockito.ArgumentMatcher;

import java.lang.reflect.Constructor;

import static org.mockito.ArgumentMatchers.argThat;

public class TestHelpers {
    private TestHelpers() { }

    public static UserAddress buildMockUserAddress() throws Exception {
        final Constructor<UserAddress> constructor = UserAddress.class.getDeclaredConstructor(int.class, String.class, String.class,
            String.class, String.class, String.class, String.class, String.class, String.class, String.class, String.class, String.class,
            String.class, boolean.class, String.class, String.class);
        constructor.setAccessible(true);
        return constructor.newInstance(1, "firstName lastName", "address1", "address2", "address3", "address4", "address5",
            "administrativeArea", "locality", "countryCode", "postalCode", "sortingCode", "phoneNumber", false, "companyName", "emailAddress");
    }

    public static String buildAndroidPayEventResponseJson() {
        return "{" +
                "\"merchantName\": \"Merchant Name\"," +
                "\"pricingLineItems\": {\"subtotal\": \"5.23\", \"taxPrice\": \"1.23\", \"totalPrice\": \"6.46\", \"shippingPrice\": \"0.28\"}," +
                "\"currencyCode\": \"CAD\"," +
                "\"countryCode\": \"CA\"," +
                "\"requiresShipping\": true," +
                "\"shippingMethods\": [{\"Identifier\": \"Identifier\", \"Detail\": \"Detail\", \"Label\": \"Label\", \"Amount\": \"3.45\"}]" +
                "}";
    }

    public static MaskedWallet createMaskedWallet(final UserAddress shippingAddress, final UserAddress billingAddress, final String email,
                                                  final String googleTransactionId) throws Exception {
        final Constructor<MaskedWallet> maskedWalletConstructor = MaskedWallet.class.getDeclaredConstructor();
        maskedWalletConstructor.setAccessible(true);

        final Constructor<MaskedWallet.Builder> maskedWalletBuilderConstructor =
            (Constructor<MaskedWallet.Builder>) Class.forName("com.google.android.gms.wallet.MaskedWallet$Builder")
                .getDeclaredConstructor(MaskedWallet.class);
        maskedWalletBuilderConstructor.setAccessible(true);

        final MaskedWallet.Builder maskedWalletBuilder = maskedWalletBuilderConstructor.newInstance(maskedWalletConstructor.newInstance());
        maskedWalletBuilder.setBuyerShippingAddress(shippingAddress);
        maskedWalletBuilder.setBuyerBillingAddress(billingAddress);
        maskedWalletBuilder.setEmail(email);
        maskedWalletBuilder.setGoogleTransactionId(googleTransactionId);

        return maskedWalletBuilder.build();
    }

    public static MailingAddressInput mailingAddressEquals(MailingAddressInput expected) {
        return argThat(new MailingInputAddressMatcher(expected));
    }

    private static class MailingInputAddressMatcher implements ArgumentMatcher<MailingAddressInput> {
        private final MailingAddressInput expected;

        private MailingInputAddressMatcher(MailingAddressInput expected) {
            this.expected = expected;
        }

        @Override
        public boolean matches(MailingAddressInput mailingAddressInput) {
            return expected.toJsonString().equals(mailingAddressInput.toJsonString());
        }
    }
}
