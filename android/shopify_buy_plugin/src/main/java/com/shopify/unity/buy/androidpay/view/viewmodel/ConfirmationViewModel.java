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
