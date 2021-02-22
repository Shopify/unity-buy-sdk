//
//  MessageTests.swift
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

import XCTest
@testable import UnityBuySDKPlugin

class MessageTests: XCTestCase {
    
    let timeout = 10.0
    
    override func setUp() {
        super.setUp()
        if Tester.hasLoaded == false {
            let didLoad = self.expectation(description: "Tester failed to load")
            Tester.completion = {
                didLoad.fulfill()
            }
            self.wait(for: [didLoad], timeout: timeout)
        }
    }
    
    func testSendMessage() {
        let unityObject = TesterName
        let content     = "Test String{/!#^3;'][>}"
        let method      = TesterMethod.repeatMessage.rawValue
        let message     = UnityMessage(content: content, object: unityObject, method: method)
        let expectation = self.expectation(description: "MessageCenter.send(TesterMethod.repeatMessage) failed to complete")
        
        MessageCenter.send(message) { response in
            XCTAssertEqual(message.content, response)
            expectation.fulfill()
        }
        
        XCTAssertEqual(message, MessageCenter.message(forIdentifier: message.identifier))
        self.wait(for: [expectation], timeout: timeout)
    }
}
