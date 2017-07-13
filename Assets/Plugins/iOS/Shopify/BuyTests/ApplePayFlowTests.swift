//
//  ApplePayFlowTests.swift
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
class ApplePayFlowTests: XCTestCase {
    
    let timeout = 10.0
    
    override func setUp() {
        super.setUp()
        if Tester.hasLoaded == false {
            let didLoad = expectation(description: "Tester failed to load")
            Tester.completion = {
                didLoad.fulfill()
            }
            self.wait(for: [didLoad], timeout: timeout)
        }
    }
    
    override func tearDown() {
        MockAuthorizationController.instances.removeAll()
        super.tearDown()
    }
    
    /// Tests that selecting a shipping method returns the proper summary items for the checkout
    func testSelectShippingMethod() {
        
        let session      = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let dispatcher   = ApplePayEventDispatcher(receiver: Tester.name)
        session.delegate = dispatcher
        session.presentAuthorizationController()
        
        // Check that the proper response was received from the delegate
        // The label of the selected method is the actual shipping method identifier
        // used in the store
        let selectedMethod =
            PKShippingMethod(
                label: "Express",
                amount: NSDecimalNumber(value: 0.0),
                identifier: "canada_post-DOM.EP-8.44",
                detail: "10-15 Days");
        
        let expectation = self.expectation(description: "MockAuthorizationController.invokeDidSelectShippingMethod failed to complete")
        
        let expectedSubtotal = PKPaymentSummaryItem(label: "SUBTOTAL", amount: 25.47);
        let expectedShipping = PKPaymentSummaryItem(label: "SHIPPING", amount: 8.44);
        let expectedTaxes    = PKPaymentSummaryItem(label: "TAXES",    amount: 2.93);
        let expectedTotal    = PKPaymentSummaryItem(label: "TOTAL",    amount: 33.91);
        
        let method = Tester.Method.checkoutWithShippingAddress.rawValue;
        let checkoutMessage = UnityMessage(content: "", object: Tester.name, method: method)
        
        MessageCenter.send(checkoutMessage) { response in
            MockAuthorizationController.invokeDidSelectShippingMethod(selectedMethod) { status, items in
                
                XCTAssertEqual(status, PKPaymentAuthorizationStatus.success)
                XCTAssertEqual(items[0], expectedSubtotal)
                XCTAssertEqual(items[1], expectedShipping)
                XCTAssertEqual(items[2], expectedTaxes)
                XCTAssertEqual(items[3], expectedTotal)
                
                expectation.fulfill()
            }
        }
        
        self.wait(for: [expectation], timeout: timeout)
    }
}
