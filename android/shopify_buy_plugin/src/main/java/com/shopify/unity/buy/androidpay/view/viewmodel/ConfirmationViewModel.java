package com.shopify.unity.buy.androidpay.view.viewmodel;

import android.support.annotation.NonNull;

import com.shopify.buy3.pay.PayCart;
import com.shopify.unity.buy.androidpay.view.widget.ConfirmationView;
import com.shopify.unity.buy.models.ShippingMethod;

import java.math.BigDecimal;
import java.util.List;

/**
 * View model class that is used to populate a {@link ConfirmationView}.
 */

public final class ConfirmationViewModel {

    /** Model used to populate subviews with price summary information. */
    public final TotalSummaryViewModel totalSummaryViewModel;
    /** Model used to populate subviews with shipping information. */
    public final ShippingRatesViewModel shippingRatesViewModel;

    public final boolean buttonEnabled;

    public static ConfirmationViewModel of(
            @NonNull PayCart payCart,
            @NonNull List<ShippingMethod> shippingMethods,
            boolean buttonEnabled) {
        final TotalSummaryViewModel totalSummaryViewModel = new TotalSummaryViewModel(
                payCart.subtotal,
                payCart.shippingPrice != null ? payCart.shippingPrice : BigDecimal.ZERO,
                payCart.taxPrice != null ? payCart.taxPrice : BigDecimal.ZERO,
                payCart.totalPrice
        );

        // TODO make sure the 0-indexed shipping method is the same one in the payCart
        final ShippingRatesViewModel shippingRatesViewModel =
                new ShippingRatesViewModel(shippingMethods, 0);

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
