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
    fileprivate var overlay: UIView?
    
    private(set) var checkoutURL: String
    private(set) var unityDelegateObjectName: String
    
    init(unityDelegateObjectName: String, checkoutURL: String) {
        self.unityDelegateObjectName = unityDelegateObjectName
        self.checkoutURL = checkoutURL
        self.overlay = {
            let view = UIView(frame: UIScreen.main.bounds)
            view.autoresizingMask = [.flexibleHeight, .flexibleWidth]
            view.backgroundColor = .black
            view.alpha = 0
            return view
        }()
        
        super.init()
    }
    
    func startCheckout() -> Bool {
        let root = UnityAppController.root
        guard let url = URL(string: checkoutURL) else {
            return false;
        }
        
        webViewController = WebCheckoutViewController(url: url, delegate: self)
        webViewController!.modalPresentationStyle = .overFullScreen
        
        toggleOverlay(show: true, onVC: root)
        
        willPresentWebCheckout()
        toggleOverlay(show: true, onVC: root)
        root.present(webViewController!, animated: true) {
            self.didPresentWebCheckout()
        }
        
        return true;
    }
    
    fileprivate func toggleOverlay(show: Bool, onVC vc: UIViewController) {
        guard let overlay = overlay else {
            return
        }
        
        if show {
            vc.view.addSubview(overlay)
            vc.view.bringSubview(toFront: overlay)
        }
        
        UIView.animate(withDuration: 0.4, animations: { 
            self.overlay?.alpha = show ? 0.3 : 0
        }) { _ in
            if !show {
                self.overlay?.removeFromSuperview()
                self.overlay = nil
            }
        }
    }
}

extension WebCheckoutSession: WebCheckoutDelegate {
    func willPresentWebCheckout() {
        let message = UnityMessage(content: "willPresent", object: unityDelegateObjectName, method: "OnUIStateChanged")
        MessageCenter.send(message)
    }
        
    func didPresentWebCheckout() {
        let message = UnityMessage(content: "presenting", object: unityDelegateObjectName, method: "OnUIStateChanged")
        MessageCenter.send(message)
    }
        
    func willDismissWebCheckout() {
        toggleOverlay(show: false, onVC: UnityAppController.root)
        let message = UnityMessage(content: "willDismiss", object: unityDelegateObjectName, method: "OnUIStateChanged")
        MessageCenter.send(message)
    }
    
    func didDismissWebCheckout() {
        let message = UnityMessage(content: "dismissed", object: unityDelegateObjectName, method: "OnUIStateChanged")
        MessageCenter.send(message)
    }
        
    func didCompletePurchase() {
        let message = UnityMessage(content: "", object: unityDelegateObjectName, method: "DidCompletePurchase")
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
