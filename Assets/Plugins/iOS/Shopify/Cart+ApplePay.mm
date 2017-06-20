//
//  Cart+ApplePay.mm
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

#import "Cart+ApplePay.h"
#import "ProductName-Swift.h"

ApplePayEventDispatcher *dispatcher;

NSArray *ItemsForString(NSString *jsonString) {
    NSString *dataKey = @"Items";
    NSError *error = nil;
    NSDictionary *itemsJson = [NSJSONSerialization JSONObjectWithData:[jsonString dataUsingEncoding:NSUTF8StringEncoding]
                                                              options:kNilOptions
                                                                error:&error];
    
    if (error == nil && itemsJson != nil && [itemsJson objectForKey:dataKey] != nil) {
        return itemsJson[dataKey];
    } else {
        return nil;
    }
}


bool _CanCheckoutWithApplePay() {
    return [PaymentSession canMakePayments];
}

bool _CanShowApplePaySetup() {
    return [PaymentSession canShowSetup];
}

void _ShowApplePaySetup() {
    [PaymentSession showSetup];
}

bool _CreateApplePaySession(const char *merchantID, const char *countryCode, const char *currencyCode, bool requiringShipping, const char *unityDelegateObjectName, const char *serializedSummaryItems, const char *serializedShippingMethods) {
    
    NSString *itemsString = [NSString stringWithUTF8String:serializedSummaryItems];
    NSArray *itemJsons = ItemsForString(itemsString);
    
    NSString *shippingString = [NSString stringWithUTF8String:serializedShippingMethods];
    NSArray *shippingJsons = ItemsForString(shippingString);
    
    if (itemJsons == nil) {
        return false;
    }
    
    NSArray *summaryItems = [PKPaymentSummaryItem deserializeWithSummaryItems:itemJsons];
    NSArray *shippingMethods;
    if (shippingJsons != nil) {
        shippingMethods = [PKShippingMethod deserializedWithShippingMethods:shippingJsons];
    }
    
    dispatcher = [[ApplePayEventDispatcher alloc] initWithReceiver:[NSString stringWithUTF8String:unityDelegateObjectName]];
    
    session = [[PaymentSession alloc] initWithMerchantId:[NSString stringWithUTF8String:merchantID]
                                              countryCode:[NSString stringWithUTF8String:countryCode]
                                             currencyCode:[NSString stringWithUTF8String:currencyCode]
                           requiringShippingAddressFields:requiringShipping
                                             summaryItems:summaryItems
                                          shippingMethods:shippingMethods
                                           controllerType:PKPaymentAuthorizationViewController.class];
    
    session.delegate = dispatcher;
    
    return true;
}

void _PresentApplePayAuthorization() {
    [session presentAuthorizationController];
}
