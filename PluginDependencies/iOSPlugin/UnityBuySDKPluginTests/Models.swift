//
//  Models.swift
//  UnityBuySDK
//
//  Created by Shopify.
//  Copyright (c) 2017 Shopify Inc. All rights reserved.
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
@testable import UnityBuySDKPlugin

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
    static let zip          = "M5V 2J4"
    
    static let merchantId   = "com.merchant.id"
    static let countryCode  = "US"
    static let currencyCode = "USD"
    
    static let supportedPaymentNetworks: [PKPaymentNetwork] = [.amex, .visa, .masterCard]
    
    // ----------------------------------
    //  MARK: - PersonNameComponents -
    //
    static func createPersonName() -> PersonNameComponents {
        var personName        = PersonNameComponents()
        personName.givenName  = firstName
        personName.familyName = lastName
        return personName
    }
    
    // ----------------------------------
    //  MARK: - CNPostalAddress -
    //
    
    static func createPartialPostalAddress() -> CNPostalAddress {
        let postalAddress        = CNMutablePostalAddress()
        postalAddress.city       = city
        postalAddress.country    = country
        postalAddress.postalCode = zip
        postalAddress.state      = province
        return postalAddress
    }
    
    static func createPostalAddress() -> CNPostalAddress {
        let postalAddress        = CNMutablePostalAddress()
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
        let contact           = PKContact()
        contact.name          = createPersonName()
        contact.emailAddress  = emailAddress
        contact.phoneNumber   = CNPhoneNumber(stringValue: phone)
        contact.postalAddress = postalAddress
        return contact
    }
    
    static func createPaymentMethod() -> PKPaymentMethod {
        return MockPaymentMethod(displayName: "AMEX", network: .amex, type: .credit)
    }
    
    static func createShippingMethod() -> PKShippingMethod {
        let shippingMethod        = PKShippingMethod(label: "Free Shipping", amount: 0)
        shippingMethod.identifier = "unique_id"
        shippingMethod.detail     = "10-15 Days"
        return shippingMethod
    }
    
    static func createPaymentToken() -> PKPaymentToken {
        return MockPaymentToken(paymentMethod: Models.createPaymentMethod() as! MockPaymentMethod)
    }
    
    static func createSimulatorPaymentToken() -> PKPaymentToken {
        return MockPaymentToken(paymentMethod: Models.createPaymentMethod() as! MockPaymentMethod, forSimulator: true)
    }
    
    static func createPayment() -> PKPayment {
        return MockPayment(
            token: createPaymentToken() as! MockPaymentToken,
            billingContact: createContact(with: createPostalAddress()),
            shippingContact: createContact(with: createPostalAddress()),
            shippingMethod: createShippingMethod())
    }
    
    static func createSummaryItem() -> PKPaymentSummaryItem {
        return PKPaymentSummaryItem(label: "SubTotal", amount: NSDecimalNumber(value: 1.00))
    }
    
    static func createSummaryItems() -> [PKPaymentSummaryItem] {
        var summaryItems = [PKPaymentSummaryItem]()
        summaryItems.append(PKPaymentSummaryItem(label: "SubTotal", amount: NSDecimalNumber(value: 1.00)))
        summaryItems.append(PKPaymentSummaryItem(label: "Tax",      amount: NSDecimalNumber(value: 1.00)))
        summaryItems.append(PKPaymentSummaryItem(label: "Shipping", amount: NSDecimalNumber(value: 1.00)))
        summaryItems.append(PKPaymentSummaryItem(label: "Total",    amount: NSDecimalNumber(value: 3.00)))
        return summaryItems
    }
    
    static func createShippingMethods() -> [PKShippingMethod] {
        var shippingMethods = [PKShippingMethod]()
        
        shippingMethods.append(
            PKShippingMethod(
                label: "Free",
                amount: NSDecimalNumber(value: 0.0),
                identifier: "Free",
                detail: "10-15 Days"))
        
        shippingMethods.append(
            PKShippingMethod(
                label: "Standard",
                amount: NSDecimalNumber(value: 5.0),
                identifier: "Standard",
                detail: "10-15 Days"))
        
        shippingMethods.append(
            PKShippingMethod(
                label: "Express",
                amount: NSDecimalNumber(value: 10.0),
                identifier: "Express",
                detail: "10-15 Days"))
        
        return shippingMethods
    }
    
    static func createPaymentSession(requiringShippingAddressFields: Bool, usingNonDefault controllerType: PKPaymentAuthorizationController.Type?) -> PaymentSession {
        
        if let controllerType = controllerType {
            return PaymentSession(
                merchantId: merchantId,
                countryCode: countryCode,
                currencyCode: currencyCode,
                requiringShippingAddressFields: requiringShippingAddressFields,
                supportedNetworks: supportedPaymentNetworks,
                summaryItems: createSummaryItems(),
                shippingMethods: createShippingMethods(),
                controllerType: controllerType)!
        } else {
            return PaymentSession(
                merchantId: merchantId,
                countryCode: countryCode,
                currencyCode: currencyCode,
                requiringShippingAddressFields: requiringShippingAddressFields,
                supportedNetworks: supportedPaymentNetworks,
                summaryItems: createSummaryItems(),
                shippingMethods: createShippingMethods())!
        }
    }
    
    static func createSerializedPaymentNetworksString() -> String {
        let jsonData = try! JSONSerialization.data(withJSONObject: supportedPaymentNetworks)
        let stringRepresentation = String(data: jsonData, encoding: .utf8)!
        return stringRepresentation
    }
}

// ----------------------------------
//  MARK: - JSON models -
//
extension Models {
    enum SummaryItemField: String {
        case amount = "Amount"
        case label  = "Label"
        case type   = "Type"
    }
    
    enum ShippingMethodField: String {
        case detail     = "Detail"
        case identifier = "Identifier"
    }
    
    static func createSummaryItemJson(amount: String, label: String, type: PKPaymentSummaryItemType? = nil) -> JSON {
        
        var json = JSON.init()
        json[SummaryItemField.amount.rawValue] = amount
        json[SummaryItemField.label.rawValue]  = label
        
        if let type = type {
            json[SummaryItemField.type.rawValue] = type.rawStringValue
        }
        
        return json
    }
    
    static func createShippingMethodJson(amount: String, label: String, identifier: String, detail: String, type: PKPaymentSummaryItemType? = nil) -> JSON {
        
        var json = createSummaryItemJson(amount: amount, label: label, type: type)
        json[ShippingMethodField.detail.rawValue]      = detail
        json[ShippingMethodField.identifier.rawValue]  = identifier
        
        if let type = type {
            json[SummaryItemField.type.rawValue] = type.rawStringValue
        }
        
        return json
    }
}
