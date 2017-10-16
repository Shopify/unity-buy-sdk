package com.shopify.unity.buy.androidpay.view.widget;

/**
 * Defines that a view consumes an object that is used to update the view state.
 */

public interface Updatable<T> {

    /**
     * Updates the current view state.
     * @param t the data object used to populate the current view
     */
    void update(T t);
}
