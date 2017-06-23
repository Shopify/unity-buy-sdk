//
//  WebCheckoutViewController.swift
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

import UIKit
import WebKit

// MARK: View Controller Logic
class WebCheckoutViewController: UIViewController {
    weak var delegate: WebCheckoutDelegate?
    
    fileprivate(set) var url: URL
    
    fileprivate var checkoutView: WebCheckoutView {
        return view as! WebCheckoutView
    }
    
    fileprivate var webView: WKWebView {
        return checkoutView.webView
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
        self.view = WebCheckoutView(frame: UIScreen.main.bounds)
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        webView.navigationDelegate = self
        checkoutView.cancelButton.addTarget(self, action: #selector(didPressCancel), for: .touchUpInside)
        
        loadCheckoutURL()
    }
}

// MARK: Actions

extension WebCheckoutViewController {
    func loadCheckoutURL() {
        let request = URLRequest(url: url)
        webView.load(request)
        checkoutView.startInitialLoad()
    }
    
    func didPressCancel() {
        self.delegate?.didCancelCheckout()
    }
}

// MARK: WKNavigationDelegate
extension WebCheckoutViewController: WKNavigationDelegate {
    
    public func webView(_ webView: WKWebView, didStartProvisionalNavigation navigation: WKNavigation!) {}
    
    public func webView(_ webView: WKWebView, didFailProvisionalNavigation navigation: WKNavigation!, withError error: Error) {}
    
    public func webView(_ webView: WKWebView, didCommit navigation: WKNavigation!) {}
    
    public func webView(_ webView: WKWebView, didFinish navigation: WKNavigation!) {
        if checkoutView.initialLoading {
            // Add a bit of time for the page to render
            let after: DispatchTime = DispatchTime.now() + .seconds(1)
            DispatchQueue.main.asyncAfter(deadline: after) {
                self.checkoutView.endInitialLoad()
            }
            
            return
        }
        
        if let url = webView.url, isThankYou(url: url) {
            // Change the color and title of the button
            checkoutView.setButton(state: .finished)
            delegate?.didFinishCheckout()
            return
        }
    }
    
    public func webView(_ webView: WKWebView, didFail navigation: WKNavigation!, withError error: Error) {}
    
    public func webView(_ webView: WKWebView, didReceive challenge: URLAuthenticationChallenge,
                        completionHandler: @escaping (URLSession.AuthChallengeDisposition, URLCredential?) -> Swift.Void) {
        completionHandler(.performDefaultHandling, nil)
    }
    
    public func webViewWebContentProcessDidTerminate(_ webView: WKWebView) {}
    
    func isThankYou(url: URL) -> Bool {
        return url.absoluteString.hasSuffix("/thank_you")
    }
}

// MARK: Views

// View contents of the checkout view controller.
private class WebCheckoutView: UIView {
    enum CheckoutButtonState {
        case checkingOut
        case finished
    }
    
    private let cornerRadius: CGFloat = 5
    
    private(set) var webView: WKWebView!
    private(set) var cancelButton: UIButton!
    private(set) var loadingView: LoadingView!
    
    fileprivate var initialLoading: Bool = false
    
    override init(frame: CGRect) {
        webView = WKWebView()
        webView.translatesAutoresizingMaskIntoConstraints = false
        webView.layer.masksToBounds = true
        webView.layer.cornerRadius = cornerRadius
        
        cancelButton = UIButton(type: .custom)
        cancelButton.setTitleColor(.white, for: .normal)
        cancelButton.titleLabel?.font = UIFont.boldSystemFont(ofSize: 18)
        cancelButton.layer.cornerRadius = cornerRadius
        cancelButton.translatesAutoresizingMaskIntoConstraints = false
        
        loadingView = LoadingView()
        loadingView.translatesAutoresizingMaskIntoConstraints = false
        loadingView.layer.cornerRadius = cornerRadius
        
        super.init(frame: frame)
        
        addSubview(webView)
        addSubview(cancelButton)
        addSubview(loadingView)
        
        NSLayoutConstraint.activate([
            webView.topAnchor.constraint(equalTo: topAnchor, constant: 20),
            webView.leadingAnchor.constraint(equalTo: leadingAnchor, constant: 10),
            webView.trailingAnchor.constraint(equalTo: trailingAnchor, constant: -10),
            
            cancelButton.leadingAnchor.constraint(equalTo: leadingAnchor, constant: 10),
            cancelButton.trailingAnchor.constraint(equalTo: trailingAnchor, constant: -10),
            cancelButton.topAnchor.constraint(equalTo: webView.bottomAnchor, constant: 10),
            cancelButton.bottomAnchor.constraint(equalTo: bottomAnchor, constant: -10),
            
            loadingView.topAnchor.constraint(equalTo: webView.topAnchor),
            loadingView.bottomAnchor.constraint(equalTo: webView.bottomAnchor),
            loadingView.trailingAnchor.constraint(equalTo: webView.trailingAnchor),
            loadingView.leadingAnchor.constraint(equalTo: webView.leadingAnchor)
        ])
    }
    
    override func didMoveToWindow() {
        super.didMoveToWindow()
        setButton(state: .checkingOut)
    }
    
    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
    
    func setButton(state: CheckoutButtonState) {
        assert(Thread.isMainThread, "Updating the button must be done on the main thread.")
        switch state {
        case .checkingOut:
            self.cancelButton.backgroundColor = .red
            self.cancelButton.setTitle("Cancel", for: .normal)
        case .finished:
            self.cancelButton.backgroundColor = .green
            self.cancelButton.setTitle("Finished", for: .normal)
        }
    }
    
    func startInitialLoad() {
        loadingView.alpha = 1
        loadingView.indicator.startAnimating()
        initialLoading = true
    }
    
    func endInitialLoad() {
        UIView.animate(withDuration: 0.3) {
            self.loadingView.alpha = 0
            self.initialLoading = false
        }
    }
}

// Loading view overlay presented on top of the web view when its first loading.
private class LoadingView: UIView {
    private(set) var indicator: UIActivityIndicatorView!
    
    override init(frame: CGRect) {
        super.init(frame: frame)
        backgroundColor = .white
        
        indicator = UIActivityIndicatorView(activityIndicatorStyle: .gray)
        indicator.translatesAutoresizingMaskIntoConstraints = false
        addSubview(indicator)
        
        NSLayoutConstraint.activate([
            indicator.centerXAnchor.constraint(equalTo: centerXAnchor),
            indicator.centerYAnchor.constraint(equalTo: centerYAnchor)
        ])
    }
    
    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
}
