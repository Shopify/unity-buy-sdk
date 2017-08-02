package com.shopify.unity.buy;

import android.app.Fragment;
import android.os.Bundle;
import android.support.annotation.Nullable;

import com.shopify.buy3.pay.PayCart;

public class UnityAndroidPayFragment extends Fragment {
    private PayCart cart;
    private String unityDelegateObjectName;
    private String countryCode;

    private static final String EXTRA_UNITY_DELEGATE_OBJECT_NAME = "unityDelegateObjectName";
    private static final String EXTRA_PAY_CART = "payCart";
    private static final String EXTRA_COUNTRY_CODE = "countryCode";

    public static final UnityAndroidPayFragment newInstance(
        String unityDelegateObjectName,
        PayCart cart,
        String countryCode
    ) {

        UnityAndroidPayFragment fragment = new UnityAndroidPayFragment();
        Bundle bundle = new Bundle(3);
        bundle.putString(EXTRA_UNITY_DELEGATE_OBJECT_NAME, unityDelegateObjectName);
        bundle.putParcelable(EXTRA_PAY_CART, cart);
        bundle.putString(EXTRA_COUNTRY_CODE, countryCode);
        fragment.setArguments(bundle);
        return fragment;
    }

    @Override
    public void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Bundle bundle = getArguments();
        if (bundle != null) {
            unityDelegateObjectName = bundle.getString(EXTRA_UNITY_DELEGATE_OBJECT_NAME);
            cart = bundle.getParcelable(EXTRA_PAY_CART);
            countryCode = bundle.getString(EXTRA_COUNTRY_CODE);
        }

        // Don't recreate this fragment during configuration change.
        setRetainInstance(true);
    }
}
