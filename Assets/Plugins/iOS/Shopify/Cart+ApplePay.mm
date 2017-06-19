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
    
    NSString *dataKey = @"Items";
    NSString *itemsString = [NSString stringWithUTF8String:serializedSummaryItems];
    NSString *shippingString = [NSString stringWithUTF8String:serializedShippingMethods];
    
    /// Parse Summary Items
    NSError *error = nil;
    NSDictionary *summaryItemsJson = [NSJSONSerialization JSONObjectWithData:[itemsString dataUsingEncoding:NSUTF8StringEncoding]
                                                                     options:kNilOptions
                                                                       error:&error];
    
    NSArray *summaryItems;
    if (error == nil && summaryItemsJson != nil && [summaryItemsJson objectForKey:dataKey] != nil) {
        
        NSArray *summaryItemsArray = summaryItemsJson[dataKey];
        summaryItems = [PKPaymentSummaryItem deserializeWithSummaryItems:summaryItemsArray];
        
        if (summaryItems == nil) {
            return false;
        }
    } else {
        return false;
    }

    /// Parse Shipping Items
    NSDictionary *shippingItemsJson = [NSJSONSerialization JSONObjectWithData:[shippingString dataUsingEncoding:NSUTF8StringEncoding]
                                                                      options:kNilOptions
                                                                        error:&error];
    
    NSArray *shippingMethods;
    if (error == nil && shippingItemsJson != nil && [shippingItemsJson objectForKey:dataKey] != nil) {
        NSArray *shippingMethodsArray = shippingItemsJson[dataKey];
        shippingMethods = [PKShippingMethod deserializedWithShippingMethods:shippingMethodsArray];
    }

    dispatcher = [[ApplePayEventDispatcher alloc] initWithReceiver:[NSString stringWithUTF8String:unityDelegateObjectName]];
    
    _Session = [[PaymentSession alloc] initWithMerchantId:[NSString stringWithUTF8String:merchantID]
                                              countryCode:[NSString stringWithUTF8String:countryCode]
                                             currencyCode:[NSString stringWithUTF8String:currencyCode]
                           requiringShippingAddressFields:requiringShipping
                                             summaryItems:summaryItems
                                          shippingMethods:shippingMethods
                                           controllerType:PKPaymentAuthorizationViewController.class];
    
    _Session.delegate = dispatcher;
    
    return true;
}

void _PresentApplePayAuthorization() {
    [_Session presentAuthorizationController];
}
