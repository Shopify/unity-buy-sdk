//
//  PaymentSessionTests.swift
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

class PaymentSessionTests: XCTestCase {

    let expectTimeout = 10.0
    
    override func tearDown() {
        if #available(iOS 10.0, *) {
            MockAuthorizationController.instances.removeAll()
        }
        super.tearDown()
    }
    
    // ----------------------------------
    //  MARK: - Init -
    //
    func testInit() {
        
        let summaryItems    = Models.createSummaryItems()
        let shippingMethods = Models.createShippingMethods()
        let session         = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: nil)
    
        XCTAssertEqual(session.request.merchantIdentifier,   Models.merchantId)
        XCTAssertEqual(session.request.countryCode,          Models.countryCode)
        XCTAssertEqual(session.request.currencyCode,         Models.currencyCode)
        XCTAssertEqual(session.request.supportedNetworks,    Models.supportedPaymentNetworks)
        XCTAssertEqual(session.request.paymentSummaryItems,  summaryItems)
        XCTAssertEqual(session.request.shippingMethods!,     shippingMethods)
        XCTAssertEqual(session.request.merchantCapabilities, PaymentSession.capabilities)
        XCTAssertEqual(session.request.requiredShippingAddressFields, .all)
        XCTAssertEqual(session.request.requiredBillingAddressFields,  .all)
        
        let noShippingSession = Models.createPaymentSession(requiringShippingAddressFields: false, usingNonDefault: nil)
        
        XCTAssertEqual(noShippingSession.request.requiredShippingAddressFields, PKAddressField(rawValue: PKAddressField.email.rawValue | PKAddressField.phone.rawValue))
    }
    
    // ----------------------------------
    //  MARK: - Presenting and Dismissing -
    //
    /// Tests that the shouldResignActive is correctly set according to the state of the Payment view controller
