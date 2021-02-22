//
//  ApplePayEventDispatcherTests.swift
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

class ApplePayEventDispatcherTests: XCTestCase {
    
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
        
        let setupMessage = UnityMessage(content: "", object: TesterName, method: TesterMethod.setupApplePayEventTest.rawValue)
        let messageExpectation = self.expectation(description: "MessageCenter.send(setupApplePayEventTest) failed to complete")
        MessageCenter.send(setupMessage) { response in
            messageExpectation.fulfill()
        }
        
        self.wait(for: [messageExpectation], timeout: timeout)
    }
    
    override func tearDown() {
        MockAuthorizationController.instances.removeAll()
        super.tearDown()
    }
    
    /// Tests that the correct serialized message was sent to Unity
    func testPaymentSessionFinish() {
        
        let session      = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let dispatcher   = ApplePayEventDispatcher(receiver:TesterName)
        session.delegate = dispatcher
        session.presentAuthorizationController()
        
        /// Since we did not authorize before invoking finish, we expect to send Unity a status of PaymentStatus.cancelled
        MockAuthorizationController.invokeDidFinish()
        assertLastMessageContentEqual(to: PaymentStatus.cancelled.description)
    }
    
    /// Tests that the correct serialized payment was sent to Unity, and the expected response was parsed correctly
    func testPaymentAuthorization() {
        
        let session      = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let dispatcher   = ApplePayEventDispatcher(receiver: TesterName)
        let payment      = Models.createPayment()
        session.delegate = dispatcher
        session.presentAuthorizationController()
        
        // Check that the proper response was received from the delegate
        // The value we expect to receive is defined in the Unity Tester object
        let authExpectation = self.expectation(description: "MockAuthorizationController.invokeDidAuthorizePayment failed to complete")
        MockAuthorizationController.invokeDidAuthorizePayment(payment) { (status: PKPaymentAuthorizationStatus) in
            XCTAssertEqual(.success, status)
            authExpectation.fulfill()
        }
        wait(for: [authExpectation], timeout: timeout)
        assertLastMessageContentEqual(to: try! payment.serializedString())
        
        MockAuthorizationController.invokeDidFinish()
        assertLastMessageContentEqual(to: PaymentStatus.success.description)
    }
    
    /// Tests that the correct serialized shipping method was sent to Unity, and the expected response was parsed correctly
    func testSelectShippingMethod() {
        
        let session      = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let dispatcher   = ApplePayEventDispatcher(receiver: TesterName)
        session.delegate = dispatcher
        session.presentAuthorizationController()
        
        // Check that the proper response was received from the delegate
        // The value we expect to receive is defined in the Unity Tester object
        let shippingMethods = Models.createShippingMethods()
        let selectedMethod  = shippingMethods[1]
        let expectation     = self.expectation(description: "MockAuthorizationController.invokeDidSelectShippingMethod failed to complete")
        let expectedItem    = Models.createSummaryItem()
        
        MockAuthorizationController.invokeDidSelectShippingMethod(selectedMethod) { status, items in
            XCTAssertEqual(.success, status)
            XCTAssertEqual(items.count, 1)
            XCTAssertEqual(expectedItem, items[0])
            expectation.fulfill()
        }
        wait(for: [expectation], timeout: timeout)
        
        assertLastMessageContentEqual(to: selectedMethod.identifier)
        MockAuthorizationController.invokeDidFinish()
    }
    
    /// Tests that the correct serialized shipping contact was sent to Unity, and the expected response was parsed correctly
    func testSelectShippingContact() {
    
        let session      = Models.createPaymentSession(requiringShippingAddressFields: true, usingNonDefault: MockAuthorizationController.self)
        let dispatcher   = ApplePayEventDispatcher(receiver: TesterName)
        session.delegate = dispatcher
        session.presentAuthorizationController()
        
        // Check that the proper response was received from the delegate
        // The value we expect to receive is defined in the Unity Tester object
        let selectedContact = Models.createContact(with: Models.createPostalAddress())
        let expectation     = self.expectation(description: "MockAuthorizationController.invokeDidSelectShippingContact failed to complete")
        let expectedMethod  = Models.createShippingMethod()
        let expectedItem    = Models.createSummaryItem()
        
        MockAuthorizationController.invokeDidSelectShippingContact(selectedContact) { status, methods, items in
            XCTAssertEqual(.success, status)
            XCTAssertEqual(items.count, 1)
            XCTAssertEqual(methods.count, 1)
            XCTAssertEqual(expectedItem, items[0])
            XCTAssertEqual(expectedMethod, methods[0])
            expectation.fulfill()
        }
        self.wait(for: [expectation], timeout: timeout)
        
        assertLastMessageContentEqual(to: try! selectedContact.serializedString())
        MockAuthorizationController.invokeDidFinish()
    }
}

extension ApplePayEventDispatcherTests {

    func assertLastMessageContentEqual(to content: String?) {
        let getLastMessage     = UnityMessage(content: "", object:  TesterName, method: TesterMethod.getLastMessage.rawValue)
        let messageExpectation = self.expectation(description: "MessageCenter.send(getLastMessage) failed to complete")
        MessageCenter.send(getLastMessage) { response in
            XCTAssertEqual(response, content)
            messageExpectation.fulfill()
        }
        
        self.wait(for: [messageExpectation], timeout: timeout)
    }
}
