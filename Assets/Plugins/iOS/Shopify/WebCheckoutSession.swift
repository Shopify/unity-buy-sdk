//
//  WebCheckoutSession.swift
//  Unity-iPhone
//
//  Created by Shopify.
// 
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
    func willPresentWebCheckout()
    func didPresentWebCheckout()
    
    func didDismissWebCheckout()
    func willDismissWebCheckout()
    
    func didCompletePurchase()
}

@objc class WebCheckoutSession: NSObject {
    static func createSession(unityDelegateObjectName: String, url: String) -> WebCheckoutSession {
        let session = WebCheckoutSession(unityDelegateObjectName: unityDelegateObjectName,
                                         checkoutURL: url)
        activeWebPaySession = session
        return session;
    }
    
    fileprivate var webViewController: WebCheckoutViewController?
    
    private(set) var checkoutURL: String
    private(set) var unityDelegateObjectName: String
    
    init(unityDelegateObjectName: String, checkoutURL: String) {
        self.unityDelegateObjectName = unityDelegateObjectName
        self.checkoutURL = checkoutURL
        super.init()
    }
    
    func startCheckout() -> Bool {
        let unityController = UIApplication.shared.delegate as! UnityAppController
        
        guard let url = URL(string: checkoutURL) else {
            return false;
        }
        
        webViewController = WebCheckoutViewController(url: url, delegate: self)
        
        willPresentWebCheckout()
        unityController.rootViewController.present(webViewController!, animated: true) {
            self.didPresentWebCheckout()
        }
        
        return true;
    }
}

extension WebCheckoutSession: WebCheckoutDelegate {
    func willPresentWebCheckout() {
        print("Will Present Web Checkout.")
    }
    
    func didPresentWebCheckout() {
        print("Did Present Web Checkout.")
    }
    
    func willDismissWebCheckout() {
        print("Will Dismiss Web Checkout.")
    }
    
    func didDismissWebCheckout() {
        print("Did Dismiss Web Checkout.")
    }
    
    func didCompletePurchase() {
        print("Did Complete Purchase.")
    }
}
