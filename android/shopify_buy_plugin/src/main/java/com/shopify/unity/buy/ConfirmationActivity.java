package com.shopify.unity.buy;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;

import com.shopify.buy3.pay.PayCart;
import com.shopify.unity.buy.view.viewmodel.ConfirmationViewModel;
import com.shopify.unity.buy.view.viewmodel.ShippingRatesViewModel;
import com.shopify.unity.buy.view.viewmodel.TotalSummaryViewModel;
import com.shopify.unity.buy.view.widget.ConfirmationView;

import java.math.BigDecimal;

/**
 * Activity that shows a checkout confirmation screen with broken down prices,
 * shipping and payment information.
 */

public final class ConfirmationActivity extends AppCompatActivity {

    /** {@link Intent} extra that holds the {@link PayCart} of the current checkout. */
    private static final String EXTRA_PAY_CART = "payCart";

    /**
     * Creates a new {@link Intent} that resolves to this {@link ConfirmationActivity}.
     *
     * @param context a {@link Context} used to create the {@code Intent}
     * @param payCart a {@link PayCart} used to populate the screen
     * @return an {@code Intent} that shows the {@code payCart} checkout information
     */
    static Intent newIntent(@NonNull Context context, @NonNull PayCart payCart) {
        return new Intent(context, ConfirmationActivity.class)
                .putExtra(EXTRA_PAY_CART, payCart);
    }

    /**
     * Gets the {@link PayCart} from the {@link Intent} that
     * started this {@link ConfirmationActivity}.
     *
     * @return a {@code PayCart} to populate the screen
     */
    private PayCart getPayCart() {
        return getIntent().getParcelableExtra(EXTRA_PAY_CART);
    }

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.view_confirmation);

        final Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        final ActionBar actionBar = getSupportActionBar();
        if (actionBar != null) {
            actionBar.setTitle(R.string.confirmation_title);
            actionBar.setHomeAsUpIndicator(R.drawable.ic_close);
            actionBar.setDisplayHomeAsUpEnabled(true);
        }

        final ConfirmationView confirmationView = findViewById(R.id.confirmation);
        confirmationView.update(confirmationViewModelFromPayCart(getPayCart()));
    }

    @Override
    public boolean onSupportNavigateUp() {
        finish();
        return super.onSupportNavigateUp();
    }

    /**
     * Creates a {@link ConfirmationViewModel} from a {@link PayCart} object.
     *
     * @param payCart the {@code PayCart} to read fields from
     * @return the {@code ConfirmationViewModel} based on the {@code payCart}
     */
    private ConfirmationViewModel confirmationViewModelFromPayCart(@NonNull PayCart payCart) {
        final TotalSummaryViewModel totalSummaryViewModel = totalSummaryViewModelFromPayCart(payCart);
        final ShippingRatesViewModel shippingRatesViewModel = shippingRatesViewModelFromPayCart(payCart);
        return new ConfirmationViewModel(totalSummaryViewModel, shippingRatesViewModel);
    }

    /**
     * Creates a {@link TotalSummaryViewModel} from a {@link PayCart} object.
     *
     * @param payCart the {@code PayCart} to read fields from
     * @return the {@code TotalSummaryViewModel} based on the {@code payCart}
     */
    private TotalSummaryViewModel totalSummaryViewModelFromPayCart(@NonNull PayCart payCart) {
        return new TotalSummaryViewModel(
                payCart.subtotal,
                nonNullShippingPrice(payCart),
                nonNullTaxPrice(payCart),
                payCart.totalPrice
        );
    }

    /**
     * Creates a {@link ShippingRatesViewModel} from a {@link PayCart} object.
     *
     * @param payCart the {@code PayCart} to read fields from
     * @return the {@code ShippingRatesViewModel} based on the {@code payCart}
     */
    private ShippingRatesViewModel shippingRatesViewModelFromPayCart(@NonNull PayCart payCart) {
        return new ShippingRatesViewModel(
                "Some shipping method", // TODO get this from Unity
                nonNullShippingPrice(payCart)
        );
    }

    /**
     * Extracts the {@code shippingPrice} field from the {@code payCart}.
     * If the field is {@code null}, this method will return {@code 0}.
     *
     * @param payCart the {@code payCart} to read the {@code shippingPrice} field from
     * @return a {@link BigDecimal} that represents the cart's {@code shippingPrice}
     */
    private BigDecimal nonNullShippingPrice(@NonNull PayCart payCart) {
        return payCart.shippingPrice != null ? payCart.shippingPrice : new BigDecimal(0);
    }

    /**
     * Extracts the {@code taxPrice} field from the {@code payCart}.
     * If the field is {@code null}, this method will return {@code 0}.
     *
     * @param payCart the {@code payCart} to read the {@code taxPrice} field from
     * @return a {@link BigDecimal} that represents the cart's {@code taxPrice}
     */
    private BigDecimal nonNullTaxPrice(@NonNull PayCart payCart) {
        return payCart.taxPrice != null ? payCart.taxPrice : new BigDecimal(0);
    }
}
