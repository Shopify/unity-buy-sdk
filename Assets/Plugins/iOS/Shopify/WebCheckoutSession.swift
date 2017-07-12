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
import SafariServices

private var activeWebPaySession: WebCheckoutSession?

@objc class WebCheckoutSession: NSObject {
    static func createSession(unityDelegateObjectName: String, url: String) -> WebCheckoutSession {
        let session = WebCheckoutSession(unityDelegateObjectName: unityDelegateObjectName,
                                         checkoutURL: url)
        activeWebPaySession = session
        return session
    }
    
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
        
        let webViewController = SFSafariViewController(url: url)
        webViewController.delegate = self

        let navController = WebCheckoutNavigationController(rootViewController: webViewController)
        navController.isNavigationBarHidden = true
        UnityAppController.root.present(navController, animated: true, completion: nil)

        return true
    }
}

extension WebCheckoutSession: SFSafariViewControllerDelegate {
    public func safariViewControllerDidFinish(_ controller: SFSafariViewController) {
        let message = UnityMessage(content: "dismissed", object: unityDelegateObjectName, method: "OnNativeMessage")
        MessageCenter.send(message)
    }
}

private extension UnityAppController {
    // A small helper variable for getting a handle to the root view controller.
    static var root: UIViewController {
        let unityController = UIApplication.shared.delegate as! UnityAppController
        return unityController.rootViewController
    }
}

// Small helper class for presenting the SFSafariViewController
private class WebCheckoutNavigationController: UINavigationController {
        
    // Only allow the same rotations the underlying game allows.
    override var supportedInterfaceOrientations: UIInterfaceOrientationMask {
        return UnityAppController.root.supportedInterfaceOrientations
    }
}
