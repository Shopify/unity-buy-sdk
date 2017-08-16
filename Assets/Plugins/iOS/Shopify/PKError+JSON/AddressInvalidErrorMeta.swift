//
//  AddressInvalidErrorMeta.swift
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
import Contacts

@available(iOS 11.0, *)
class AddressInvalidErrorMeta: PaymentErrorMeta {
    let postalAddressKey: String /// A CNPostalAddressKey
    
    override init? (json: JSON) {
        
        guard
            let fieldString = json[ErrorKey.field.rawValue] as? String,
            let field       = Field(rawValue: fieldString)
        else {
            return nil
        }
        
        self.postalAddressKey = field.postalAddressKey()
        
        super.init(json: json)
        
        if self.type != ErrorType.paymentBillingAddress &&
            self.type != ErrorType.paymentShippingAddress {
            return nil
        }
    }
}

// ----------------------------------
//  MARK: - Enums -
//
@available(iOS 11.0, *)
extension AddressInvalidErrorMeta {
    fileprivate enum ErrorKey: String {
        case field = "Field"
    }
    
    fileprivate enum Field: String {
        case street         = "Street"
        case sublocality    = "Sublocality"
        case city           = "City"
        case state          = "State"
        case postalCode     = "PostalCode"
        case country        = "Country"
        case isoCountryCode = "ISOCountryCode"
        case subAdministrativeArea = "SubAdministrativeArea"
        
        func postalAddressKey() -> String {
            switch self {
            case .street:         return CNPostalAddressStreetKey
            case .sublocality:    return CNPostalAddressSubLocalityKey
            case .city:           return CNPostalAddressCityKey
            case .state:          return CNPostalAddressStateKey
            case .postalCode:     return CNPostalAddressPostalCodeKey
            case .country:        return CNPostalAddressCountryKey
            case .isoCountryCode: return CNPostalAddressISOCountryCodeKey
            case .subAdministrativeArea: return CNPostalAddressSubAdministrativeAreaKey
            }
        }
    }
}
