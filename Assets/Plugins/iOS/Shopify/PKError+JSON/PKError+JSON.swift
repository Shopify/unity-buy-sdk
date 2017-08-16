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
        
        guard let errorType = PaymentErrorMeta.errorType(in: json) else {
            return nil
        }
        
        switch errorType {
        case .paymentBillingAddress:
            if let errorMeta = AddressInvalidErrorMeta(json: json) {
                return paymentBillingAddressInvalidError(from: errorMeta)
            }
        case .paymentShippingAddress:
            if let errorMeta = AddressInvalidErrorMeta(json: json) {
                return paymentShippingAddressInvalidError(from: errorMeta)
            }
        case .paymentContactInvalid:
            if let errorMeta = ContactInvalidErrorMeta(json: json) {
                return paymentContactInvalidError(from: errorMeta)
            }
        case .shippingAddressUnservicable:
            if let errorMeta = PaymentErrorMeta(json: json) {
                return paymentShippingAddressUnserviceableError(from: errorMeta)
            }
        }
        
        return nil
    }
}

//  ----------------------------------
//  MARK: - Fileprivate Error Creators -
//
@available(iOS 11.0, *)
extension NSError {
    
    fileprivate class func paymentBillingAddressInvalidError(from errorMeta: AddressInvalidErrorMeta) -> Error {
        return paymentBillingAddressInvalidError(withKey: errorMeta.postalAddressKey, localizedDescription: errorMeta.description)
    }
    
    fileprivate class func paymentShippingAddressInvalidError(from errorMeta: AddressInvalidErrorMeta) -> Error {
        return paymentShippingAddressInvalidError(withKey: errorMeta.postalAddressKey, localizedDescription: errorMeta.description)
    }
    
    fileprivate class func paymentContactInvalidError(from errorMeta: ContactInvalidErrorMeta) -> Error {
        return paymentContactInvalidError(withContactField: errorMeta.contactField, localizedDescription: errorMeta.description)
    }
    
    fileprivate class func paymentShippingAddressUnserviceableError(from errorMeta: PaymentErrorMeta) -> Error {
        return paymentShippingAddressUnserviceableError(withLocalizedDescription: errorMeta.description)
    }
}
