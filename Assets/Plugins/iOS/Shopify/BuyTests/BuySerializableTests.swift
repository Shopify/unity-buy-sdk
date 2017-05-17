//
//  BuySerializableTests.swift
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
import ProductName

class BuySerializableTests: XCTestCase {
    
    let emailAddress = "test_email@shopify.com"
    let firstName    = "first name"
    let lastName     = "last name"
    let phone        = "123-456-7890"
    let address1     = "80 Spadina Ave"
    let address2     = "420 Wellington"
    let edgeAddress1 = "420\\nWellington"
    let edgeAddress2 = "80\\nSpadina\\nAve"
    let city         = "Toronto"
    let country      = "Canada"
    let province     = "ON"
    let zip          = "A1B 2C3"
    
    // ----------------------------------
    //  MARK: - PKPayment -
    //
    func testPKPaymentSerializable() {
        
        let billingContact        = createContact()
        let shippingContact       = billingContact
        let shippingMethod        = PKShippingMethod.init(label: "Free Shipping", amount: 0)
        shippingMethod.identifier = "unique_id"
        
        let paymentMethod = MockPaymentMethod.init(displayName: "AMEX", network: .amex, type: .credit)
        let token         = MockPaymentToken.init(paymentMethod: paymentMethod)
        let payment       = MockPayment.init(token: token,
                                             billingContact: billingContact,
                                             shippingContact: shippingContact,
                                             shippingMethod: shippingMethod)
        
        // Serialize and record data from result
        let paymentDict         = payment.asDictionary()
        let billingContactDict  = paymentDict[PKPaymentSerializedField.billingContact.rawValue]     as! [String: Any]
        let shippingContactDict = paymentDict[PKPaymentSerializedField.shippingContact.rawValue]    as! [String: Any]
        let tokenDict           = paymentDict[PKPaymentSerializedField.tokenData.rawValue]          as! [String: Any]
        let shippingIdentifier  = paymentDict[PKPaymentSerializedField.shippingIdentifier.rawValue] as! String
        
        let tokenDataDict              = tokenDict[PKPaymentTokenSerializedField.paymentData.rawValue] as Any
        let tokenData                  = try! JSONSerialization.data(withJSONObject: tokenDataDict)
        let tokenTransactionIdentifier = tokenDict[PKPaymentTokenSerializedField.transactionIdentifier.rawValue] as! String
        
        assertEqualContact(billingContactDict,  contact: billingContact)
        assertEqualContact(shippingContactDict, contact: shippingContact)
        
        XCTAssertEqual(shippingIdentifier,         shippingMethod.identifier)
        XCTAssertEqual(tokenData,                  token.paymentData)
        XCTAssertEqual(tokenTransactionIdentifier, token.transactionIdentifier)
        
        XCTAssertNotNil(payment.asJSONString())
        XCTAssertNoThrow(try JSONSerialization.jsonObject(with: payment.asJSONString().data(using: .utf8)!))
    }
    
    // ----------------------------------
    //  MARK: - PKPaymentToken -
    //
    func testPKPaymentTokenSerializable() {
        
        let paymentMethod = MockPaymentMethod.init(displayName: "AMEX", network: .amex, type: .credit)
        let token         = MockPaymentToken.init(paymentMethod: paymentMethod)
        
        // Serialize and record data from result
        let tokenDict     = token.asDictionary()
        let tokenDataDict = tokenDict[PKPaymentTokenSerializedField.paymentData.rawValue] as Any
        let tokenData     = try! JSONSerialization.data(withJSONObject: tokenDataDict)
        let tokenTransactionIdentifier = tokenDict[PKPaymentTokenSerializedField.transactionIdentifier.rawValue] as! String
        
        XCTAssertEqual(tokenData,                  token.paymentData)
        XCTAssertEqual(tokenTransactionIdentifier, token.transactionIdentifier)
        
        XCTAssertNotNil(token.asJSONString())
        XCTAssertNoThrow(try JSONSerialization.jsonObject(with: token.asJSONString().data(using: .utf8)!))
    }
    
