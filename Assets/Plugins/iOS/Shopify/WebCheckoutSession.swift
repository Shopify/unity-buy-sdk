//
//  WebCheckoutSession.swift
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
import WebKit

private var activeWebPaySession: WebCheckoutSession?

protocol WebCheckoutDelegate: class {
    func didLoadThankYouPage()
    func willDismiss(completionHandler: UnityMessage.MessageCompletion?)
}

@objc class WebCheckoutSession: NSObject {
    static func createSession(unityDelegateObjectName: String, url: String) -> WebCheckoutSession {
        let session = WebCheckoutSession(unityDelegateObjectName: unityDelegateObjectName,
                                         checkoutURL: url)
        activeWebPaySession = session
        return session
    }
    
    fileprivate var webViewController: Checkout.WebViewController!
    
    private let checkoutURL: String
    fileprivate let unityDelegateObjectName: String
    
    init(unityDelegateObjectName: String, checkoutURL: String) {
        self.unityDelegateObjectName = unityDelegateObjectName
        self.checkoutURL = checkoutURL
        
        super.init()
    }
    
    func startCheckout() -> Bool {
        guard let url = URL(string: checkoutURL) else {
            return false
        }
        
        webViewController = Checkout.WebViewController(url: url, delegate: self)
        let navController = UINavigationController(rootViewController: webViewController)
        
        UnityAppController.root.present(navController, animated: true, completion: nil)
        
        return true
    }
}

extension WebCheckoutSession: WebCheckoutDelegate {
    func didLoadThankYouPage() {
        let message = UnityMessage(content: "loadedThankYouPage", object: unityDelegateObjectName, method: "OnNativeMessage")
        MessageCenter.send(message)
    }
    
    func willDismiss(completionHandler: UnityMessage.MessageCompletion?) {
        let message = UnityMessage(content: "cancelled", object: unityDelegateObjectName, method: "OnNativeMessage")
        MessageCenter.send(message, completionHandler: completionHandler)
    }
}

private extension UnityAppController {
    // A small helper variable for getting a handle to the root view controller.
    static var root: UIViewController {
        let unityController = UIApplication.shared.delegate as! UnityAppController
        return unityController.rootViewController
    }
}
