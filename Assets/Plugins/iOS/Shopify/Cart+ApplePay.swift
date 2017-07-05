//
//  Cart+ApplePay.swift
//  Unity-iPhone
//
//  Created by Shopify.
// 
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

import Foundation
import PassKit

@objc class Cart: NSObject {
    
    private(set) static var session: PaymentSession?
    private static var dispatcher: ApplePayEventDispatcher?
    
    static func createApplePaySession(unityDelegateObjectName: String, merchantID: String, countryCode: String, currencyCode: String, serializedSummaryItems: String, serializedShippingMethods: String?, requiringShipping: Bool) -> Bool {
        
        guard
            let summaryItemsJson = extractItems(from: serializedSummaryItems),
            let summaryItems     = PKPaymentSummaryItem.deserialize(summaryItemsJson)
        else {
            return false
        }
        
        var shippingMethods: [PKShippingMethod]?
        if let serializedShippingMethods = serializedShippingMethods {
            
            guard
                let shippingMethodsJson = extractItems(from: serializedShippingMethods),
                let methods             = PKShippingMethod.deserialize(shippingMethodsJson)
            else {
                return false
            }
            
            shippingMethods = methods
        }

        dispatcher = ApplePayEventDispatcher(receiver: unityDelegateObjectName)
        session    = PaymentSession(
                        merchantId: merchantID,
                        countryCode: countryCode,
                        currencyCode: currencyCode,
                        requiringShippingAddressFields: requiringShipping,
                        summaryItems: summaryItems,
                        shippingMethods: shippingMethods)
        
        session!.delegate = dispatcher
        
        return true
    }
    
    public static func presentAuthorizationController() -> Bool {
        guard let session = session else {
            return false
        }
        
        session.presentAuthorizationController()
        return true
    }
    
    private static func extractItems(from jsonString: String) -> [JSON]? {
        
        guard
            let data = jsonString.data(using: .utf8),
            let object = try? JSONSerialization.jsonObject(with: data),
            let stringCollection = object as? [String]
        else {
            return nil
        }
        
        return stringCollection.flatMap { string in
            if let stringData  = string.data(using: .utf8),
                let itemObject = try? JSONSerialization.jsonObject(with: stringData),
                let itemJson   = itemObject as? JSON {
                return itemJson
            } else {
                return nil
            }
        }
    }
}
