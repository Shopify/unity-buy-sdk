package com.shopify.unity.buy.androidpay.view.viewmodel;

import android.support.annotation.NonNull;

import com.shopify.unity.buy.androidpay.view.widget.ShippingRatesView;
import com.shopify.unity.buy.models.ShippingMethod;

import java.util.List;

/**
 * View model class that is used to populate a {@link ShippingRatesView}.
 */

public final class ShippingRatesViewModel {

    /** All supported shipping methods. */
    public final List<ShippingMethod> shippingMethods;
    /** Current selected shipping method. */
    public final int currentPosition;

    public ShippingRatesViewModel(@NonNull List<ShippingMethod> shippingMethods,
                                  int currentPosition) {
        this.shippingMethods = shippingMethods;
        this.currentPosition = currentPosition;
    }

    /**
     * @return the currently selected {@link ShippingMethod}
     */
    public ShippingMethod getCurrentShippingMethod() {
        return shippingMethods.get(currentPosition);
    }
}
