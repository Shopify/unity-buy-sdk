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
package com.shopify.unity.buy.androidpay.view.widget;

import android.content.Context;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.support.design.widget.BottomSheetDialog;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.Toolbar;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import com.shopify.unity.buy.R;
import com.shopify.unity.buy.models.ShippingMethod;

import java.text.NumberFormat;
import java.util.List;
import java.util.Locale;

/**
 * Bottom sheet that presents a list of available shipping methods during checkout confirmation.
 */
public final class ShippingMethodSelectDialog extends BottomSheetDialog {

    /** The {@link RecyclerView} that holds the list of available shipping methods. */
    private final RecyclerView recyclerView;

    public ShippingMethodSelectDialog(@NonNull Context context) {
        super(context, R.style.BuyTheme_ShippingMethodDialog);
        setContentView(R.layout.shipping_rate_list);
        final Toolbar toolbar = findViewById(R.id.toolbar);
        if (toolbar != null) {
            toolbar.setTitle(R.string.confirmation_shipping_method_select_title);
            toolbar.setNavigationIcon(R.drawable.ic_close);
            toolbar.setNavigationOnClickListener(new View.OnClickListener() {
                @Override public void onClick(View view) {
                    dismiss();
                }
            });
        }
        recyclerView = findViewById(R.id.list);
    }

    /**
     * Pops this bottom sheet into the screen.
     *
     * @param shippingMethods a list of {@link ShippingMethod ShippingMethods} to populate it
     * @param onShippingMethodSelectListener shipping method selection callback
     */
    public void show(@NonNull List<ShippingMethod> shippingMethods,
              @Nullable final OnShippingMethodSelectListener onShippingMethodSelectListener) {
        final ShippingMethodsAdapter adapter = new ShippingMethodsAdapter(shippingMethods,
                new OnShippingMethodSelectListener() {
                    @Override public void onShippingMethodSelected(ShippingMethod shippingMethod, int position) {
                        if (onShippingMethodSelectListener != null) {
                            onShippingMethodSelectListener.onShippingMethodSelected(shippingMethod, position);
                        }
                        dismiss();
                    }
                });
        recyclerView.setAdapter(adapter);
        show();
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        enableImmersiveMode();
    }

    @Override
    public void onWindowFocusChanged(boolean hasFocus) {
        super.onWindowFocusChanged(hasFocus);
        if (hasFocus) {
            enableImmersiveMode();
        }
    }

    /**
     * Hides the system UI components such as the tool bar and the status bar.
     */
    private void enableImmersiveMode() {
        getWindow().getDecorView().setSystemUiVisibility(
                View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                        | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                        | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                        | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                        | View.SYSTEM_UI_FLAG_FULLSCREEN
                        | View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
        );
    }

    /**
     * Adapter that populates the {@link RecyclerView} contained in this bottom sheet with a
     * list of available shipping methods.
     */
    private static final class ShippingMethodsAdapter
            extends RecyclerView.Adapter<ShippingMethodViewHolder> {

        /** List of available {@link ShippingMethod ShippingMethods} to be displayed. */
        @NonNull private final List<ShippingMethod> shippingMethods;
        /** Shipping method selection callback. */
        @NonNull private final OnShippingMethodSelectListener onShippingMethodSelectListener;

        ShippingMethodsAdapter(@NonNull List<ShippingMethod> shippingMethods,
                               @NonNull OnShippingMethodSelectListener onShippingMethodSelectListener) {
            this.shippingMethods = shippingMethods;
            this.onShippingMethodSelectListener = onShippingMethodSelectListener;
        }

        @Override
        public ShippingMethodViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            final View view = LayoutInflater
                    .from(parent.getContext())
                    .inflate(R.layout.shipping_rate_list_item, parent, false);

            final ShippingMethodViewHolder viewHolder = new ShippingMethodViewHolder(view);
            view.setOnClickListener(new View.OnClickListener() {
                @Override public void onClick(View view) {
                    final int position = viewHolder.getAdapterPosition();
                    onShippingMethodSelectListener.onShippingMethodSelected(getItem(position), position);
                }
            });
            return viewHolder;
        }

        @Override
        public void onBindViewHolder(ShippingMethodViewHolder holder, int position) {
            holder.bind(getItem(position));
        }

        @Override
        public int getItemCount() {
            return shippingMethods.size();
        }

        /**
         * Gets the {@link ShippingMethod} at a given {@code position}.
         *
         * @param position the {@code position} to get the shipping method from
         * @return a {@code ShippingMethod} object at {@code position}
         */
        private ShippingMethod getItem(int position) {
            return shippingMethods.get(position);
        }
    }

    /**
     * View holder class that wraps views which are rendered as item content for the
     * {@link RecyclerView}.
     */
    private static final class ShippingMethodViewHolder extends RecyclerView.ViewHolder {

        /** Used to format prices according to the current {@link Locale}. */
        static final NumberFormat CURRENCY_FORMAT = NumberFormat.getCurrencyInstance();

        /** Shipping method title view. */
        private TextView title;
        /** Shipping method price view. */
        private TextView price;

        ShippingMethodViewHolder(View itemView) {
            super(itemView);
            title = itemView.findViewById(R.id.title);
            price = itemView.findViewById(R.id.price);
        }

        /** Binds a given {@link ShippingMethod} to the views held by this view holder instance. */
        void bind(ShippingMethod shippingMethod) {
            title.setText(shippingMethod.label);
            price.setText(CURRENCY_FORMAT.format(shippingMethod.amount));
        }
    }

    /**
     * {@link ShippingMethod} selection callback interface.
     */
    public interface OnShippingMethodSelectListener {

        /**
         * Method that is run whenever the user selects a {@link ShippingMethod} from the
         * list contained by this bottom sheet.
         *
         * @param shippingMethod the selected {@code ShippingMethod}
         * @param position the position of {@code shippingMethod} in the list
         */
        void onShippingMethodSelected(ShippingMethod shippingMethod, int position);
    }
}
