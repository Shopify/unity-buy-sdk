package com.shopify.unity.buy.models;

import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import com.shopify.buy3.pay.PayCart;
import com.shopify.unity.buy.utils.Logger;

import java.util.ArrayList;
import java.util.List;

/**
 * @author Flavio Faria
 */

public final class CheckoutInfo {

    @Nullable private PayCart payCart;
    @NonNull private List<ShippingMethod> shippingMethods;
    private int currentShippingMethodPosition;

    public static CheckoutInfo fresh() {
        return new CheckoutInfo(null, new ArrayList<ShippingMethod>(), -1);
    }

    public static Builder from(@NonNull CheckoutInfo checkoutInfo) {
        return new Builder()
                .payCart(checkoutInfo.getPayCart())
                .shippingMethods(checkoutInfo.getShippingMethods())
                .currentShippingMethod(checkoutInfo.getCurrentShippingMethod());
    }

    private CheckoutInfo(@Nullable PayCart payCart,
                         @NonNull List<ShippingMethod> shippingMethods,
                         int currentShippingMethodPosition) {
        this.payCart = payCart;
        this.shippingMethods = shippingMethods;
        this.currentShippingMethodPosition = currentShippingMethodPosition;
    }

    @Nullable
    public PayCart getPayCart() {
        return payCart;
    }

    @NonNull
    public List<ShippingMethod> getShippingMethods() {
        return shippingMethods;
    }

    public int getCurrentShippingMethodPosition() {
        return currentShippingMethodPosition;
    }

    @Nullable
    public ShippingMethod getCurrentShippingMethod() {
        ShippingMethod shippingMethod = null;
        final int count = shippingMethods.size();
        if (currentShippingMethodPosition >= 0 && currentShippingMethodPosition < count) {
            shippingMethod = shippingMethods.get(currentShippingMethodPosition);
        }
        return shippingMethod;
    }


    public static class Builder {

        @Nullable private PayCart payCart;
        @NonNull private List<ShippingMethod> shippingMethods = new ArrayList<>();
        @Nullable private ShippingMethod currentShippingMethod;

        private Builder() {

        }

        public Builder payCart(@Nullable PayCart payCart) {
            this.payCart = payCart;
            return this;
        }

        public Builder shippingMethods(@NonNull List<ShippingMethod> shippingMethods) {
            this.shippingMethods = shippingMethods;
            return this;
        }

        public Builder currentShippingMethod(@Nullable ShippingMethod shippingMethod) {
            this.currentShippingMethod = shippingMethod;
            return this;
        }

        public CheckoutInfo build() {
            int position = -1;
            if (currentShippingMethod != null) {
                position = shippingMethods.indexOf(currentShippingMethod);
                if (position == -1) {
                    Logger.error("Shipping method not in list!");
                }
            }
            return new CheckoutInfo(payCart, shippingMethods, position);
        }
    }
}
