//
//  ApplePayErrorDeserializingTests.swift
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

@available(iOS 11.0, *)
class ApplePayErrorDeserializingTests: XCTestCase {
    
    fileprivate let timeout = 10.0
    fileprivate let expectedDescription = "The value you entered was invalid"
    
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
    
    fileprivate func shippingAddressInvalidErrorExpectation(forField field: String) -> XCTestExpectation {
        return expectation(description: "createApplePayShippingAddressInvalidError with field named:\(field) failed to finish")
    }
    
    fileprivate func billingAddressInvalidErrorExpectation(forField field: String) -> XCTestExpectation {
        return expectation(description: "createApplePayBillingAddressInvalidError with field named:\(field) failed to finish")
    }
    
    fileprivate func contactInvalidErrorExpectation(forField field: String) -> XCTestExpectation {
        return expectation(description: "createApplePayContactInvalidError with field named:\(field) failed to finish")
    }
}

// ----------------------------------
//  MARK: - ShippingAddressInvalidError -
//
@available(iOS 11.0, *)
extension ApplePayErrorDeserializingTests {
    
    func testStreetShippingAddressInvalidError() {
        
        let field = "Street"
        let expectedError = PKPaymentRequest.paymentShippingAddressInvalidError(withKey: CNPostalAddressStreetKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = shippingAddressInvalidErrorExpectation(forField: field)
        createShippingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testSublocalityShippingAddressInvalidError() {
        
        let field = "Sublocality"
        let expectedError = PKPaymentRequest.paymentShippingAddressInvalidError(withKey: CNPostalAddressSubLocalityKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = shippingAddressInvalidErrorExpectation(forField: field)
        createShippingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testCityShippingAddressInvalidError() {
        
        let field = "City"
        let expectedError = PKPaymentRequest.paymentShippingAddressInvalidError(withKey: CNPostalAddressCityKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = shippingAddressInvalidErrorExpectation(forField: field)
        createShippingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testSubAdministrativeAreaShippingAddressInvalidError() {
        
        let field = "SubAdministrativeArea"
        let expectedError = PKPaymentRequest.paymentShippingAddressInvalidError(withKey: CNPostalAddressSubAdministrativeAreaKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = shippingAddressInvalidErrorExpectation(forField: field)
        createShippingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testStateShippingAddressInvalidError() {
        
        let field = "State"
        let expectedError = PKPaymentRequest.paymentShippingAddressInvalidError(withKey: CNPostalAddressStateKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = shippingAddressInvalidErrorExpectation(forField: field)
        createShippingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testPostalCodeShippingAddressInvalidError() {
        
        let field = "PostalCode"
        let expectedError = PKPaymentRequest.paymentShippingAddressInvalidError(withKey: CNPostalAddressPostalCodeKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = shippingAddressInvalidErrorExpectation(forField: field)
        createShippingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testCountryShippingAddressInvalidError() {
        
        let field = "Country"
        let expectedError = PKPaymentRequest.paymentShippingAddressInvalidError(withKey: CNPostalAddressCountryKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = shippingAddressInvalidErrorExpectation(forField: field)
        createShippingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testISOCountryCodeShippingAddressInvalidError() {
        
        let field = "ISOCountryCode"
        let expectedError = PKPaymentRequest.paymentShippingAddressInvalidError(withKey: CNPostalAddressISOCountryCodeKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = shippingAddressInvalidErrorExpectation(forField: field)
        createShippingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
}

// ----------------------------------
//  MARK: - BillingAddressInvalidError -
//
@available(iOS 11.0, *)
extension ApplePayErrorDeserializingTests {
    
    func testStreetBillingAddressInvalidError() {
        
        let field = "Street"
        let expectedError = PKPaymentRequest.paymentBillingAddressInvalidError(withKey: CNPostalAddressStreetKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = billingAddressInvalidErrorExpectation(forField: field)
        createBillingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testSublocalityBillingAddressInvalidError() {
        
        let field = "Sublocality"
        let expectedError = PKPaymentRequest.paymentBillingAddressInvalidError(withKey: CNPostalAddressSubLocalityKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = billingAddressInvalidErrorExpectation(forField: field)
        createBillingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testCityBillingAddressInvalidError() {
        
        let field = "City"
        let expectedError = PKPaymentRequest.paymentBillingAddressInvalidError(withKey: CNPostalAddressCityKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = billingAddressInvalidErrorExpectation(forField: field)
        createBillingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testSubAdministrativeAreaBillingAddressInvalidError() {
        
        let field = "SubAdministrativeArea"
        let expectedError = PKPaymentRequest.paymentBillingAddressInvalidError(withKey: CNPostalAddressSubAdministrativeAreaKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = billingAddressInvalidErrorExpectation(forField: field)
        createBillingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testStateBillingAddressInvalidError() {
        
        let field = "State"
        let expectedError = PKPaymentRequest.paymentBillingAddressInvalidError(withKey: CNPostalAddressStateKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = billingAddressInvalidErrorExpectation(forField: field)
        createBillingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testPostalCodeBillingAddressInvalidError() {
        
        let field = "PostalCode"
        let expectedError = PKPaymentRequest.paymentBillingAddressInvalidError(withKey: CNPostalAddressPostalCodeKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = billingAddressInvalidErrorExpectation(forField: field)
        createBillingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testCountryBillingAddressInvalidError() {
        
        let field = "Country"
        let expectedError = PKPaymentRequest.paymentBillingAddressInvalidError(withKey: CNPostalAddressCountryKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = billingAddressInvalidErrorExpectation(forField: field)
        createBillingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testISOCountryCodeBillingAddressInvalidError() {
        
        let field = "ISOCountryCode"
        let expectedError = PKPaymentRequest.paymentBillingAddressInvalidError(withKey: CNPostalAddressISOCountryCodeKey, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = billingAddressInvalidErrorExpectation(forField: field)
        createBillingAddressInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String, expectedError.userInfo[PKPaymentErrorKey.postalAddressUserInfoKey.rawValue] as! String)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
}

// ----------------------------------
//  MARK: - Contact Invalid Error -
//
@available(iOS 11.0, *)
extension ApplePayErrorDeserializingTests {
    
    func testPostalAddressContactInvalidError() {
        
        let field = "PostalAddress"
        let expectedError = PKPaymentRequest.paymentContactInvalidError(withContactField: PKContactField.postalAddress, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = contactInvalidErrorExpectation(forField: field)
        createContactInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.contactFieldUserInfoKey.rawValue] as! PKContactField, expectedError.userInfo[PKPaymentErrorKey.contactFieldUserInfoKey.rawValue] as! PKContactField)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testEmailAddressContactInvalidError() {
        
        let field = "EmailAddress"
        let expectedError = PKPaymentRequest.paymentContactInvalidError(withContactField: PKContactField.emailAddress, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = contactInvalidErrorExpectation(forField: field)
        createContactInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.contactFieldUserInfoKey.rawValue] as! PKContactField, expectedError.userInfo[PKPaymentErrorKey.contactFieldUserInfoKey.rawValue] as! PKContactField)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testPhoneNumberContactInvalidError() {
        
        let field = "PhoneNumber"
        let expectedError = PKPaymentRequest.paymentContactInvalidError(withContactField: PKContactField.phoneNumber, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = contactInvalidErrorExpectation(forField: field)
        createContactInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.contactFieldUserInfoKey.rawValue] as! PKContactField, expectedError.userInfo[PKPaymentErrorKey.contactFieldUserInfoKey.rawValue] as! PKContactField)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testNameContactInvalidError() {
        
        let field = "Name"
        let expectedError = PKPaymentRequest.paymentContactInvalidError(withContactField: PKContactField.name, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = contactInvalidErrorExpectation(forField: field)
        createContactInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.contactFieldUserInfoKey.rawValue] as! PKContactField, expectedError.userInfo[PKPaymentErrorKey.contactFieldUserInfoKey.rawValue] as! PKContactField)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
    
    func testPhoneticNameContactInvalidError() {
        
        let field = "PhoneticName"
        let expectedError = PKPaymentRequest.paymentContactInvalidError(withContactField: PKContactField.phoneticName, localizedDescription: expectedDescription) as NSError
        
        let createErrorExpectation = contactInvalidErrorExpectation(forField: field)
        createContactInvalidErrorJsonString(forField: field, withDescription: expectedDescription) { errorJsonString in
            
            let errorJson     = try! JSONSerialization.jsonObject(with: errorJsonString.data(using: .utf8)!) as! JSON
            let paymentError  = PKPaymentRequest.paymentError(with: errorJson)! as NSError
            
            XCTAssertEqual(paymentError.localizedDescription, expectedError.localizedDescription)
            XCTAssertEqual(paymentError.userInfo[PKPaymentErrorKey.contactFieldUserInfoKey.rawValue] as! PKContactField, expectedError.userInfo[PKPaymentErrorKey.contactFieldUserInfoKey.rawValue] as! PKContactField)
            
            createErrorExpectation.fulfill()
        }
        
        self.wait(for: [createErrorExpectation], timeout: timeout)
    }
}

// ----------------------------------
//  MARK: - Error creation helpers -
//
@available(iOS 11.0, *)
extension ApplePayErrorDeserializingTests {
    private enum ErrorMetaKey: String {
        case field = "Field"
        case description = "Description"
    }
    
    private func sendErrorMeta(_ meta: JSON, toMethod method: String, completion: @escaping (_ jsonString: String) -> Void) {
        
        let errorMetaData = try! JSONSerialization.data(withJSONObject: meta)
        let errorMetaString = String(data: errorMetaData, encoding: .utf8)!
        
        let createErrorMessage = UnityMessage(content: errorMetaString, object: TesterName, method: method)
        
        MessageCenter.send(createErrorMessage) { response in
            completion(response!)
        }
    }
    
    fileprivate func createShippingAddressInvalidErrorJsonString(forField field: String, withDescription description: String, completion: @escaping (_ jsonString: String) -> Void) {
        
        let errorMeta = [ErrorMetaKey.field.rawValue: field, ErrorMetaKey.description.rawValue: description]
        sendErrorMeta(errorMeta, toMethod: TesterMethod.createApplePayShippingAddressInvalidError.rawValue, completion: completion)
    }
    
    fileprivate func createBillingAddressInvalidErrorJsonString(forField field: String, withDescription description: String, completion: @escaping (_ jsonString: String) -> Void) {
        
        let errorMeta = [ErrorMetaKey.field.rawValue: field, ErrorMetaKey.description.rawValue: description]
        sendErrorMeta(errorMeta, toMethod: TesterMethod.createApplePayBillingAddressInvalidError.rawValue, completion: completion)
    }
    
    fileprivate func createContactInvalidErrorJsonString(forField field: String, withDescription description: String, completion: @escaping (_ jsonString: String) -> Void) {
        
        let errorMeta = [ErrorMetaKey.field.rawValue: field, ErrorMetaKey.description.rawValue: description]
        sendErrorMeta(errorMeta, toMethod: TesterMethod.createApplePayContactInvalidError.rawValue, completion: completion)
    }
    
    fileprivate func createShippingAddressUnservicable (forField field: String, withDescription description: String, completion: @escaping (_ jsonString: String) -> Void) {
        
        let errorMeta = [ErrorMetaKey.description.rawValue: description]
        sendErrorMeta(errorMeta, toMethod: TesterMethod.createApplePayShippingAddressUnservicableError.rawValue, completion: completion)
    }
}
