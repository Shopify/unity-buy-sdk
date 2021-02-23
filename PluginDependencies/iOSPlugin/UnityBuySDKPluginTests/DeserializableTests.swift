//
//  DeserializableTests.swift
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
import PassKit

@testable import UnityBuySDKPlugin

class DeserializableTests: XCTestCase {
    
    let amount = "1.00"
    let label  = "total"
    let type   = PKPaymentSummaryItemType.final
    let detail = "10-15 Days"
    let identifier = "FreeShipping"
    
    let amountB = "1.00"
    let labelB  = "subtotal"
    let typeB   = PKPaymentSummaryItemType.final
    let detailB = "1-2 Days"
    let identifierB = "ExpressShipping"
    
    func testSummaryItemJson() {
        
        let json = Models.createSummaryItemJson(amount: amount, label: label, type: type)
        let jsonData   = try! JSONSerialization.data(withJSONObject: json)
        let jsonString = String.init(data: jsonData, encoding: .utf8)!
        let itemFromJson   = PKPaymentSummaryItem.deserialize(json)!
        let itemFromString = PKPaymentSummaryItem.deserialize(jsonString)!
        
        XCTAssertEqual(itemFromJson.amount, NSDecimalNumber(string: amount))
        XCTAssertEqual(itemFromJson.label,  label)
        XCTAssertEqual(itemFromJson.type,   type)
        XCTAssertEqual(itemFromJson, itemFromString)
    }
    
    func testSummaryItemJsonNoType() {
        
        let json = Models.createSummaryItemJson(amount: amount, label: label)
        let jsonData   = try! JSONSerialization.data(withJSONObject: json)
        let jsonString = String.init(data: jsonData, encoding: .utf8)!
        let itemFromJson   = PKPaymentSummaryItem.deserialize(json)!
        let itemFromString = PKPaymentSummaryItem.deserialize(jsonString)!

        XCTAssertEqual(itemFromJson.amount, NSDecimalNumber(string: amount))
        XCTAssertEqual(itemFromJson.label, label)
        /// When given no type the default is .final 
        /// https://developer.apple.com/documentation/passkit/pkpaymentsummaryitem/1619275-init
        XCTAssertEqual(itemFromJson.type, PKPaymentSummaryItemType.final)
        XCTAssertEqual(itemFromJson, itemFromString)
    }
    
     func testMultiSummaryItems() {
        
        let jsonA = Models.createSummaryItemJson(amount: amount, label: label, type: type)
        let jsonB = Models.createSummaryItemJson(amount: amountB, label: labelB, type: typeB)
        let jsonCollection = [jsonA, jsonB]
     
        let items = PKPaymentSummaryItem.deserialize(jsonCollection)!
        
        XCTAssertEqual(items.count, 2)
        XCTAssertEqual(items[0].amount, NSDecimalNumber(string: amount))
        XCTAssertEqual(items[0].label,  label)
        XCTAssertEqual(items[0].type,   type)
        
        XCTAssertEqual(items[1].amount, NSDecimalNumber(string: amountB))
        XCTAssertEqual(items[1].label,  labelB)
        XCTAssertEqual(items[1].type,   typeB)
    }
    
    func testShippingMethod() {

        let json   = Models.createShippingMethodJson(amount: amount, label: label, identifier: identifier, detail: detail, type: type)
        let jsonData   = try! JSONSerialization.data(withJSONObject: json)
        let jsonString = String.init(data: jsonData, encoding: .utf8)!
        let methodFromJson   = PKShippingMethod.deserialize(json)!
        let methodFromString = PKShippingMethod.deserialize(jsonString)!
        
        XCTAssertEqual(methodFromJson.amount,     NSDecimalNumber(string: amount))
        XCTAssertEqual(methodFromJson.label,      label)
        XCTAssertEqual(methodFromJson.type,       type)
        XCTAssertEqual(methodFromJson.identifier, identifier)
        XCTAssertEqual(methodFromJson.detail,     detail)
        XCTAssertEqual(methodFromJson, methodFromString)
    }
    
    func testShippingMethodNoType() {
        
        let json = Models.createShippingMethodJson(amount: amount, label: label, identifier: identifier, detail: detail)
        let jsonData   = try! JSONSerialization.data(withJSONObject: json)
        let jsonString = String.init(data: jsonData, encoding: .utf8)!
        let methodFromJson   = PKShippingMethod.deserialize(json)!
        let methodFromString = PKShippingMethod.deserialize(jsonString)!

        XCTAssertEqual(methodFromJson.amount,     NSDecimalNumber(string: amount))
        XCTAssertEqual(methodFromJson.label,      label)
        XCTAssertEqual(methodFromJson.identifier, identifier)
        XCTAssertEqual(methodFromJson.detail,     detail)
        /// When given no type the default is .final
        /// https://developer.apple.com/documentation/passkit/pkpaymentsummaryitem/1619275-init
        XCTAssertEqual(methodFromJson.type, PKPaymentSummaryItemType.final)
        XCTAssertEqual(methodFromJson, methodFromString)
    }
    
    func testMultiShippingMethods() {
        
        let jsonA = Models.createShippingMethodJson(amount: amount, label: label, identifier: identifier, detail: detail)
        let jsonB = Models.createShippingMethodJson(amount: amountB, label: labelB, identifier: identifierB, detail: detailB)
        let jsonCollection = [jsonA, jsonB]
        
        let items = PKShippingMethod.deserialize(jsonCollection)!
        
        XCTAssertEqual(items.count, 2)
        XCTAssertEqual(items[0].amount,     NSDecimalNumber(string: amount))
        XCTAssertEqual(items[0].label,      label)
        XCTAssertEqual(items[0].type,       type)
        XCTAssertEqual(items[0].identifier, identifier)
        XCTAssertEqual(items[0].detail,     detail)
        
        XCTAssertEqual(items[1].amount,     NSDecimalNumber(string: amountB))
        XCTAssertEqual(items[1].label,      labelB)
        XCTAssertEqual(items[1].type,       typeB)
        XCTAssertEqual(items[1].identifier, identifierB)
        XCTAssertEqual(items[1].detail,     detailB)
    }
}
