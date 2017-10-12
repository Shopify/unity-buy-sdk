package com.shopify.unity.buy.utils;

import android.content.res.Configuration;
import android.content.res.Resources;
import android.support.test.InstrumentationRegistry;

import org.junit.rules.TestRule;
import org.junit.runner.Description;
import org.junit.runners.model.Statement;

import java.util.Locale;

public class ForceLocaleRule implements TestRule {

    private final Locale testLocale;
    private Locale deviceLocale;

    public ForceLocaleRule(Locale testLocale) {
        this.testLocale = testLocale;
    }

    @Override
    public Statement apply(final Statement base, Description description) {
        return new Statement() {
            public void evaluate() throws Throwable {
                try {
                    if (testLocale != null) {
                        deviceLocale = Locale.getDefault();
                        setLocale(testLocale);
                    }

                    base.evaluate();
                } finally {
                    if (deviceLocale != null) {
                        setLocale(deviceLocale);
                    }
                }
            }
        };
    }

    private void setLocale(Locale locale) {
        Resources resources = InstrumentationRegistry.getTargetContext().getResources();
        Locale.setDefault(locale);
        Configuration config = resources.getConfiguration();
        config.locale = locale;
        resources.updateConfiguration(config, resources.getDisplayMetrics());
    }
}