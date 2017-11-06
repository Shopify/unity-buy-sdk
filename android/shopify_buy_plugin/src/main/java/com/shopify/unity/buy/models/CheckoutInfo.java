package com.shopify.unity.buy.models;

import android.support.annotation.NonNull;
import android.support.annotation.Nullable;

import com.shopify.buy3.pay.PayCart;
import com.shopify.unity.buy.utils.Logger;

import java.util.ArrayList;
import java.util.List;

/**
 * Holds checkout state metadata, such as the current {@link PayCart} state,
 * the list of available {@link ShippingMethod ShippingMethods} for the current
 * checkout, as well as the currently selected {@code ShippingMethod}.
 */

public final class CheckoutInfo {

    /** Latest {@link PayCart} state. */
    @Nullable private PayCart payCart;
    /** Latest available {@link ShippingMethod ShippingMethods} */
    @NonNull private List<ShippingMethod> shippingMethods;
    /** Latest selected shipping method position. */
    private int currentShippingMethodPosition;

    /**
     * Creates a new, fresh {@link CheckoutInfo} object with empty state.
     *
     * @return the empty {@code CheckoutInfo} object
     */
    public static CheckoutInfo fresh() {
        return new CheckoutInfo(null, new ArrayList<ShippingMethod>(), -1);
    }

    /**
     * Creates a {@link Builder} from an existing {@link CheckoutInfo} object.
     *
     * @param checkoutInfo a {@code CheckoutInfo} object to copy fields from
     * @return a {@code Builder} with all fields copied from {@code checkoutInfo}
     */
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

    /**
     * @return the currently selected shipping method
     */
    @Nullable
    public ShippingMethod getCurrentShippingMethod() {
        ShippingMethod shippingMethod = null;
        final int count = shippingMethods.size();
        if (currentShippingMethodPosition >= 0 && currentShippingMethodPosition < count) {
            shippingMethod = shippingMethods.get(currentShippingMethodPosition);
        }
        return shippingMethod;
    }


    /**
     * Builder used to easily create {@link CheckoutInfo} objects by chaining
     * setter calls.
     */
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
