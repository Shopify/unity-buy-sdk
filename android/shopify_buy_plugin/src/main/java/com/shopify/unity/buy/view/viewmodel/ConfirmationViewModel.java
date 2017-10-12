package com.shopify.unity.buy.view.viewmodel;

import android.support.annotation.NonNull;

import com.shopify.unity.buy.view.widget.ConfirmationView;

/**
 * View model class that is used to populate a {@link ConfirmationView}.
 */

public final class ConfirmationViewModel {

    /** Model used to populate subviews with price summary information. */
    public final TotalSummaryViewModel totalSummaryViewModel;
    /** Model used to populate subviews with shipping information. */
    public final ShippingRatesViewModel shippingRatesViewModel;

    public ConfirmationViewModel(@NonNull TotalSummaryViewModel totalSummaryViewModel,
                                 @NonNull ShippingRatesViewModel shippingRatesViewModel) {

        this.totalSummaryViewModel = totalSummaryViewModel;
        this.shippingRatesViewModel = shippingRatesViewModel;
    }
}
