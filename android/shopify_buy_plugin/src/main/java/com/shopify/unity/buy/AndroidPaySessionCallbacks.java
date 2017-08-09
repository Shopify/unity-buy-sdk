package com.shopify.unity.buy;

import com.shopify.unity.buy.models.MailingAddressInput;

interface AndroidPaySessionCallbacks {

    void onError(String errorMessage);

    void onCancel();

    void onUpdateShippingAddress(MailingAddressInput address, MessageCenter.MessageCallbacks messageCallbacks);
}