//    func testPresentAndDismissViewController() {
//
//        let session = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
//
//        let unityController = UIApplication.shared.delegate as! UnityBuyAppController
//        XCTAssertTrue(unityController.shouldResignActive)
//
//        session.presentAuthorizationController()
//        XCTAssertFalse(unityController.shouldResignActive)
//
//        MockAuthorizationController.invokeDidFinish()
//        XCTAssertTrue(unityController.shouldResignActive)
//    }
    
    // ----------------------------------
    //  MARK: - Select Shipping Method -
    //
    @available(iOS 11.0, *)
    func testSelectShippingMethod() {
        let shippingMethods = Models.createShippingMethods()
        var summaryItems    = Models.createSummaryItems()
        let paymentSession  = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        
        // Pretend that the user has chosen shipping method 2
        let selectedMethod   = shippingMethods[1]
        let completionStatus = PKPaymentAuthorizationStatus.success
        summaryItems[2] = PKPaymentSummaryItem.init(label: "Shipping", amount: selectedMethod.amount)
        
        let delegate = MockPaymentSessionDelegate()
        delegate.onSessionDidSelectShippingMethodUpdateRequest = { session, method, completion in
            
            XCTAssertEqual(paymentSession, session)
            XCTAssertEqual(selectedMethod, method)
            
            completion(PKPaymentRequestShippingMethodUpdate(paymentSummaryItems: summaryItems))
        }
        
        paymentSession.delegate = delegate
        paymentSession.presentAuthorizationController()
        
        let expectation = self.expectation(description: "MockAuthorizationController.invokeDidSelectShippingMethod failed to complete")
        
        MockAuthorizationController.invokeDidSelectShippingMethodHandler(selectedMethod) { requestUpdate in
            XCTAssertEqual(completionStatus, requestUpdate.status)
            XCTAssertEqual(summaryItems, requestUpdate.paymentSummaryItems)

            expectation.fulfill()
        }
        
        self.wait(for: [expectation], timeout: expectTimeout)
        
        // Cleanup any Unity State that was changed from displaying MockAuthorizationController
        MockAuthorizationController.invokeDidFinish();
    }
    
    // ----------------------------------
    //  MARK: - Select Shipping Contact -
    //
    @available(iOS 11.0, *)
    func testSelectShippingContact() {
        var shippingMethods = Models.createShippingMethods()
        let summaryItems    = Models.createSummaryItems()
        let paymentSession  = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        
        // Pretend user has selected this contact, causing changes in shipping
        let completionStatus = PKPaymentAuthorizationStatus.success
        let selectedContact = PKContact.init()
        selectedContact.emailAddress = "test@shopify.com"
        shippingMethods.remove(at: 0)
        
        let delegate = MockPaymentSessionDelegate()
        delegate.onSessionDidSelectShippingContact = { session, contact, completion in
            XCTAssertEqual(paymentSession, session)
            XCTAssertEqual(selectedContact, contact)
            completion(completionStatus, shippingMethods, summaryItems)
        }
        
        paymentSession.delegate = delegate
        paymentSession.presentAuthorizationController()
        
        let expectation = self.expectation(description: "MockAuthorizationController.invokeDidSelectShippingContact failed to complete")

        MockAuthorizationController.invokeDidSelectShippingContact(selectedContact) { (status, methods, items) in
            XCTAssertEqual(completionStatus, status)
            XCTAssertEqual(shippingMethods, methods)
            XCTAssertEqual(summaryItems, items)
            
            expectation.fulfill()
        }
        
        self.wait(for: [expectation], timeout: expectTimeout)
        
        // Cleanup any Unity State that was changed
        MockAuthorizationController.invokeDidFinish();
    }
    
    // ----------------------------------
    //  MARK: - Payment Authorization -
    //
    @available(iOS 11.0, *)
    func testPaymentAuthorization() {
        
        let paymentSession   = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let completionStatus = PKPaymentAuthorizationStatus.success
        let expectedPayment  = Models.createPayment()
        
        let delegate = MockPaymentSessionDelegate()
        delegate.onSessionDidAuthorizePayment = { session, payment, completion in
            XCTAssertEqual(paymentSession, session)
            XCTAssertEqual(expectedPayment, payment)
            completion(completionStatus)
        }
        
        paymentSession.delegate = delegate
        paymentSession.presentAuthorizationController()
        
        let expectation = self.expectation(description: "MockAuthorizationController.invokeDidAuthorizePayment failed to complete")
        
        MockAuthorizationController.invokeDidAuthorizePayment(expectedPayment) { (status: PKPaymentAuthorizationStatus) in
            XCTAssertEqual(completionStatus, status)
            expectation.fulfill()
        }
        
        self.wait(for: [expectation], timeout: expectTimeout)
        
        // Cleanup any Unity State that was changed
        MockAuthorizationController.invokeDidFinish();
    }
    
    // ----------------------------------
    //  MARK: - Payment Finishing -
    //
    
    /// Tests that the correct states are passed to the PaymentSessionDelegate after authorizing and finishing
    @available(iOS 11.0, *)
    func testPaymentFinishSuccess() {
        let paymentSession   = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let completionStatus = PKPaymentAuthorizationStatus.success
        let expectedPayment  = Models.createPayment()
        let paymentStatus    = PaymentStatus.success

        let authorizeExpectation = self.expectation(description: "MockAuthorizationController.invokeDidAuthorizePayment failed to complete")
        let finishExpectation    = self.expectation(description: "MockPaymentSessionDelegate.onSessionDidFinish failed to complete")
        
        let delegate = MockPaymentSessionDelegate()
        
        delegate.onSessionDidAuthorizePayment = { session, payment, completion in
            completion(completionStatus)
        }
        
        delegate.onSessionDidFinish = { session, status in
            XCTAssertEqual(paymentSession, session)
            XCTAssertEqual(paymentStatus, status)
            finishExpectation.fulfill()
        }
        
        paymentSession.delegate = delegate
        paymentSession.presentAuthorizationController()
        
        MockAuthorizationController.invokeDidAuthorizePayment(expectedPayment) { (status: PKPaymentAuthorizationStatus) in
            authorizeExpectation.fulfill()
        }
        
        self.wait(for: [authorizeExpectation], timeout: expectTimeout)
        
        MockAuthorizationController.invokeDidFinish();
        
        self.wait(for: [finishExpectation], timeout: expectTimeout)
    }
    
    /// Tests that the correct states are passed to the PaymentSessionDelegate after authorizing and finishing
    @available(iOS 11.0, *)
    func testPaymentFinishFailure() {
        let paymentSession   = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let completionStatus = PKPaymentAuthorizationStatus.failure
        let paymentStatus    = PaymentStatus.failed

        let finishExpectation    = self.expectation(description: "MockPaymentSessionDelegate.onSessionDidFinish failed to complete")
        
        let delegate = MockPaymentSessionDelegate()
        
        delegate.onSessionDidAuthorizePayment = { session, payment, completion in
            completion(completionStatus)
        }
        
        delegate.onSessionDidFinish = { session, status in
            XCTAssertEqual(paymentSession, session)
            XCTAssertEqual(paymentStatus, status)
            finishExpectation.fulfill()
        }
        
        paymentSession.delegate = delegate
        paymentSession.presentAuthorizationController()
        paymentSession.isAuthenticating = true
        
        MockAuthorizationController.invokeDidFinish();
        
        self.wait(for: [finishExpectation], timeout: expectTimeout)
    }
    
    /// Tests that the correct states are passed to the PaymentSessionDelegate after finishing without authorizing
    @available(iOS 11.0, *)
    func testPaymentFinishCancelled() {
        let paymentSession    = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let paymentStatus     = PaymentStatus.cancelled
        let finishExpectation = self.expectation(description: "MockPaymentSessionDelegate.onSessionDidFinish failed to complete")
        
        let delegate = MockPaymentSessionDelegate()
        
        delegate.onSessionDidFinish = { session, status in
            XCTAssertEqual(paymentSession, session)
            XCTAssertEqual(paymentStatus, status)
            finishExpectation.fulfill()
        }
        
        paymentSession.delegate = delegate
        paymentSession.presentAuthorizationController()
        
        MockAuthorizationController.invokeDidFinish();
        
        self.wait(for: [finishExpectation], timeout: expectTimeout)
    }
    
    /// Tests that the correct states are passed to the PaymentSessionDelegate after authorizing and finishing
    @available(iOS 11.0, *)
    func testPaymentFinishAuthorizedCancelled() {
        let paymentSession    = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let expectedPayment   = Models.createPayment()
        let paymentStatus     = PaymentStatus.cancelled
        let completionStatus  = PKPaymentAuthorizationStatus.invalidShippingPostalAddress
        
        let authorizeExpectation = self.expectation(description: "MockAuthorizationController.invokeDidAuthorizePayment failed to complete")
        let finishExpectation    = self.expectation(description: "MockPaymentSessionDelegate.onSessionDidFinish failed to complete")
        
        let delegate = MockPaymentSessionDelegate()
        
        delegate.onSessionDidAuthorizePayment = { session, payment, completion in
            completion(completionStatus)
        }
        
        delegate.onSessionDidFinish = { session, status in
            XCTAssertEqual(paymentSession, session)
            XCTAssertEqual(paymentStatus, status)
            finishExpectation.fulfill()
        }
        
        paymentSession.delegate = delegate
        paymentSession.presentAuthorizationController()
        
        MockAuthorizationController.invokeDidAuthorizePayment(expectedPayment) { (status: PKPaymentAuthorizationStatus) in
            authorizeExpectation.fulfill()
        }
        
        self.wait(for: [authorizeExpectation], timeout: expectTimeout)
        
        MockAuthorizationController.invokeDidFinish();
        
        self.wait(for: [finishExpectation], timeout: expectTimeout)
    }
}
