//
//  PaymentErrorMeta.swift
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

@available(iOS 11.0, *)
class PaymentErrorMeta {
    let type: ErrorType
    let description: String?
    
    init? (json: JSON) {
        
        guard let errorType = PaymentErrorMeta.errorType(in: json) else {
            return nil
        }
        
        self.type = errorType
        self.description = json[PaymentErrorKey.description.rawValue] as? String
    }
}

// ----------------------------------
//  MARK: - Enums -
//
@available(iOS 11.0, *)
extension PaymentErrorMeta {
    
    public enum ErrorType: String {
        case paymentBillingAddress       = "PaymentBillingAddress"
        case paymentShippingAddress      = "PaymentShippingAddress"
        case paymentContactInvalid       = "PaymentContactInvalid"
        case shippingAddressUnservicable = "ShippingAddressUnservicable"
    }
    
    fileprivate enum PaymentErrorKey: String {
        case type = "Type"
        case description = "Description"
    }
}

// ----------------------------------
//  MARK: - Class functions -
//
@available(iOS 11.0, *)
extension PaymentErrorMeta {
    
    class func errorType(in json: JSON) -> ErrorType? {
        
        guard let typeString = json[PaymentErrorKey.type.rawValue] as? String else {
            return nil
        }
        
        return ErrorType(rawValue: typeString)
    }
}
