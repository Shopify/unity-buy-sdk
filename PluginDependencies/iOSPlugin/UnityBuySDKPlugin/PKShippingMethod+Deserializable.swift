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

extension PKShippingMethod {
    public class func deserializeShippingMethod(_ json: JSON) -> Self? {
        guard
            let label  = json[Field.label.rawValue] as? String,
            let amount = json[Field.amount.rawValue] as? String
        else {
            return nil
        }

        let shippingMethod = self.init(label: label, amount: NSDecimalNumber(string: amount))

        if let typeString = json[Field.type.rawValue] as? String,
            let type      = PKPaymentSummaryItemType(rawStringValue: typeString) {
           shippingMethod.type = type
        }

        shippingMethod.detail     = json[Field.detail.rawValue] as? String
        shippingMethod.identifier = json[Field.identifier.rawValue] as? String
        return shippingMethod
    }
    
    public class func deserializeShippingMethod(_ jsonCollection: [JSON]) -> [PKShippingMethod]? {
        let items = jsonCollection.compactMap {
            PKShippingMethod.deserializeShippingMethod($0)
        }
        
        if items.count < jsonCollection.count {
            return nil
        } else {
            return items
        }
    }
    
    public static func deserializeShippingMethod(_ string: String) -> PKShippingMethod? {
        guard
            let data = string.data(using: .utf8),
            let jsonObject = (try? JSONSerialization.jsonObject(with: data)) as? JSON
        else {
            return nil
        }

        return PKShippingMethod.deserializeShippingMethod(jsonObject)
    }
}

extension PKShippingMethod {
    fileprivate enum Field: String {
        case amount     = "Amount"
        case label      = "Label"
        case type       = "Type"
        case detail     = "Detail"
        case identifier = "Identifier"
    }
}
