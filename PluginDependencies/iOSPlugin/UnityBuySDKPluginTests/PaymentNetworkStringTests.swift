//
//  PaymentNetworkStringTests.swift
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
import XCTest

@testable import UnityBuySDKPlugin

class PaymentNetworkStringTests: XCTestCase {
    
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
    
    func testAmexDeserialization() {
        let method = TesterMethod.getAmexPaymentNetworkString
        let expectedNetwork = PKPaymentNetwork.amex
        
        let messageExpectation = expectation(description: "\(method.rawValue) failed to complete")
        
        callMethod(method) { response in
            XCTAssertEqual(PKPaymentNetwork(response!), expectedNetwork)
            messageExpectation.fulfill()
        }
        
        self.wait(for: [messageExpectation], timeout: timeout)
    }
    
    func testDiscoverDeserialization() {
        let method = TesterMethod.getDiscoverPaymentNetworkString
        let expectedNetwork = PKPaymentNetwork.discover
        
        let messageExpectation = expectation(description: "\(method.rawValue) failed to complete")
        
        callMethod(method) { response in
            XCTAssertEqual(PKPaymentNetwork(response!), expectedNetwork)
            messageExpectation.fulfill()
        }
        
        self.wait(for: [messageExpectation], timeout: timeout)
    }
    
    func testMastercardDeserialization() {
        let method = TesterMethod.getMasterCardPaymentNetworkString
        let expectedNetwork = PKPaymentNetwork.masterCard
        
        let messageExpectation = expectation(description: "\(method.rawValue) failed to complete")
        
        callMethod(method) { response in
            XCTAssertEqual(PKPaymentNetwork(response!), expectedNetwork)
            messageExpectation.fulfill()
        }
        
        self.wait(for: [messageExpectation], timeout: timeout)
    }

    func testVisaDeserialization() {
        let method = TesterMethod.getVisaPaymentNetworkString
        let expectedNetwork = PKPaymentNetwork.visa
        
        let messageExpectation = expectation(description: "\(method.rawValue) failed to complete")
        
        callMethod(method) { response in
            XCTAssertEqual(PKPaymentNetwork(response!), expectedNetwork)
            messageExpectation.fulfill()
        }
        
        self.wait(for: [messageExpectation], timeout: timeout)
    }
    
    @available(iOS 10.1, *)
    func testJCBDeserialization() {
        let method = TesterMethod.getJCBPaymentNetworkString
        let expectedNetwork = PKPaymentNetwork.JCB
        
        let messageExpectation = expectation(description: "\(method.rawValue) failed to complete")
        
        callMethod(method) { response in
            XCTAssertEqual(PKPaymentNetwork(response!), expectedNetwork)
            messageExpectation.fulfill()
        }
        
        self.wait(for: [messageExpectation], timeout: timeout)
    }
    
    @available(iOS 10.1, *)
    func testAllCardBrandDeserialization() {
        let method = TesterMethod.getAllCardBrandPaymentNetworksString
        
        let messageExpectation = expectation(description: "\(method.rawValue) failed to complete")
        
        callMethod(method) { response in
            
            /// Response consists of a serialized array defined in PaymentNetworkSerializationTester.cs containing
            /// the serialized CardBrands below. The serializer in Unity will skip CardBrands that don't have a
            /// PaymentNetwork equivalent when serializing from Unity.
            /// CardBrand.AMERICAN_EXPRESS
            /// CardBrand.DINERS_CLUB (Skipped)
            /// CardBrand.DISCOVER
            /// CardBrand.JCB
            /// CardBrand.MASTERCARD
            /// CardBrand.VISA
            /// CardBrand.UNKNOWN (Skipped)

            let data = response!.data(using: .utf8)!
            let paymentNetworks = try! JSONSerialization.jsonObject(with: data) as! [String]
            
            XCTAssertEqual(PKPaymentNetwork(paymentNetworks[0]), .amex )
            XCTAssertEqual(PKPaymentNetwork(paymentNetworks[1]), .discover)
            XCTAssertEqual(PKPaymentNetwork(paymentNetworks[2]), .JCB)
            XCTAssertEqual(PKPaymentNetwork(paymentNetworks[3]), .masterCard)
            XCTAssertEqual(PKPaymentNetwork(paymentNetworks[4]), .visa)
            XCTAssertEqual(paymentNetworks.count, 5)
            
            messageExpectation.fulfill()
        }
        
        self.wait(for: [messageExpectation], timeout: timeout)
    }
}

// ----------------------------------
//  MARK: - Helpers -
//
extension PaymentNetworkStringTests {
    fileprivate func callMethod(_ method: TesterMethod, completion: @escaping UnityMessage.MessageCompletion) {
        let message = UnityMessage(content: "", object: TesterName, method: method.rawValue)
        MessageCenter.send(message, completionHandler: completion)
    }
}
