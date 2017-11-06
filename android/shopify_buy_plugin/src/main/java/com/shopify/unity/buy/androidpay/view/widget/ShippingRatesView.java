package com.shopify.unity.buy.androidpay.view.widget;

import android.content.Context;
import android.support.annotation.Nullable;
import android.support.annotation.VisibleForTesting;
import android.support.constraint.ConstraintLayout;
import android.util.AttributeSet;
import android.view.View;
import android.widget.TextView;

import com.shopify.unity.buy.R;
import com.shopify.unity.buy.androidpay.view.viewmodel.ShippingRatesViewModel;
import com.shopify.unity.buy.models.ShippingMethod;

import java.text.NumberFormat;

/**
 * Custom view that shows shipping information.
 */

public final class ShippingRatesView extends ConstraintLayout {

    /** Localized currency formatter. */
    private static final NumberFormat CURRENCY_FORMAT = NumberFormat.getCurrencyInstance();

    /** Shipping line description label. */
    private TextView shippingLineView;
    /** Shipping price label. */
    private TextView priceView;
    /** Change shipping method label. */
    private View changeView;

    public ShippingRatesView(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();
        shippingLineView = findViewById(R.id.shipping_line);
        priceView = findViewById(R.id.price);
        changeView = findViewById(R.id.change);
    }

    public void setListener(@Nullable final Listener listener) {
        if (listener != null) {
            changeView.setOnClickListener(new OnClickListener() {
                @Override public void onClick(View view) {
                    listener.onShippingRateChangeClick();
                }
            });
        } else {
            changeView.setOnClickListener(null);
        }
    }

    /**
     * Updates this view with the passed view model.
     *
     * @param shippingRatesViewModel the view model with data to render this view
     */
    public void update(ShippingRatesViewModel shippingRatesViewModel) {
        final ShippingMethod currentShippingMethod =
                shippingRatesViewModel.getCurrentShippingMethod();
        shippingLineView.setText(currentShippingMethod.label);
        priceView.setText(CURRENCY_FORMAT.format(currentShippingMethod.amount));
    }

    @VisibleForTesting
    TextView getShippingLineView() {
        return shippingLineView;
    }

    @VisibleForTesting
    TextView getPriceView() {
        return priceView;
    }

    /**
     * Listener interface to observe clicks on the shipping rate selection view.
     */
    public interface Listener {

        /**
         * Invoked when the user clicks on the shipping rate selection view.
         */
        void onShippingRateChangeClick();
    }
}
