//
//  PKContact+Serializable.swift
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

public enum PKContactSerializedField: String {
    case address1
    case address2
    case city
    case company
    case country
    case firstName
    case lastName
    case phone
    case province
    case zip
    case email
}

extension PKContact: BuySerializable {
    public func asDictionary() -> Dictionary<String, Any> {
        var dictionary = [String: Any]()
        add(.firstName, withValue: self.name?.givenName,           to: &dictionary)
        add(.lastName,  withValue: self.name?.familyName,          to: &dictionary)
        add(.city,      withValue: self.postalAddress?.city,       to: &dictionary)
        add(.country,   withValue: self.postalAddress?.country,    to: &dictionary)
        add(.province,  withValue: self.postalAddress?.state,      to: &dictionary)
        add(.zip,       withValue: self.postalAddress?.postalCode, to: &dictionary)
        add(.email,     withValue: self.emailAddress,              to: &dictionary)
        
        if let street = self.postalAddress?.street {
            let addresses = addressComponents(inStreet: street)
            
            add(.address1, withValue: addresses[0], to: &dictionary)
            
            if addresses.count > 1 {
                add(.address2, withValue: addresses[1], to: &dictionary)
            }
        }
        
        return dictionary
    }
    
    private func add(_ key: PKContactSerializedField, withValue value: Any?, to dictionary: inout Dictionary<String, Any>) {
        insert(key, withValue: value, to: &dictionary)
    }
    
    private func addressComponents(inStreet street: String) -> [String] {
        return street.components(separatedBy: "\n")
    }
}
