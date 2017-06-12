//
//  PKPaymentAuthorizationStatus+String.swift
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

extension PKPaymentAuthorizationStatus: RawRepresentable {
    
    public init?(rawValue: String) {
        switch rawValue {
        case PKPaymentAuthorizationStatus.failure.rawValue: self = .failure
        case PKPaymentAuthorizationStatus.success.rawValue: self = .success
        case PKPaymentAuthorizationStatus.invalidShippingContact.rawValue:       self = .invalidShippingContact
        case PKPaymentAuthorizationStatus.invalidBillingPostalAddress.rawValue:  self = .invalidBillingPostalAddress
        case PKPaymentAuthorizationStatus.invalidShippingPostalAddress.rawValue: self = .invalidShippingPostalAddress
        default:
            if #available(iOS 9.2, *) {
                switch rawValue {
                case PKPaymentAuthorizationStatus.pinRequired.rawValue:  self = .pinRequired
                case PKPaymentAuthorizationStatus.pinIncorrect.rawValue: self = .pinIncorrect
                case PKPaymentAuthorizationStatus.pinLockout.rawValue:   self = .pinLockout
                default: return nil
                }
            } else {
                return nil
            }
        }
    }
    
    public var rawValue: String {
        switch self {
        case .failure:
            return "Failure"
        case .success:
            return "Success"
        case .invalidBillingPostalAddress:
            return "InvalidBillingPostalAddress"
        case .invalidShippingPostalAddress:
            return "InvalidShippingPostalAddress"
        case .invalidShippingContact:
            return "InvalidShippingContact"
        case .pinRequired:
            return "PinRequired"
        case .pinIncorrect:
            return "PinIncorrect"
        case .pinLockout:
            return "PinLockout"
        }
    }
}
