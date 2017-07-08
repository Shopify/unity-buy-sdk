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
    
    fileprivate var url: URL
    fileprivate var progressBar: WebPageProgressIndicator!
    fileprivate var actionButton: UIBarButtonItem!
    fileprivate var checkoutLabelItem: UIBarButtonItem!
    
    private var kvoContext = 0
    
    fileprivate var webView: WKWebView {
        return view as! WKWebView
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
        self.view = WKWebView(frame: view.frame)
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        webView.navigationDelegate = self
        setupNavigationBar()
        loadCheckoutURL()
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        webView.addObserver(self, forKeyPath: #keyPath(WKWebView.estimatedProgress), options: .new, context: &kvoContext)
    }
    
    override func viewDidDisappear(_ animated: Bool) {
        super.viewDidDisappear(animated)
        webView.removeObserver(self, forKeyPath: #keyPath(WKWebView.estimatedProgress))
    }
    
    override func observeValue(forKeyPath keyPath: String?, of object: Any?, change: [NSKeyValueChangeKey : Any]?, context: UnsafeMutableRawPointer?) {
        if keyPath == "estimatedProgress" && context == &kvoContext {
            progressBar.progress = CGFloat(webView.estimatedProgress)
        } else {
            super.observeValue(forKeyPath: keyPath, of: object, change: change, context: context)
        }
    }
}
    
}

// MARK: Actions
extension Checkout.WebViewController {
    func setupNavigationBar() {
        actionButton = UIBarButtonItem(title: "Cancel", style: .plain, target: self, action: #selector(didTapCancel))
        navigationItem.rightBarButtonItem = actionButton
        
        let titleView = Checkout.TitleView()
        navigationItem.titleView = titleView
        
        progressBar = Checkout.WebPageProgressIndicator(fillColor: view.tintColor)
        progressBar.translatesAutoresizingMaskIntoConstraints = false
        
        if let navBar = self.navigationController?.navigationBar {
            navBar.addSubview(progressBar)
            NSLayoutConstraint.activate([
                progressBar.trailingAnchor.constraint(equalTo: navBar.trailingAnchor),
                progressBar.leadingAnchor.constraint(equalTo: navBar.leadingAnchor),
                progressBar.bottomAnchor.constraint(equalTo: navBar.bottomAnchor),
                progressBar.heightAnchor.constraint(equalToConstant: 2)
            ])
        }
    }
    
    func loadCheckoutURL() {
        let request = URLRequest(url: url)
        webView.load(request)
    }
    
    func didTapCancel() {
        delegate?.willDismiss { _ in
            self.dismiss(animated: true)
        }
    }
    
    func didTapDone() {
        // TODO: Will include more logic around done vs cancelled.
        delegate?.willDismiss { _ in
            self.dismiss(animated: true)
        }
    }
}

// MARK: WKNavigationDelegate
extension Checkout.WebViewController: WKNavigationDelegate {
    public func webView(_ webView: WKWebView, didStartProvisionalNavigation navigation: WKNavigation!) {
        progressBar.progress = 0
        progressBar.alpha = 1
    }
    
    public func webView(_ webView: WKWebView, didFailProvisionalNavigation navigation: WKNavigation!, withError error: Error) {}
    
    public func webView(_ webView: WKWebView, didCommit navigation: WKNavigation!) {
        if let url = webView.url, isThankYouPage(url) {
            delegate?.didLoadThankYouPage()
            actionButton = UIBarButtonItem(barButtonSystemItem: .done, target: self, action: #selector(didTapDone))
            navigationItem.rightBarButtonItem = actionButton
            return
        }
    }
    
    public func webView(_ webView: WKWebView, didFinish navigation: WKNavigation!) {
        progressBar.alpha = 0
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
