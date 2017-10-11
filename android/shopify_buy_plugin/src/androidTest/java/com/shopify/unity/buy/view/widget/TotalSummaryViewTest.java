package com.shopify.unity.buy.view.widget;

import android.content.Context;
import android.support.test.InstrumentationRegistry;
import android.view.ContextThemeWrapper;
import android.view.LayoutInflater;

import com.shopify.unity.buy.R;
import com.shopify.unity.buy.utils.ForceLocaleRule;
import com.shopify.unity.buy.view.viewmodel.TotalSummaryViewModel;

import org.junit.Before;
import org.junit.ClassRule;
import org.junit.Test;

import java.math.BigDecimal;
import java.util.Locale;

import static org.junit.Assert.assertEquals;

/**
 * @author Flavio Faria
 */
public class TotalSummaryViewTest {

    @ClassRule
    public static ForceLocaleRule localeTestRule = new ForceLocaleRule(Locale.US);
    private Context themedContext;

    @Before
    public void setUp() throws Exception {
        Context context = InstrumentationRegistry.getContext();
        themedContext = new ContextThemeWrapper(context, R.style.BuyTheme);
    }

    @Test
    public void update() throws Exception {
        TotalSummaryView view = (TotalSummaryView) LayoutInflater
                .from(themedContext)
                .inflate(R.layout.view_confirmation_total_summary, null);

        TotalSummaryViewModel model = new TotalSummaryViewModel(
                new BigDecimal(21.48),
                new BigDecimal(3.20),
                new BigDecimal(2.32),
                new BigDecimal(27.00)
        );

        view.update(model);
        assertEquals(view.subtotal.getText(), "$21.48");
        assertEquals(view.shipping.getText(), "$3.20");
        assertEquals(view.tax.getText(), "$2.32");
        assertEquals(view.total.getText(), "Total: $27.00");
    }
}
