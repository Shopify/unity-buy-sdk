package com.shopify.unity.buy.models;

import android.os.Parcel;
import android.support.test.filters.SmallTest;
import android.support.test.runner.AndroidJUnit4;

import org.json.JSONObject;
import org.junit.Test;
import org.junit.runner.RunWith;

import java.math.BigDecimal;
import java.util.List;

import static org.junit.Assert.assertEquals;

/**
 * @author Flavio Faria
 */
@RunWith(AndroidJUnit4.class)
@SmallTest
public class ShippingMethodTest {

    private static final String JSON_STR = "{" +
            "\"Identifier\": \"Identifier\", " +
            "\"Detail\": \"Detail\", " +
            "\"Label\": \"Label\", " +
            "\"Amount\": \"3.95\"" +
            "}";

    @Test
    public void fromJsonString() throws Exception {
        ShippingMethod shippingMethod = ShippingMethod.fromJsonString(JSON_STR);
        assertEquals("Identifier", shippingMethod.identifier);
        assertEquals("Detail", shippingMethod.detail);
        assertEquals("Label", shippingMethod.label);
        assertEquals(BigDecimal.valueOf(3.95), shippingMethod.amount);
    }

    @Test
    public void fromJson() throws Exception {
        ShippingMethod shippingMethod = ShippingMethod.fromJson(new JSONObject(JSON_STR));
        assertEquals("Identifier", shippingMethod.identifier);
        assertEquals("Detail", shippingMethod.detail);
        assertEquals("Label", shippingMethod.label);
        assertEquals(BigDecimal.valueOf(3.95), shippingMethod.amount);
    }

    @Test
    public void listFromJsonString() throws Exception {
        String jsonArrayStr = "[" + JSON_STR + "]";
        List<ShippingMethod> shippingMethods = ShippingMethod.listFromJsonString(jsonArrayStr);
        assertEquals(1, shippingMethods.size());
        assertEquals("Identifier", shippingMethods.get(0).identifier);
        assertEquals("Detail", shippingMethods.get(0).detail);
        assertEquals("Label", shippingMethods.get(0).label);
        assertEquals(BigDecimal.valueOf(3.95), shippingMethods.get(0).amount);
    }

    @Test
    public void parcelable() {
        ShippingMethod input = new ShippingMethod("identifier", "detail",
                "label", BigDecimal.valueOf(0));

        Parcel parcel = Parcel.obtain();
        input.writeToParcel(parcel, input.describeContents());
        parcel.setDataPosition(0);

        ShippingMethod output = ShippingMethod.CREATOR.createFromParcel(parcel);
        assertEquals(output.identifier, "identifier");
        assertEquals(output.detail, "detail");
        assertEquals(output.label, "label");
        assertEquals(output.amount, BigDecimal.valueOf(0));
    }
}
