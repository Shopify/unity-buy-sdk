//
//  ContactInvalidErrorMeta.swift
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

@available(iOS 11.0, *)
class ContactInvalidErrorMeta: PaymentErrorMeta {
    let contactField: PKContactField
    
    override init? (json: JSON) {
        
        guard
            let contactInvalidFieldString = json[ErrorKey.field.rawValue] as? String,
            let contactInvalidField       = Field(rawValue: contactInvalidFieldString)
        else {
            return nil
        }
        
        self.contactField = contactInvalidField.contactField()
        
        super.init(json: json)
        
        if self.type != ErrorType.paymentContactInvalid {
            return nil
        }
    }
}

// ----------------------------------
//  MARK: - Enums -
//
@available(iOS 11.0, *)
extension ContactInvalidErrorMeta {
    
    fileprivate enum ErrorKey: String {
        case field = "Field"
    }
    
    fileprivate enum Field: String {
        case postalAddress = "PostalAddress"
        case emailAddress = "EmailAddress"
        case phoneNumber   = "PhoneNumber"
        case name          = "Name"
        case phoneticName  = "PhoneticName"
        
        func contactField() -> PKContactField {
            switch self {
            case .postalAddress: return PKContactField.postalAddress
            case .emailAddress:  return PKContactField.emailAddress
            case .phoneNumber:   return PKContactField.phoneNumber
            case .name:          return PKContactField.name
            case .phoneticName:  return PKContactField.phoneticName
            }
        }
    }
}
