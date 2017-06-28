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
        checkoutView.actionButton.addTarget(self, action: #selector(didPressCancel), for: .touchUpInside)
        
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
        delegate?.willDismissWebCheckout()
        self.dismiss(animated: true) {
            self.delegate?.didDismissWebCheckout()
        }
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
        
        if let url = webView.url, isThankYouPage(url) {
            // Change the color and title of the button
            checkoutView.isFinished = true
            delegate?.didCompletePurchase()
            return
        }
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

// MARK: Views

// View contents of the checkout view controller.
private class WebCheckoutView: UIView {
    var isFinished = false {
        didSet {
            actionButton.checkoutState = isFinished ? .finished : .checkingOut
        }
    }
    
    private let cornerRadius: CGFloat = 3
    private let viewMargin: CGFloat = 10
    private let buttonHeight: CGFloat = 46
    
    // Colors from https://polaris.shopify.com/visuals/colors#color-usage
    
    private(set) var webView: WKWebView!
    private(set) var actionButton: CheckoutButton!
    private(set) var loadingView: LoadingView!
    
    fileprivate var initialLoading = false
    
    override init(frame: CGRect) {
        webView = WKWebView()
        webView.translatesAutoresizingMaskIntoConstraints = false
        webView.layer.masksToBounds = true
        webView.layer.cornerRadius = cornerRadius
        
        actionButton = CheckoutButton(cornerRadius: cornerRadius)
        
        loadingView = LoadingView()
        loadingView.translatesAutoresizingMaskIntoConstraints = false
        loadingView.layer.cornerRadius = cornerRadius
        
        super.init(frame: frame)
        
        backgroundColor = .clear
        
        addSubview(webView)
        addSubview(actionButton)
        addSubview(loadingView)
        
        NSLayoutConstraint.activate([
            webView.topAnchor.constraint(equalTo: topAnchor, constant: 20),
            webView.leadingAnchor.constraint(equalTo: leadingAnchor, constant: viewMargin),
            webView.trailingAnchor.constraint(equalTo: trailingAnchor, constant: -viewMargin),
            
            actionButton.leadingAnchor.constraint(equalTo: leadingAnchor, constant: viewMargin),
            actionButton.trailingAnchor.constraint(equalTo: trailingAnchor, constant: -viewMargin),
            actionButton.topAnchor.constraint(equalTo: webView.bottomAnchor, constant: viewMargin),
            actionButton.bottomAnchor.constraint(equalTo: bottomAnchor, constant: -viewMargin),
            actionButton.heightAnchor.constraint(equalToConstant: buttonHeight),
            
            loadingView.topAnchor.constraint(equalTo: webView.topAnchor),
            loadingView.bottomAnchor.constraint(equalTo: webView.bottomAnchor),
            loadingView.trailingAnchor.constraint(equalTo: webView.trailingAnchor),
            loadingView.leadingAnchor.constraint(equalTo: webView.leadingAnchor)
        ])
    }
    
    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
    
    func startInitialLoad() {
        loadingView.alpha = 1
        loadingView.startLoadingAnimations()
        initialLoading = true
    }
    
    func endInitialLoad() {
        UIView.animate(withDuration: 0.3) {
            self.loadingView.alpha = 0
            self.initialLoading = false
            self.loadingView.stopLoadingAnimation()
        }
    }
}

private enum CheckoutState {
    case checkingOut
    case finished
    
    var highlightColor: UIColor {
        switch self {
        case .checkingOut: return UIColor(hex: "FEAF9A")
        case .finished: return UIColor(hex: "BBE5B3")
        }
    }
    
    var normalColor: UIColor {
        switch self {
        case .checkingOut: return UIColor(hex: "ED6347")
        case .finished: return UIColor(hex: "50B83C")
        }
    }
    
    var title: String {
        switch self {
        case .checkingOut: return "Cancel"
        case .finished: return "Finish"
        }
    }
}

// Encapsulates the changing of the colors and state of the bottom button.
private class CheckoutButton: UIButton {
    var checkoutState: CheckoutState {
        didSet {
            updateButton()
        }
    }
    
    override var isHighlighted: Bool {
        get {
            return super.isHighlighted
        }
        set (value) {
            super.isHighlighted = value
            updateButton()
        }
    }
    
    init(cornerRadius: CGFloat) {
        checkoutState = .checkingOut
        super.init(frame: CGRect.zero)
        setTitleColor(.white, for: .normal)
        layer.cornerRadius = cornerRadius
        translatesAutoresizingMaskIntoConstraints = false
        titleLabel?.font = UIFont.boldSystemFont(ofSize: 16)
        
        updateButton()
    }
    
    private func updateButton() {
        setTitle(checkoutState.title, for: .normal)
        backgroundColor = isHighlighted ? checkoutState.highlightColor : checkoutState.normalColor
    }
    
    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
}


// Loading view overlay presented on top of the web view when its first loading.
private class LoadingView: UIView {
    private var indicator: UIActivityIndicatorView!
    
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
    
    func startLoadingAnimations() {
        indicator.startAnimating()
    }
    
    func stopLoadingAnimation() {
        indicator.stopAnimating()
    }
    
    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
}
