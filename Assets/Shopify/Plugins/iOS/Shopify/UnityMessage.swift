//
//  UnityMessage.swift
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


@objc public class UnityMessage: NSObject {
    
    typealias MessageCompletion = (_ response: String?) -> Void

    let object: String
    let method: String
    let content: String
    let identifier: String
    
    private let semaphore: DispatchSemaphore
    private(set) var response: String?
    
    // ----------------------------------
    //  MARK: - Init -
    //
    init(content: String, object: String, method: String) {
        self.object     = object
        self.method     = method
        self.content    = content
        self.identifier = UUID().uuidString
        self.semaphore  = DispatchSemaphore(value: 0)
        super.init()
    }
    
    func wait(with completion: MessageCompletion?) {
        DispatchQueue.global(qos: .userInitiated).async {
            self.semaphore.wait()
            completion?(self.response)
        }
    }
    
    @objc public func complete(with response: String? = nil) {
        self.response = response
        semaphore.signal()
    }
}

extension UnityMessage {
    fileprivate enum Field: String {
        case content    = "Content"
        case identifier = "Identifier"
    }
}

extension UnityMessage: Serializable {
    func serializedJSON() -> JSON {
        var json = JSON.init()
        json[Field.identifier.rawValue] = identifier
        json[Field.content.rawValue]    = content
        return json
    }
}
