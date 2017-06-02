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

struct ApplePayEventResponse {
    
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
    init?(serialized: String) {

        guard
            let data = serialized.data(using: .utf8),
            let jsonObject = (try? JSONSerialization.jsonObject(with: data)) as? JSON else {
                return nil
        }

        /// Parse Authorization Status
        if let authStatusString = jsonObject[ResponseKey.authorizationStatus.rawValue] as? String {
            authorizationStatus = PKPaymentAuthorizationStatus.from(string: authStatusString)
        } else {
            authorizationStatus = nil
        }

        /// Parse Summary Items
        if let summaryItemsJsonObject = jsonObject[ResponseKey.summaryItems.rawValue] as? [JSON] {

            guard let items = PKPaymentSummaryItem.items(forSerializedSummaryItems: summaryItemsJsonObject) else {
                return nil
            }

            summaryItems = items
        } else {
            summaryItems = nil
        }
        
        /// Parse Shipping Methods
        if let shippingMethodsJsonObject = jsonObject[ResponseKey.shippingMethods.rawValue] as? [JSON] {
            
            guard let methods = PKShippingMethod.items(forSerializedShippingMethods: shippingMethodsJsonObject) else {
                return nil
            }
            
            shippingMethods = methods
        } else {
            shippingMethods = nil
        }
    }
}
