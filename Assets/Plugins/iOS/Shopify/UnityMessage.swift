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

enum UnityMessageField: String {
    case message    = "Message"
    case identifier = "Identifier"
}

@objc class UnityMessage: NSObject {
    
    public typealias MessageCompletion = (_ result: String?) -> Void

    let recipientObjectName: String
    let recipientMethodName: String
    let content: String
    let identifier: String
    
    internal let semaphore: DispatchSemaphore
    
    var result: String?
    
    // ----------------------------------
    //  MARK: - Init -
    //
    init(content: String, recipientObjectName: String, recipientMethodName: String) {
        self.recipientObjectName = recipientObjectName
        self.recipientMethodName = recipientMethodName
        self.content    = content
        self.identifier = UUID().uuidString
        self.semaphore  = DispatchSemaphore(value: 0)
        super.init()
    }
    
    func wait(withCompletion completion: MessageCompletion?) {
        DispatchQueue.global(qos: .userInitiated).async {
            self.semaphore.wait()
            completion?(self.result)
        }
    }
    
    func complete(withResult result: String? = nil) {
        self.result = result
        semaphore.signal()
    }
}

extension UnityMessage: Serializable {
    func serializedJSON() -> JSON {
        var json = JSON.init()
        json[UnityMessageField.identifier.rawValue] = identifier
        json[UnityMessageField.message.rawValue]    = content
        return json
    }
}
