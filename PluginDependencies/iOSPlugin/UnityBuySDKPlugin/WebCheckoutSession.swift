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

protocol WebCheckoutSessionDelegate {
    func webCheckoutSessionPresentsController(controller: UIViewController)
}

@objc public class WebCheckoutSession: NSObject {
    @objc public static func createSession(unityDelegateObjectName: String, url: String) -> WebCheckoutSession {
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
    
    @objc public func startCheckout() -> Bool {
        guard let url = URL(string: checkoutURL) else {
            return false
        }
        
        HTTPCookieStorage.shared.cookieAcceptPolicy = HTTPCookie.AcceptPolicy.always
        let webViewController = SFSafariViewController(url: url)
        webViewController.delegate = self

        let navController = UnityModalNavigationController(rootViewController: webViewController)
        navController.isNavigationBarHidden = true
        
        UnityGetGLViewController().present(navController, animated: true, completion: nil)
        
        return true
    }
}

extension WebCheckoutSession: SFSafariViewControllerDelegate {
    public func safariViewControllerDidFinish(_ controller: SFSafariViewController) {
        let message = UnityMessage(content: "dismissed", object: unityDelegateObjectName, method: "OnNativeMessage")
        MessageCenter.send(message)
    }
}

//// Helper subclass that matches the Unity controller's configuration.
private class UnityModalNavigationController: UINavigationController {
    override var supportedInterfaceOrientations: UIInterfaceOrientationMask {
        return UnityGetGLViewController().supportedInterfaceOrientations
    }

    override var preferredStatusBarStyle: UIStatusBarStyle {
        return UnityGetGLViewController().preferredStatusBarStyle
    }

    override var prefersStatusBarHidden: Bool {
        return UnityGetGLViewController().prefersStatusBarHidden
    }
}
