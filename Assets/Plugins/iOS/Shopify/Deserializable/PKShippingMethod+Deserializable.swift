//
//  PKShippingMethod+Deserializable.swift
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

enum ShippingMethodField: String {
    case detail
    case identifier
}

extension PKShippingMethod {
    
    convenience init?(serializedShippingMethod json:JSON) {
        
        guard let label = json[PaymentSummaryItemField.label.rawValue] as? String,
            let amount = json[PaymentSummaryItemField.amount.rawValue] as? String else {
            return nil
        }
           
        if
            let typeString = json[PaymentSummaryItemField.type.rawValue] as? String,
            let typeInt = UInt(typeString),
            let type = PKPaymentSummaryItemType.init(rawValue: typeInt) {
                self.init(label: label, amount: NSDecimalNumber.init(string: amount), type: type)
        }
        else {
            self.init(label: label, amount: NSDecimalNumber.init(string: amount))
        }
        
        detail = json[ShippingMethodField.detail.rawValue] as? String
        identifier = json[ShippingMethodField.identifier.rawValue] as? String
    }
    
    static func items(forSerializedShippingMethods shippingMethodsJsonObject: [JSON]) -> [PKShippingMethod]? {
        var methods = [PKShippingMethod]()
        
        for methodJson in shippingMethodsJsonObject {
            
            guard let shippingMethod = PKShippingMethod.init(serializedShippingMethod: methodJson) else {
                return nil
            }
            
            methods.append(shippingMethod)
        }
        
        return methods
    }
}