    // ----------------------------------
    //  MARK: - PKContact -
    //
    func testPKContactSerializable() {
        let contact = createContact()
        assertPKContactSerializable(contact)
    }
    
    func testPKContactMultiAddressSerializable() {
        let contact           = createContact()
        contact.postalAddress = createMultiPostalAddress()
        assertPKContactSerializable(contact)
    }
    
    func testPKContactMultiAddressSerializableEdgeCase() {
        let contact           = createContact()
        contact.postalAddress = createMultiPostalAddressEdgeCase()
        assertPKContactSerializable(contact)
    }
    
    // ----------------------------------
    //  MARK: - Conveniences -
    //
    func createPersonName() -> PersonNameComponents {
        var personName = PersonNameComponents.init()
        personName.givenName  = firstName
        personName.familyName = lastName
        return personName
    }
    
    func createPostalAddress() -> CNPostalAddress {
        let postalAddress        = CNMutablePostalAddress.init()
        postalAddress.street     = address1
        postalAddress.postalCode = zip
        postalAddress.country    = country
        postalAddress.state      = province
        postalAddress.city       = city
        return postalAddress
    }
    
    func createMultiPostalAddress() -> CNPostalAddress {
        let postalAddress    = createPostalAddress() as! CNMutablePostalAddress
        postalAddress.street = address1 + "\n" + address2
        return postalAddress
    }
    
    func createMultiPostalAddressEdgeCase() -> CNPostalAddress {
        let postalAddress    = createPostalAddress() as! CNMutablePostalAddress
        postalAddress.street = edgeAddress1 + "\n" + edgeAddress2
        return postalAddress
    }
    
    func createContact() -> PKContact {
        let contact           = PKContact.init()
        contact.name          = createPersonName()
        contact.emailAddress  = emailAddress
        contact.phoneNumber   = CNPhoneNumber.init(stringValue: phone)
        contact.postalAddress = createPostalAddress()
        return contact
    }
    
    // ----------------------------------
    //  MARK: - Convenience Asserts -
    //
    func assertEqualContact(_ contactDict: Dictionary<String, Any>, contact: PKContact) {
        XCTAssertEqual(contactDict[PKContactSerializedField.firstName.rawValue] as? String, contact.name?.givenName);
        XCTAssertEqual(contactDict[PKContactSerializedField.lastName.rawValue]  as? String, contact.name?.familyName);
        XCTAssertEqual(contactDict[PKContactSerializedField.city.rawValue]      as? String, contact.postalAddress?.city);
        XCTAssertEqual(contactDict[PKContactSerializedField.country.rawValue]   as? String, contact.postalAddress?.country);
        XCTAssertEqual(contactDict[PKContactSerializedField.province.rawValue]  as? String, contact.postalAddress?.state);
        XCTAssertEqual(contactDict[PKContactSerializedField.zip.rawValue]       as? String, contact.postalAddress?.postalCode);
        XCTAssertEqual(contactDict[PKContactSerializedField.email.rawValue]     as? String, contact.emailAddress);
        
        if let firstAddress = contactDict[PKContactSerializedField.address1.rawValue] as? String {
            if let secondAddress = contactDict[PKContactSerializedField.address2.rawValue] as? String {
                XCTAssertEqual(firstAddress + "\n" + secondAddress, contact.postalAddress?.street);
            }
            else
            {
                XCTAssertEqual(firstAddress, contact.postalAddress?.street);
            }
        }
    }
    
    func assertPKContactSerializable(_ contact: PKContact) {
        let contactDict = contact.asDictionary()
        assertEqualContact(contactDict, contact: contact)
        XCTAssertNotNil(contact.asJSONString())
        XCTAssertNoThrow(try JSONSerialization.jsonObject(with: contact.asJSONString().data(using: .utf8)!))
    }
}
