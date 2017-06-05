//
//  UnityMessageSender.swift
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

typealias MessageCompletion = (_ result: String?) -> Void

@objc class UnityMessageSender: NSObject {

    static var pendingMessageResponses = [String: UnityMessageResponse]()
    
    static func sendMessage(_ message: String, toObject objectName: String, havingMethod methodName: String) -> String? {
        
        let message  = UnityMessage.init(message: message);
        let response = UnityMessageResponse.init(identifier: message.identifier, completion: DispatchSemaphore(value: 0))
        pendingMessageResponses[response.identifier] = response
        
        UnityInterfaceWrapper.sendMessage(try! message.serializedString(), toObject: objectName, havingMethodName: methodName)
        
        response.completion.wait()
        pendingMessageResponses[response.identifier] = nil
        
        if let result = response.result {
            return result
        }
        
        return nil
    }
    
    static func sendMessage(_ message: String, toObject objectName: String, havingMethod methodName: String, completionHandler: MessageCompletion?) {
        
        let message  = UnityMessage.init(message: message);
        let response = UnityMessageResponse.init(identifier: message.identifier, completion: DispatchSemaphore(value: 0))
        pendingMessageResponses[response.identifier] = response

        UnityInterfaceWrapper.sendMessage(try! message.serializedString(), toObject: objectName, havingMethodName: methodName)

        DispatchQueue.global(qos: .userInitiated).async {
            response.completion.wait()
            pendingMessageResponses[response.identifier] = nil
            
            completionHandler?(response.result)
        }
    }
}
