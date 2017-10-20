package com.shopify.unity.buy.androidpay.view.widget;

import android.content.Context;
import android.support.annotation.VisibleForTesting;
import android.support.constraint.ConstraintLayout;
import android.util.AttributeSet;
import android.view.View;
import android.widget.TextView;

import com.shopify.unity.buy.R;
import com.shopify.unity.buy.androidpay.view.viewmodel.ShippingRatesViewModel;
import com.shopify.unity.buy.models.ShippingMethod;

import java.text.NumberFormat;

import static com.shopify.unity.buy.androidpay.view.widget.ShippingMethodSelectDialog.OnShippingMethodSelectListener;

/**
 * Custom view that shows shipping information.
 */

public final class ShippingRatesView extends ConstraintLayout {

    /** Localized currency formatter. */
    private static final NumberFormat CURRENCY_FORMAT = NumberFormat.getCurrencyInstance();

    /** Shipping line description label. */
    private TextView shippingLine;
    /** Shipping price label. */
    private TextView price;

    private ShippingRatesViewModel viewModel;

    public ShippingRatesView(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();
        shippingLine = findViewById(R.id.shipping_line);
        price = findViewById(R.id.price);
        findViewById(R.id.change).setOnClickListener(new OnClickListener() {
            @Override public void onClick(View view) {
                onChangeClick();
            }
        });
    }

    public void update(ShippingRatesViewModel shippingRatesViewModel) {
        viewModel = shippingRatesViewModel;
        final ShippingMethod currentShippingMethod =
                shippingRatesViewModel.getCurrentShippingMethod();
        shippingLine.setText(currentShippingMethod.label);
        price.setText(CURRENCY_FORMAT.format(currentShippingMethod.amount));
    }

    @VisibleForTesting
    TextView getShippingLine() {
        return shippingLine;
    }

    @VisibleForTesting
    TextView getPrice() {
        return price;
    }

    private void onChangeClick() {
        new ShippingMethodSelectDialog(getContext()).show(viewModel.shippingMethods,
                new OnShippingMethodSelectListener() {
                    @Override
                    public void onShippingMethodSelected(ShippingMethod shippingMethod, int position) {
                        update(new ShippingRatesViewModel(viewModel.shippingMethods, position));
                    }
                });
        // TODO show shipping method selection dialog
    }
}
