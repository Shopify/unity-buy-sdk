//
//  PKError+JSON.swift
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

// ----------------------------------
//  MARK: - Class Payment Error Creator -
//
@available(iOS 11.0, *)
extension NSError {

    /// Convenience method to create a payment error with a JSON
    /// Returns a Payment error, payment errors can be found in PKError.h
    class func paymentError(with json: JSON) -> Error? {
        
        guard let errorType = errorType(in: json) else {
            return nil
        }
        
        switch errorType {
        case .paymentBillingAddress:
            return paymentBillingAddressInvalidError(from: json)
        case .paymentShippingAddress:
            return paymentShippingAddressInvalidError(from: json)
        case .paymentContactInvalid:
            return paymentContactInvalidError(from: json)
        case .shippingAddressUnservicable:
            return paymentShippingAddressUnserviceableError(from: json)
        }
    }
}

//  ----------------------------------
//  MARK: - Fileprivate Error Creators -
//
@available(iOS 11.0, *)
extension NSError {
    
    fileprivate class func paymentBillingAddressInvalidError(from json: JSON) -> Error? {

        guard
            let description        = json[PaymentErrorKey.description.rawValue] as? String,
            let addressFieldString = json[PaymentErrorKey.field.rawValue] as? String,
            let addressKey         = CNPostalAddress.postalAddressKey(for: addressFieldString)
        else {
            return nil
        }

        return paymentBillingAddressInvalidError(withKey: addressKey, localizedDescription: description)
    }
    
    fileprivate class func paymentShippingAddressInvalidError(from json: JSON) -> Error? {

        guard
            let description        = json[PaymentErrorKey.description.rawValue] as? String,
            let addressFieldString = json[PaymentErrorKey.field.rawValue] as? String,
            let addressKey         = CNPostalAddress.postalAddressKey(for: addressFieldString)
        else {
            return nil
        }

        return paymentShippingAddressInvalidError(withKey: addressKey, localizedDescription: description)
    }
    
    fileprivate class func paymentContactInvalidError(from json: JSON) -> Error? {

        guard
            let description        = json[PaymentErrorKey.description.rawValue] as? String,
            let contactFieldString = json[PaymentErrorKey.field.rawValue] as? String,
            let contactField       = PKContactField(string: contactFieldString)
        else {
            return nil
        }

        return paymentContactInvalidError(withContactField: contactField, localizedDescription: description)
    }
    
    fileprivate class func paymentShippingAddressUnserviceableError(from json: JSON) -> Error? {

        if let description = json[PaymentErrorKey.description.rawValue] as? String {
            return paymentShippingAddressUnserviceableError(withLocalizedDescription: description)
        } else {
            return nil
        }
    }
}

// ----------------------------------
//  MARK: - Parsing Errors -
//
@available(iOS 11.0, *)
extension NSError {

    fileprivate enum PaymentErrorKey: String {
        case type        = "Type"
        case description = "Description"
        case field       = "Field"
    }

    fileprivate enum ErrorType: String {
        case paymentBillingAddress       = "PaymentBillingAddress"
        case paymentShippingAddress      = "PaymentShippingAddress"
        case paymentContactInvalid       = "PaymentContactInvalid"
        case shippingAddressUnservicable = "ShippingAddressUnservicable"
    }

    fileprivate class func errorType(in json: JSON) -> ErrorType? {

        guard let typeString = json[PaymentErrorKey.type.rawValue] as? String else {
            return nil
        }

        return ErrorType(rawValue: typeString)
    }
}
