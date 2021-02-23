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

@objc public class Cart: NSObject {
    
    private(set) static var session: PaymentSession?
    private static var dispatcher: ApplePayEventDispatcher?
    
    @objc public static func createApplePaySession(unityDelegateObjectName: String, merchantID: String, countryCode: String, currencyCode: String, serializedSupportedNetworks: String, serializedSummaryItems: String, serializedShippingMethods: String?, requiringShipping: Bool) -> Bool {
        
        guard
            let summaryItemsJson  = extractItems(from: serializedSummaryItems),
            let summaryItems      = PKPaymentSummaryItem.deserialize(summaryItemsJson),
            let supportedNetworks = supportedPaymentNetworks(from: serializedSupportedNetworks)
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
        self.session = PaymentSession(
                        merchantId: merchantID,
                        countryCode: countryCode,
                        currencyCode: currencyCode,
                        requiringShippingAddressFields: requiringShipping,
                        supportedNetworks: supportedNetworks,
                        summaryItems: summaryItems,
                        shippingMethods: shippingMethods)
        
        self.session?.delegate = dispatcher
        
        return true
    }
    
    @objc public static func presentAuthorizationController() -> Bool {
        guard let session = session else {
            return false
        }
        
        session.presentAuthorizationController()
        return true
    }
}


//  ----------------------------------
//  MARK: - Static PaymentSession helpers -
//
extension Cart {
    @objc public static func canMakePayments(usingSerializedNetworks networks: String) -> Bool {
        
        if let supportedNetworks = supportedPaymentNetworks(from: networks) {
            return PaymentSession.canMakePayments(usingNetworks: supportedNetworks)
        }
        
        return false
    }
    
    @objc public static func canShowSetup(forSerializedNetworks networks: String) -> Bool {
        
        if let supportedNetworks = supportedPaymentNetworks(from: networks) {
            return PaymentSession.canShowSetup(forNetworks: supportedNetworks)
        }
        
        return false
    }
    
    @objc public static func showPaymentSetup() {
        PaymentSession.showSetup()
    }
}


// ----------------------------------
//  MARK: - String deserialization helpers -
//
extension Cart {
    
    fileprivate static func stringCollection(from jsonString: String) -> [String]? {
        
        if let data = jsonString.data(using: .utf8),
            let object = try? JSONSerialization.jsonObject(with: data),
            let stringCollection = object as? [String] {
            return stringCollection
        }
        
        return nil
        
    }
    
    fileprivate static func extractItems(from jsonString: String) -> [JSON]? {
        
        guard let stringCollection = stringCollection(from: jsonString) else {
            return nil
        }
        
        return stringCollection.compactMap { string in
            if let stringData  = string.data(using: .utf8),
                let itemObject = try? JSONSerialization.jsonObject(with: stringData),
                let itemJson   = itemObject as? JSON {
                return itemJson
            } else {
                return nil
            }
        }
    }
    
    fileprivate static func supportedPaymentNetworks(from jsonString: String) -> [PKPaymentNetwork]? {
        guard let stringCollection = stringCollection(from: jsonString) else {
            return nil
        }
        
        return stringCollection.compactMap { string in
            return PKPaymentNetwork(string)
        }
    }
}
