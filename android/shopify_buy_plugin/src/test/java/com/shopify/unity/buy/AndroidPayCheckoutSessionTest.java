package com.shopify.unity.buy;


import com.shopify.buy3.pay.PayCart;
import com.shopify.unity.buy.utils.JUnitLogger;

import org.json.JSONException;
import org.junit.Assert;
import org.junit.Test;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertNull;

public class AndroidPayCheckoutSessionTest {
    private static String merchantName = "My Store";
    private static String currencyCode = "CAD";
    private static String countryCode = "en_CA";
    private static String taxPrice = "1.00";
    private static String subtotal = "2.00";
    private static String totalPrice = "3.00";
    private static String shippingPrice = "4.00";

    @Test
    public void testCartFromUnityValidParams() throws JSONException {
        AndroidPayCheckoutSession session = new AndroidPayCheckoutSession(new JUnitLogger());

        List<String> summaryItems = new ArrayList<String>();
        summaryItems.add(summaryItem("TAXES", taxPrice));
        summaryItems.add(summaryItem("SUBTOTAL", subtotal));
        summaryItems.add(summaryItem("TOTAL", totalPrice));
        summaryItems.add(summaryItem("SHIPPING", shippingPrice));

        PayCart cart = session.cartFromUnity(merchantName, summaryItems.toString(), currencyCode, countryCode, true);

        assertNotNull(cart);

        assertEquals(cart.currencyCode, currencyCode);
        assertEquals(cart.merchantName, merchantName);
        assertEquals(cart.taxPrice, new BigDecimal(taxPrice));
        assertEquals(cart.subtotal, new BigDecimal(subtotal));
        assertEquals(cart.totalPrice, new BigDecimal(totalPrice));
        assertEquals(cart.shippingPrice, new BigDecimal(shippingPrice));
    }

    @Test(expected = JSONException.class)
    public void testCheckoutWithMissingSummaryItems() throws JSONException {
        AndroidPayCheckoutSession session = new AndroidPayCheckoutSession(new JUnitLogger());

        List<String> summaryItems = new ArrayList<String>();
        summaryItems.add(summaryItem("TAXES", taxPrice));
        summaryItems.add(summaryItem("SHIPPING", shippingPrice));

        session.cartFromUnity(merchantName, summaryItems.toString(), currencyCode, countryCode, true);
    }

    private String summaryItem(String label, String amount) {
        String format = "\"{\\\"Label\\\": \\\"%s\\\", \\\"Amount\\\": \\\"%s\\\"}\"";
        return String.format(Locale.CANADA, format, label, amount);
    }
}
