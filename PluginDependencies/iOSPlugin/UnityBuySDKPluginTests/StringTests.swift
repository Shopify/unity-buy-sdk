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

@testable import UnityBuySDKPlugin

@available(iOS 10.0, *)
class StringTests : XCTestCase {
    
    /// Test that rawValues correspond to the init
    func testPaymentAuthorizationStatusInit() {
        XCTAssertEqual(PKPaymentAuthorizationStatus.failure, PKPaymentAuthorizationStatus(rawStringValue: "Failure"))
        XCTAssertEqual(PKPaymentAuthorizationStatus.success, PKPaymentAuthorizationStatus(rawStringValue: "Success"))
        XCTAssertEqual(PKPaymentAuthorizationStatus.invalidShippingContact, PKPaymentAuthorizationStatus(rawStringValue: "InvalidShippingContact"))
        XCTAssertEqual(PKPaymentAuthorizationStatus.invalidBillingPostalAddress, PKPaymentAuthorizationStatus(rawStringValue: "InvalidBillingPostalAddress"))
        XCTAssertEqual(PKPaymentAuthorizationStatus.invalidShippingPostalAddress, PKPaymentAuthorizationStatus(rawStringValue: "InvalidShippingPostalAddress"))
        
        if #available(iOS 9.2, *) {
            XCTAssertEqual(PKPaymentAuthorizationStatus.pinRequired, PKPaymentAuthorizationStatus(rawStringValue: "PinRequired"))
            XCTAssertEqual(PKPaymentAuthorizationStatus.pinIncorrect, PKPaymentAuthorizationStatus(rawStringValue: "PinIncorrect"))
            XCTAssertEqual(PKPaymentAuthorizationStatus.pinLockout, PKPaymentAuthorizationStatus(rawStringValue: "PinLockout"))
        }
    }
    
    func testPaymentSummaryItemTypeInit() {
        XCTAssertEqual(PKPaymentSummaryItemType.final, PKPaymentSummaryItemType(rawStringValue: "Final"))
        XCTAssertEqual(PKPaymentSummaryItemType.pending, PKPaymentSummaryItemType(rawStringValue: "Pending"))
    }
}

