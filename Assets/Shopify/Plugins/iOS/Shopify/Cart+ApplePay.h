//
//  Cart+ApplePay.h
//  UnityBuySDK
//
//  Created by Shopify.
//  Copyright © 2017 Shopify Inc. All rights reserved.
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

#ifndef Cart_ApplePay_h
#define Cart_ApplePay_h

#ifdef __cplusplus
extern "C" {
#endif
    bool _CreateApplePaySession(const char *unityDelegateObjectName, const char *merchantID, const char *countryCode, const char *currencyCode, const char *serializedPaymentNetworks, const char *serializedSummaryItems, const char *serializedShippingMethods, bool requiringShipping);
    bool _CanCheckoutWithApplePay(const char *serializedSupportedNetworks);
    bool _CanShowApplePaySetup(const char *serializedSupportedNetworks);
    void _ShowApplePaySetup();
    bool _PresentApplePayAuthorization();
#ifdef __cplusplus
}
#endif

#endif /* Cart_ApplePay_h */
