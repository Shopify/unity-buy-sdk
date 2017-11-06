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
package com.shopify.unity.buy.androidpay.view.viewmodel;

import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import com.shopify.buy3.pay.PayCart;
import com.shopify.unity.buy.androidpay.view.widget.ConfirmationView;
import com.shopify.unity.buy.models.CheckoutInfo;
import com.shopify.unity.buy.models.ShippingMethod;

import java.math.BigDecimal;
import java.util.List;

import static java.lang.Math.max;

/**
 * View model class that is used to populate a {@link ConfirmationView}.
 */

public final class ConfirmationViewModel {

    /** Model used to populate subviews with price summary information. */
    public final TotalSummaryViewModel totalSummaryViewModel;
    /** Model used to populate subviews with shipping information. */
    public final ShippingRatesViewModel shippingRatesViewModel;

    public final boolean buttonEnabled;

    @Nullable
    public static ConfirmationViewModel of(@NonNull CheckoutInfo checkoutInfo,
                                           boolean buttonEnabled) {
        final PayCart payCart = checkoutInfo.getPayCart();
        if (payCart == null) {
            return null;
        }
        final TotalSummaryViewModel totalSummaryViewModel = new TotalSummaryViewModel(
                payCart.subtotal,
                payCart.shippingPrice != null ? payCart.shippingPrice : BigDecimal.ZERO,
                payCart.taxPrice != null ? payCart.taxPrice : BigDecimal.ZERO,
                payCart.totalPrice
        );

        final List<ShippingMethod> shippingMethods = checkoutInfo.getShippingMethods();
        final int selectedShippingMethodPosition = // If unknown, defaults to 0
                max(checkoutInfo.getCurrentShippingMethodPosition(), 0);
        final ShippingRatesViewModel shippingRatesViewModel =
                new ShippingRatesViewModel(shippingMethods, selectedShippingMethodPosition);

        return new ConfirmationViewModel(
                totalSummaryViewModel,
                shippingRatesViewModel,
                buttonEnabled
        );
    }

    private ConfirmationViewModel(@NonNull TotalSummaryViewModel totalSummaryViewModel,
                                 @NonNull ShippingRatesViewModel shippingRatesViewModel,
                                 boolean buttonEnabled) {

        this.totalSummaryViewModel = totalSummaryViewModel;
        this.shippingRatesViewModel = shippingRatesViewModel;
        this.buttonEnabled = buttonEnabled;
    }
}
