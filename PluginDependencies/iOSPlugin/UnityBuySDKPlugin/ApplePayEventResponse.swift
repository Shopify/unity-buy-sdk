//
//  ApplePayEventResponse.swift
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

import Foundation
import PassKit

struct ApplePayEventResponse: Deserializable {

    let authorizationStatus: PKPaymentAuthorizationStatus?
    let summaryItems: [PKPaymentSummaryItem]?
    let shippingMethods: [PKShippingMethod]?
    
    fileprivate let _paymentErrors: [Error]?
    
    @available(iOS 11.0, *)
    var paymentErrors: [Error]? {
        return _paymentErrors
    }
    
    fileprivate init (status: PKPaymentAuthorizationStatus?, items: [PKPaymentSummaryItem]?, methods: [PKShippingMethod]?, paymentErrors: [Error]?) {
        authorizationStatus = status
        summaryItems = items
        shippingMethods = methods
        _paymentErrors = paymentErrors
    }
    
    // ----------------------------------
    //  MARK: - Init -
    //
    static func deserialize(_ json: JSON) -> ApplePayEventResponse? {
        
        var authorizationStatus: PKPaymentAuthorizationStatus? = nil
        var summaryItems: [PKPaymentSummaryItem]? = nil
        var shippingMethods: [PKShippingMethod]? = nil
        var paymentErrors: [Error]? = nil
        
        /// Parse Authorization Status
        if let authStatusString = json[ResponseKey.authorizationStatus.rawValue] as? String {
            authorizationStatus = PKPaymentAuthorizationStatus(rawStringValue: authStatusString)
        }
        
        /// Parse Summary Items
        if let summaryItemsJsonObject = json[ResponseKey.summaryItems.rawValue] as? [JSON] {
            
            guard let items = PKPaymentSummaryItem.deserialize(summaryItemsJsonObject) else {
                return nil
            }
            
            summaryItems = items
        }
        
        /// Parse Shipping Methods
        if let shippingMethodsJsonObject = json[ResponseKey.shippingMethods.rawValue] as? [JSON] {
            
            guard let methods = PKShippingMethod.deserializeShippingMethod(shippingMethodsJsonObject) else {
                return nil
            }
            
            shippingMethods = methods
        }
        
        /// Parse Payment Errors
        if #available(iOS 11.0, *) {
            
            if let paymentErrorJsonObject = json[ResponseKey.paymentErrors.rawValue] as? [JSON] {
                
                let errors = paymentErrorJsonObject.compactMap {
                    PKPaymentRequest.paymentError(with: $0)
                }
                
                assert(errors.count == paymentErrorJsonObject.count)
                paymentErrors = errors
            }
        }
        
        return self.init(status: authorizationStatus, items: summaryItems, methods: shippingMethods, paymentErrors: paymentErrors)
    }
}

extension ApplePayEventResponse {
    /// Expect the response from events to follow [String: Any] JSON format.
    /// These are the keys that they are expected to follow
    fileprivate enum ResponseKey: String {
        /// The PKPaymentAuthorizationStatus for the request
        case authorizationStatus = "AuthorizationStatus"
        /// The update summary items for the request
        case summaryItems = "SummaryItems"
        /// The updated shipping methods for the request
        case shippingMethods = "ShippingMethods"
        /// A list of PKErrors for the request. If there are none the list is empty
        case paymentErrors = "Errors"
    }
}
