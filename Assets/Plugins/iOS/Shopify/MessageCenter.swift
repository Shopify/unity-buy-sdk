//
//  MessageCenter.swift
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

@objc class MessageCenter: NSObject {
    
    private static var messages = [String: UnityMessage]()
    
    static func send(_ message: UnityMessage) {
        MessageSender.sendMessage(try! message.serializedString(), object: message.object, method: message.method)
    }
    
    static func send(_ message: UnityMessage, completionHandler: UnityMessage.MessageCompletion?) {
        
        messages[message.identifier] = message
        
        MessageSender.sendMessage(try! message.serializedString(), object: message.object, method: message.method)
        
        message.wait { response in
            messages[message.identifier] = nil
            completionHandler?(response)
        }
    }
    
    static func message(forIdentifier identifier: String) -> UnityMessage? {
        return messages[identifier]
    }
}
