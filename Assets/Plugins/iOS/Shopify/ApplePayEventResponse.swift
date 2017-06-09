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
    
    /// Expect the response from events to follow [String: Any] JSON format.
    /// These are the keys that they are expected to follow
    enum ResponseKey: String {
        /// The PKPaymentAuthorizationStatus for the request
        case authorizationStatus = "AuthorizationStatus"
        /// The update summary items for the request
        case summaryItems = "SummaryItems"
        /// The updated shipping methods for the request
        case shippingMethods = "ShippingMethods"
    }

    let authorizationStatus: PKPaymentAuthorizationStatus?
    let summaryItems: [PKPaymentSummaryItem]?
    let shippingMethods: [PKShippingMethod]?

    // ----------------------------------
    //  MARK: - Init -
    //
    static func deserialize(_ json: JSON) -> ApplePayEventResponse? {
        
        var authorizationStatus: PKPaymentAuthorizationStatus? = nil
        var summaryItems: [PKPaymentSummaryItem]? = nil
        var shippingMethods: [PKShippingMethod]? = nil
        
        /// Parse Authorization Status
        if let authStatusString = json[ResponseKey.authorizationStatus.rawValue] as? String {
            authorizationStatus = PKPaymentAuthorizationStatus.from(authStatusString)
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
            
            guard let methods = PKShippingMethod.deserialize(shippingMethodsJsonObject) else {
                return nil
            }
            
            shippingMethods = methods
        }
        
        return self.init(authorizationStatus: authorizationStatus, summaryItems: summaryItems, shippingMethods: shippingMethods)
    }
}
