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
    
    fileprivate(set) var startURL: URL
    fileprivate var lastGoodURL: URL
    
    fileprivate var contentView: WebContentView {
        return view as! WebContentView
    }
    
    fileprivate var webView: WKWebView {
        return contentView.webView
    }
    
    init(url: URL, delegate: WebCheckoutDelegate) {
        self.startURL = url
        self.lastGoodURL = url
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
        let request = URLRequest(url: startURL)
        webView.load(request)
    }
    
    func generateErrorPage(message: String, retryURL: URL) -> URL {
        // Instead of using webView.loadHTMLString, we generate a new HTML file and store on disk so 
        // we can use webView.loadFileURL which allows us differentiate between an error page and a 
        // web page. Using webView.loadHTMLString will set the url to about:blank which isn't strong enough
        // to tell us that it's not an error page.
        let errorPageFileURL = Bundle.main.url(forResource: "errorPage", withExtension: "html")!
        let polarisCSSFileURL = Bundle.main.url(forResource: "polaris-1.0.2", withExtension: "css")!
        
        let errorPageString = try! String(contentsOf: errorPageFileURL)
        let polarisCSSString = try! String(contentsOf: polarisCSSFileURL)
        
        var errorHTMLString = errorPageString.replacingOccurrences(of: "<%= polaris_css %>", with: polarisCSSString)
        errorHTMLString = errorHTMLString.replacingOccurrences(of: "<%= retry_url %>", with: lastGoodURL.absoluteString)
        errorHTMLString = errorHTMLString.replacingOccurrences(of: "<%= message %>", with: message)
        
        // Write the .html file to the temporary directory.
        let tempDir = NSTemporaryDirectory()
        let errorPageURL = URL(string: "file:///\(tempDir)/errorPage.html")!
        try! errorHTMLString.write(to: errorPageURL, atomically: true, encoding: .utf8)
        return errorPageURL
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
    
    public func webView(_ webView: WKWebView, didFailProvisionalNavigation navigation: WKNavigation!, withError error: Error) {
        handleLoadError(error as NSError)
    }
    
    public func webView(_ webView: WKWebView, didCommit navigation: WKNavigation!) {
        if let url = webView.url, isThankYouPage(url) {
            delegate?.didLoadThankYouPage()
            contentView.isFinished = true
            return
        }
    }
    
    public func webView(_ webView: WKWebView, didFinish navigation: WKNavigation!) {
        // If we're not showing a file:/// url (our error page), we're good.
        if let url = webView.url, !url.isFileURL {
            lastGoodURL = url
        }
        
        contentView.hideProgressIndicator()
    }
    
    public func webView(_ webView: WKWebView, didFail navigation: WKNavigation!, withError error: Error) {
        handleLoadError(error as NSError)
    }
    
    public func webView(_ webView: WKWebView, didReceive challenge: URLAuthenticationChallenge,
                        completionHandler: @escaping (URLSession.AuthChallengeDisposition, URLCredential?) -> Swift.Void) {
        completionHandler(.performDefaultHandling, nil)
    }
    
    public func webViewWebContentProcessDidTerminate(_ webView: WKWebView) {}
    
    private func isThankYouPage(_ url: URL) -> Bool {
        return url.absoluteString.hasSuffix("/thank_you")
    }
    
    private func handleLoadError(_ error: NSError) {
        let errorPageURL: URL
        switch error.code {
        case NSURLErrorNotConnectedToInternet:
            errorPageURL = generateErrorPage(message: "Looks like you don't have an internet connection. You won't be able to continue checking out until you do. Tap 'Reload' to try loading the page again.",
                                             retryURL: lastGoodURL)
        default:
            errorPageURL =  generateErrorPage(message: "We've run into an unknown issue. Tap 'Reload' to try loading the page again.",
                                              retryURL: lastGoodURL)
        }
        
        webView.loadFileURL(errorPageURL, allowingReadAccessTo: errorPageURL)
    }
}
