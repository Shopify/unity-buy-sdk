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
