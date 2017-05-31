//
//  Models.swift
//  Unity-iPhone
//
//  Created by Shopify.
//
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
import ProductName

struct Models {
    
    static let emailAddress = "test_email@shopify.com"
    static let firstName    = "first name"
    static let lastName     = "last name"
    static let phone        = "123-456-7890"
    static let address1     = "80 Spadina Ave"
    static let address2     = "420 Wellington"
    static let edgeAddress1 = "420\\nWellington"
    static let edgeAddress2 = "80\\nSpadina\\nAve"
    static let city         = "Toronto"
    static let country      = "Canada"
    static let province     = "ON"
    static let zip          = "A1B 2C3"
    
    // ----------------------------------
    //  MARK: - PersonNameComponents -
    //
    static func createPersonName() -> PersonNameComponents {
        var personName        = PersonNameComponents.init()
        personName.givenName  = firstName
        personName.familyName = lastName
        return personName
    }
    
    // ----------------------------------
    //  MARK: - CNPostalAddress -
    //
    static func createPostalAddress() -> CNPostalAddress {
        let postalAddress        = CNMutablePostalAddress.init()
        postalAddress.street     = address1
        postalAddress.postalCode = zip
        postalAddress.country    = country
        postalAddress.state      = province
        postalAddress.city       = city
        return postalAddress
    }
    
    static func createMultiplePostalAddresses() -> CNPostalAddress {
        let postalAddress    = createPostalAddress() as! CNMutablePostalAddress
        postalAddress.street = address1 + "\n" + address2
        return postalAddress
    }
    
    static func createMultiplePostalAddressesEdgeCase() -> CNPostalAddress {
        let postalAddress    = createPostalAddress() as! CNMutablePostalAddress
        postalAddress.street = edgeAddress1 + "\n" + edgeAddress2
        return postalAddress
    }
    
    // ----------------------------------
    //  MARK: - PassKit -
    //
    static func createContact(with postalAddress: CNPostalAddress) -> PKContact {
        let contact           = PKContact.init()
        contact.name          = createPersonName()
        contact.emailAddress  = emailAddress
        contact.phoneNumber   = CNPhoneNumber.init(stringValue: phone)
        contact.postalAddress = postalAddress
        return contact
    }
    
    static func createPaymentMethod() -> PKPaymentMethod {
        return MockPaymentMethod.init(displayName: "AMEX", network: .amex, type: .credit)
    }
    
    static func createShippingMethod() -> PKShippingMethod {
        let shippingMethod        = PKShippingMethod.init(label: "Free Shipping", amount: 0)
        shippingMethod.identifier = "unique_id"
        return shippingMethod
    }
    
    static func createPaymentToken() -> PKPaymentToken {
        return MockPaymentToken.init(paymentMethod: Models.createPaymentMethod() as! MockPaymentMethod)
    }
    
    static func createPayment() -> PKPayment {
        return MockPayment.init(token: createPaymentToken() as! MockPaymentToken,
                                billingContact: createContact(with: createPostalAddress()),
                                shippingContact: createContact(with: createPostalAddress()),
                                shippingMethod: createShippingMethod())
    }
}
