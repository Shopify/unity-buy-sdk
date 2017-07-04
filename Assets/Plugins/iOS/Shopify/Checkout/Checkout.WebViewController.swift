//
//  Checkout.WebViewController.swift
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

import UIKit
import WebKit

extension Checkout {

// MARK: View Controller Logic
class WebViewController: UIViewController {
    weak var delegate: WebCheckoutDelegate?
    
    fileprivate(set) var url: URL
    
    fileprivate var contentView: WebContentView {
        return view as! WebContentView
    }
    
    fileprivate var webView: WKWebView {
        return contentView.webView
    }
    
    init(url: URL, delegate: WebCheckoutDelegate) {
        self.url = url
        self.delegate = delegate
        
        super.init(nibName: nil, bundle: nil)
    }
    
    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
    
    override func loadView() {
        super.loadView()
        self.view = WebContentView(frame: view.frame)
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        webView.navigationDelegate = self
        contentView.setActionButton(target: self, selector: #selector(didPressCancel))
        
        loadCheckoutURL()
    }
}
    
}

// MARK: Actions
extension Checkout.WebViewController {
    func loadCheckoutURL() {
        let request = URLRequest(url: url)
        webView.load(request)
    }
    
    func didPressCancel() {
        delegate?.willDismiss { _ in
            self.dismiss(animated: true)
        }
    }
}

// MARK: WKNavigationDelegate
extension Checkout.WebViewController: WKNavigationDelegate {
    public func webView(_ webView: WKWebView, didStartProvisionalNavigation navigation: WKNavigation!) {
        contentView.showProgressIndicator()
    }
    
    public func webView(_ webView: WKWebView, didFailProvisionalNavigation navigation: WKNavigation!, withError error: Error) {}
    
    public func webView(_ webView: WKWebView, didCommit navigation: WKNavigation!) {
        if let url = webView.url, isThankYouPage(url) {
            delegate?.didLoadThankYouPage()
            contentView.isFinished = true
            return
        }
    }
    
    public func webView(_ webView: WKWebView, didFinish navigation: WKNavigation!) {
        contentView.hideProgressIndicator()
    }
    
    public func webView(_ webView: WKWebView, didFail navigation: WKNavigation!, withError error: Error) {}
    
    public func webView(_ webView: WKWebView, didReceive challenge: URLAuthenticationChallenge,
                        completionHandler: @escaping (URLSession.AuthChallengeDisposition, URLCredential?) -> Swift.Void) {
        completionHandler(.performDefaultHandling, nil)
    }
    
    public func webViewWebContentProcessDidTerminate(_ webView: WKWebView) {}
    
    private func isThankYouPage(_ url: URL) -> Bool {
        return url.absoluteString.hasSuffix("/thank_you")
    }
}
