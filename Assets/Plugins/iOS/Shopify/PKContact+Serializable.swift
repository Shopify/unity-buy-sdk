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

enum ContactField: String {
    case firstName
    case lastName
    case address1
    case address2
    case city
    case country
    case province
    case zip
    case email
    case phone
}

extension PKContact: Serializable {
    func serializedJSON() -> JSON {
        var json = JSON()
        json.insert(nullable: self.name?.givenName,           forKey: ContactField.firstName)
        json.insert(nullable: self.name?.familyName,          forKey: ContactField.lastName)
        json.insert(nullable: self.postalAddress?.city,       forKey: ContactField.city)
        json.insert(nullable: self.postalAddress?.country,    forKey: ContactField.country)
        json.insert(nullable: self.postalAddress?.state,      forKey: ContactField.province)
        json.insert(nullable: self.postalAddress?.postalCode, forKey: ContactField.zip)
        json.insert(nullable: self.emailAddress,              forKey: ContactField.email)
        json.insert(nullable: self.phoneNumber?.stringValue,  forKey: ContactField.phone)
        
        if let street = self.postalAddress?.street {
            let addresses = addressComponents(inStreet: street)
            
            json.insert(nullable: addresses[0], forKey: ContactField.address1)

            if addresses.count > 1 {
                json.insert(nullable: addresses[1], forKey: ContactField.address2)
            }
        }
        
        return json
    }
    
    private func addressComponents(inStreet street: String) -> [String] {
        return street.components(separatedBy: "\n")
    }
}
