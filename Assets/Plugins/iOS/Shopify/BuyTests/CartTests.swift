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
@testable import ProductName

class CartTests: XCTestCase {
        
    func testCreatePaymentSession() {
        
        let itemLabel      = "SubTotal"
        let itemAmount     = "1.00"
        let shippingLabel  = "Free Shipping"
        let shippingAmount = "0"
        let identifier     = "FreeShipping"
        let detail         = "10-15 Days"
        
        let summaryItems        = [Models.createSummaryItemJson(amount: itemAmount, label: itemLabel)]
        let shippingMethods     = [Models.createShippingMethodJson(amount: shippingAmount, label: shippingLabel, identifier: identifier, detail: detail)]
        let summaryItemData     = try! JSONSerialization.data(withJSONObject: summaryItems)
        let shippingMethodData  = try! JSONSerialization.data(withJSONObject: shippingMethods)
        
        let serializedSummaryItems  = String.init(data: summaryItemData,    encoding: .utf8)!.cString(using: .utf8)
        let serializedShippingItems = String.init(data: shippingMethodData, encoding: .utf8)!.cString(using: .utf8)
        
        let merchantID        = "merchantID".cString(using: .utf8)
        let countryCode       = "US".cString(using: .utf8)
        let currencyCode      = "USD".cString(using: .utf8)
        let requiringShipping = true
        let unityObjectName   = "Tester".cString(using: .utf8)

        XCTAssertNil(Cart.session)
        _CreateApplePaySession(unityObjectName, merchantID, countryCode, currencyCode, serializedSummaryItems, serializedShippingItems, requiringShipping)
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
    }
}
