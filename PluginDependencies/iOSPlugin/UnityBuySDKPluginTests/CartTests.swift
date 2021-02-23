//
//  CartTests.swift
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

import XCTest
@testable import UnityBuySDKPlugin

class CartTests: XCTestCase {
        
    func testCreatePaymentSession() {
        
        let itemLabel      = "SubTotal"
        let itemAmount     = "1.00"
        let shippingLabel  = "Free Shipping"
        let shippingAmount = "0"
        let identifier     = "FreeShipping"
        let detail         = "10-15 Days"
        
        // Serialize a single summary item and shipping method
        let summaryItem         = Models.createSummaryItemJson(amount: itemAmount, label: itemLabel)
        let shippingMethod      = Models.createShippingMethodJson(amount: shippingAmount, label: shippingLabel, identifier: identifier, detail: detail)
        let summaryItemData     = try! JSONSerialization.data(withJSONObject: summaryItem)
        let shippingMethodData  = try! JSONSerialization.data(withJSONObject: shippingMethod)
        let serializedSummaryItem    = String(data: summaryItemData,    encoding: .utf8)!
        let serializedShippingMethod = String(data: shippingMethodData, encoding: .utf8)!
        
        // Put into an array of strings
        let summaryItemsData    = try! JSONSerialization.data(withJSONObject: [serializedSummaryItem])
        let shippingMethodsData = try! JSONSerialization.data(withJSONObject: [serializedShippingMethod])
        let serializedSummaryItems    = String(data: summaryItemsData,    encoding: .utf8)!.cString(using: .utf8)
        let serializedShippingMethods = String(data: shippingMethodsData, encoding: .utf8)!.cString(using: .utf8)
        
        let merchantID        = "merchantID".cString(using: .utf8)
        let countryCode       = "US".cString(using: .utf8)
        let currencyCode      = "USD".cString(using: .utf8)
        let requiringShipping = true
        let unityObjectName   = "Tester".cString(using: .utf8)
        
        let supportedPaymentNetworks = Models.createSerializedPaymentNetworksString()

        XCTAssertNil(Cart.session)
        _CreateApplePaySession(unityObjectName, merchantID, countryCode, currencyCode, supportedPaymentNetworks, serializedSummaryItems, serializedShippingMethods, requiringShipping)
        XCTAssertNotNil(Cart.session)
        
        let session = Cart.session!
        XCTAssertEqual(session.request.paymentSummaryItems.count, 1)
        XCTAssertEqual(session.request.paymentSummaryItems.first!.label, itemLabel)
        XCTAssertEqual(session.request.paymentSummaryItems.first!.amount, NSDecimalNumber(string: itemAmount))
        
        XCTAssertEqual(session.request.shippingMethods!.count, 1)
        XCTAssertEqual(session.request.shippingMethods!.first!.label,      shippingLabel)
        XCTAssertEqual(session.request.shippingMethods!.first!.amount,     NSDecimalNumber(string: shippingAmount))
        XCTAssertEqual(session.request.shippingMethods!.first!.identifier, identifier)
        XCTAssertEqual(session.request.shippingMethods!.first!.detail,     detail)
        
        XCTAssertEqual(session.request.supportedNetworks, Models.supportedPaymentNetworks)
    }
}
