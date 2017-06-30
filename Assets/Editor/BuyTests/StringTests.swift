//
//  StringTests.swift
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

@testable import ProductName

@available(iOS 10.0, *)
class StringTests : XCTestCase {

    /// Test that all rawValues are unique
    func testPaymentAuthorizationStatusUnique() {
        
        let states: [String] = [
            PKPaymentAuthorizationStatus.failure.rawValue,
            PKPaymentAuthorizationStatus.success.rawValue,
            PKPaymentAuthorizationStatus.invalidBillingPostalAddress.rawValue,
            PKPaymentAuthorizationStatus.invalidShippingPostalAddress.rawValue,
            PKPaymentAuthorizationStatus.invalidShippingContact.rawValue,
            PKPaymentAuthorizationStatus.pinRequired.rawValue,
            PKPaymentAuthorizationStatus.pinLockout.rawValue,
            PKPaymentAuthorizationStatus.pinIncorrect.rawValue]
        
        XCTAssertEqual(states.count, Set(states).count)
    }
    
    func testPaymentSummaryItemTypeUnique() {
        let types: [String] = [PKPaymentSummaryItemType.pending.rawValue, PKPaymentSummaryItemType.final.rawValue]
        XCTAssertEqual(types.count, Set(types).count)
    }
    
    /// Test that rawValues correspond to the init
    func testPaymentAuthorizationStatusInit() {
        
        let assertInitIdempotency = { (status: PKPaymentAuthorizationStatus) in
            XCTAssertEqual(
                status,
                PKPaymentAuthorizationStatus(rawValue: status.rawValue as String),
                status.rawValue as String)
        }
        
        assertInitIdempotency(.failure)
        assertInitIdempotency(.success)
        assertInitIdempotency(.invalidBillingPostalAddress)
        assertInitIdempotency(.invalidShippingPostalAddress)
        assertInitIdempotency(.invalidShippingContact)
        assertInitIdempotency(.pinRequired)
        assertInitIdempotency(.pinLockout)
        assertInitIdempotency(.pinIncorrect)
    }
    
    func testPaymentSummaryItemTypeInit() {
        
        let assertInitIdempotency = { (status: PKPaymentSummaryItemType) in
            XCTAssertEqual(
                status,
                PKPaymentSummaryItemType(rawValue: status.rawValue as String),
                status.rawValue as String)
        }

        assertInitIdempotency(.pending)
        assertInitIdempotency(.final)
    }
}
