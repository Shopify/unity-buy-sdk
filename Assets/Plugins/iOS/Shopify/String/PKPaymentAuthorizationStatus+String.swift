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

extension PKPaymentAuthorizationStatus {
    
    static func from(_ string: String) -> PKPaymentAuthorizationStatus? {
        switch(string) {
        case "Failure": return .failure
        case "Success": return .success
        case "InvalidBillingPostalAddress": return .invalidBillingPostalAddress
        case "InvalidShippingPostalAddress": return .invalidShippingPostalAddress
        case "InvalidShippingContact": return .invalidShippingContact
        case "PinRequired":
            if #available(iOS 9.2, *) {
                return .pinRequired
            } else {
                return nil
            }
        case "PinIncorrect":
            if #available(iOS 9.2, *) {
                return .pinIncorrect
            } else {
                return nil
            }
        case "PinLockout":
            if #available(iOS 9.2, *) {
                return .pinLockout
            } else {
                return nil
            }
        default:
            return nil
        }
    }
}
