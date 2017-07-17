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
    
    private let timeout = 10.0
    
    // These are the shipping methods expected from unity-buy-sdk.myshopify.com
    private let expeditedShippingMethod    = PKShippingMethod(label: "Expedited Parcel", amount: 8.44,  identifier: "canada_post-DOM.EP-8.44")
    private let xpressPostShippingMethod   = PKShippingMethod(label: "Xpresspost",       amount: 10.37, identifier: "canada_post-DOM.XP-10.37")
    private let priorityPostShippingMethod = PKShippingMethod(label: "Priority",         amount: 18,    identifier: "canada_post-DOM.PC-18.00")
    
    // These are part of the summary items that are expected from unity-buy-sdk.myshopify.com after adding
    // the product name "[Test] Ballooning Around Shirt" to the Cart
    
    private let expectedSubtotal = PKPaymentSummaryItem(label: "SUBTOTAL", amount: 25.47)
    private let expectedTaxes    = PKPaymentSummaryItem(label: "TAXES",    amount: 2.93)

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
    
    /// Tests that selecting a shipping contact returns the proper summary items and shipping methods for the checkout
    func testSetShippingContact() {
        
        let session      = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let dispatcher   = ApplePayEventDispatcher(receiver: Tester.name)
        session.delegate = dispatcher
        session.presentAuthorizationController()
        
        let setContact           = PKContact()
        setContact.postalAddress = Models.createPartialPostalAddress()
        
        let expectation      = self.expectation(description: "MockAuthorizationController.invokeDidSelectShippingContact failed to complete")
        let expectedSubtotal = PKPaymentSummaryItem(label: "SUBTOTAL", amount: 25.47)
        let expectedTaxes    = PKPaymentSummaryItem(label: "TAXES",    amount: 2.93)
        let expectedTotal    = PKPaymentSummaryItem(label: "TOTAL", amount: 25.47)
        
        let method = Tester.Method.checkout.rawValue
        let checkoutMessage = UnityMessage(content: "", object: Tester.name, method: method)
        
        MessageCenter.send(checkoutMessage) { response in
            MockAuthorizationController.invokeDidSelectShippingContact(setContact) { status, methods, items in
                
                XCTAssertEqual(status, PKPaymentAuthorizationStatus.success)
                XCTAssertEqual(items[0], expectedSubtotal)
                XCTAssertEqual(items[1], expectedTaxes)
                XCTAssertEqual(items[2], expectedTotal)
                
                XCTAssertEqual(methods[0], self.expeditedShippingMethod)
                XCTAssertEqual(methods[1], self.xpressPostShippingMethod)
                XCTAssertEqual(methods[2], self.priorityPostShippingMethod)
                
                expectation.fulfill()
            }
        }
        
        self.wait(for: [expectation], timeout: timeout)
    }
    
    /// Tests that setting an invalid shipping address returns the proper summary items and shipping methods for the checkout
    func testSetInvalidShippingContact() {
        
        let session      = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let dispatcher   = ApplePayEventDispatcher(receiver: Tester.name)
        session.delegate = dispatcher
        session.presentAuthorizationController()
        
        let setContact     = PKContact()
        let postalAddress  = Models.createPartialPostalAddress() as! CNMutablePostalAddress
        postalAddress.country = "Incorrect_country"
        setContact.postalAddress = postalAddress
        
        let expectation      = self.expectation(description: "MockAuthorizationController.invokeDidSelectShippingContact failed to complete")
        let expectedSubtotal = PKPaymentSummaryItem(label: "SUBTOTAL", amount: 25.47)
        let expectedTaxes    = PKPaymentSummaryItem(label: "TAXES",    amount: 0)
        let expectedTotal    = PKPaymentSummaryItem(label: "TOTAL", amount: 25.47)
        
        let method = Tester.Method.checkout.rawValue
        let checkoutMessage = UnityMessage(content: "", object: Tester.name, method: method)
        
        MessageCenter.send(checkoutMessage) { response in
            MockAuthorizationController.invokeDidSelectShippingContact(setContact) { status, methods, items in
                
                XCTAssertEqual(status, PKPaymentAuthorizationStatus.invalidShippingPostalAddress)
                XCTAssertEqual(items[0], expectedSubtotal)
                XCTAssertEqual(items[1], expectedTaxes)
                XCTAssertEqual(items[2], expectedTotal)
                
                XCTAssertEqual(methods.count, 0)
                
                expectation.fulfill()
            }
        }
        
        self.wait(for: [expectation], timeout: timeout)
    }
    
    /// Tests that selecting a shipping method returns the proper summary items for the checkout
    func testSelectShippingMethod() {
        
        let session      = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let dispatcher   = ApplePayEventDispatcher(receiver: Tester.name)
        session.delegate = dispatcher
        session.presentAuthorizationController()
        
        let selectedMethod = expeditedShippingMethod

        let expectation      = self.expectation(description: "MockAuthorizationController.invokeDidSelectShippingMethod failed to complete")
        let expectedSubtotal = PKPaymentSummaryItem(label: "SUBTOTAL", amount: 25.47)
        let expectedShipping = PKPaymentSummaryItem(label: "SHIPPING", amount: 8.44)
        let expectedTaxes    = PKPaymentSummaryItem(label: "TAXES",    amount: 2.93)
        let expectedTotal    = PKPaymentSummaryItem(label: "TOTAL",    amount: 33.91)
        
        let method = Tester.Method.checkoutWithShippingAddress.rawValue
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
    
    
    /// Tests that we can checkout with valid information
    func testCompleteSuccessfulCheckout() {
        
        let session      = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let dispatcher   = ApplePayEventDispatcher(receiver: Tester.name)
        session.delegate = dispatcher
        session.presentAuthorizationController()
        
        let selectedMethod = expeditedShippingMethod
        let payment =
            MockPayment(
                token: Models.createSimulatorPaymentToken() as! MockPaymentToken,
                billingContact: Models.createContact(with: Models.createPostalAddress()),
                shippingContact: Models.createContact(with: Models.createPostalAddress()),
                shippingMethod: selectedMethod)
        
        let selectShippingExpectation   = self.expectation(description: "MockAuthorizationController.invokeDidSelectShippingMethod failed to complete")
        let authorizePaymentExpectation = self.expectation(description: "MockAuthorizationController.invokeDidAuthorizePayment failed to complete")
        
        let method = Tester.Method.checkoutWithShippingAddress.rawValue
        let checkoutMessage = UnityMessage(content: "", object: Tester.name, method: method)
        
        MessageCenter.send(checkoutMessage) { response in
            MockAuthorizationController.invokeDidSelectShippingMethod(selectedMethod) { status, items in
                selectShippingExpectation.fulfill()
                MockAuthorizationController.invokeDidAuthorizePayment(payment, completion: { authStatus in
                    XCTAssertEqual(authStatus, PKPaymentAuthorizationStatus.success)
                    authorizePaymentExpectation.fulfill()
                })
            }
        }
        
        self.wait(for: [selectShippingExpectation, authorizePaymentExpectation], timeout: timeout)
    }
    
    /// Tests that we receive the correct error for incorrect shipping contact
    func testCompleteInvalidShippingContactCheckout() {
        
        let session      = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let dispatcher   = ApplePayEventDispatcher(receiver: Tester.name)
        session.delegate = dispatcher
        session.presentAuthorizationController()
        
        let selectedMethod = expeditedShippingMethod
        
        let incorrectContact = Models.createContact(with: Models.createPostalAddress())
        incorrectContact.emailAddress = "incorrectEmail"
        
        let payment =
            MockPayment(
                token: Models.createSimulatorPaymentToken() as! MockPaymentToken,
                billingContact: Models.createContact(with: Models.createPostalAddress()),
                shippingContact: incorrectContact,
                shippingMethod: selectedMethod)
        
        let selectShippingExpectation   = self.expectation(description: "MockAuthorizationController.invokeDidSelectShippingMethod failed to complete")
        let authorizePaymentExpectation = self.expectation(description: "MockAuthorizationController.invokeDidAuthorizePayment failed to complete")
        
        let method = Tester.Method.checkoutWithShippingAddress.rawValue
        let checkoutMessage = UnityMessage(content: "", object: Tester.name, method: method)
        
        MessageCenter.send(checkoutMessage) { response in
            MockAuthorizationController.invokeDidSelectShippingMethod(selectedMethod) { status, items in
                selectShippingExpectation.fulfill()
                MockAuthorizationController.invokeDidAuthorizePayment(payment, completion: { authStatus in
                    XCTAssertEqual(authStatus, PKPaymentAuthorizationStatus.invalidShippingContact)
                    authorizePaymentExpectation.fulfill()
                })
            }
        }
        
        self.wait(for: [selectShippingExpectation, authorizePaymentExpectation], timeout: timeout)
    }
    
    /// Tests that we receive the correct error for incorrect billing address
    func testCompleteInvalidBillingAddressCheckout() {
        
        let session      = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let dispatcher   = ApplePayEventDispatcher(receiver: Tester.name)
        session.delegate = dispatcher
        session.presentAuthorizationController()
        
        let selectedMethod = expeditedShippingMethod
        
        let incorrectPostal  = Models.createPostalAddress() as! CNMutablePostalAddress
        incorrectPostal.country = "Incorrect_country"
        
        let incorrectContact = Models.createContact(with: Models.createPostalAddress())
        incorrectContact.postalAddress = incorrectPostal
        
        let payment =
            MockPayment(
                token: Models.createSimulatorPaymentToken() as! MockPaymentToken,
                billingContact: incorrectContact,
                shippingContact: Models.createContact(with: Models.createPostalAddress()),
                shippingMethod: selectedMethod)
        
        let selectShippingExpectation   = self.expectation(description: "MockAuthorizationController.invokeDidSelectShippingMethod failed to complete")
        let authorizePaymentExpectation = self.expectation(description: "MockAuthorizationController.invokeDidAuthorizePayment failed to complete")
        
        let method = Tester.Method.checkoutWithShippingAddress.rawValue
        let checkoutMessage = UnityMessage(content: "", object: Tester.name, method: method)
        
        MessageCenter.send(checkoutMessage) { response in
            MockAuthorizationController.invokeDidSelectShippingMethod(selectedMethod) { status, items in
                selectShippingExpectation.fulfill()
                MockAuthorizationController.invokeDidAuthorizePayment(payment, completion: { authStatus in
                    XCTAssertEqual(authStatus, PKPaymentAuthorizationStatus.invalidBillingPostalAddress)
                    authorizePaymentExpectation.fulfill()
                })
            }
        }
        
        self.wait(for: [selectShippingExpectation, authorizePaymentExpectation], timeout: timeout)
    }
    
    /// Tests that we receive the correct error for incorrect shipping address
    func testCompleteInvalidShippingAddressCheckout() {
        
        let session      = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let dispatcher   = ApplePayEventDispatcher(receiver: Tester.name)
        session.delegate = dispatcher
        session.presentAuthorizationController()
        
        let selectedMethod = expeditedShippingMethod
        
        let incorrectPostal = Models.createPostalAddress() as! CNMutablePostalAddress
        incorrectPostal.country = "Incorrect_country"
        
        let incorrectContact = Models.createContact(with: Models.createPostalAddress())
        incorrectContact.postalAddress = incorrectPostal
        
        let payment =
            MockPayment(
                token: Models.createSimulatorPaymentToken() as! MockPaymentToken,
                billingContact: Models.createContact(with: Models.createPostalAddress()),
                shippingContact: incorrectContact,
                shippingMethod: selectedMethod)
        
        let selectShippingExpectation   = self.expectation(description: "MockAuthorizationController.invokeDidSelectShippingMethod failed to complete")
        let authorizePaymentExpectation = self.expectation(description: "MockAuthorizationController.invokeDidAuthorizePayment failed to complete")
        
        let method = Tester.Method.checkoutWithShippingAddress.rawValue
        let checkoutMessage = UnityMessage(content: "", object: Tester.name, method: method)
        
        MessageCenter.send(checkoutMessage) { response in
            MockAuthorizationController.invokeDidSelectShippingMethod(selectedMethod) { status, items in
                selectShippingExpectation.fulfill()
                MockAuthorizationController.invokeDidAuthorizePayment(payment, completion: { authStatus in
                    XCTAssertEqual(authStatus, PKPaymentAuthorizationStatus.invalidShippingPostalAddress)
                    authorizePaymentExpectation.fulfill()
                })
            }
        }
        
        self.wait(for: [selectShippingExpectation, authorizePaymentExpectation], timeout: timeout)
    }
}
