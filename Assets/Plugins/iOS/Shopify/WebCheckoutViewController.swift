//
//  WebCheckoutViewController.swift
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
    }
    
    func didPressCancel() {
        delegate?.willDismiss { _ in
            self.dismiss(animated: true)
        }
    }
}

// MARK: WKNavigationDelegate
extension WebCheckoutViewController: WKNavigationDelegate {
    public func webView(_ webView: WKWebView, didStartProvisionalNavigation navigation: WKNavigation!) {
        checkoutView.showProgressIndicator()
    }
    
    public func webView(_ webView: WKWebView, didFailProvisionalNavigation navigation: WKNavigation!, withError error: Error) {}
    
    public func webView(_ webView: WKWebView, didCommit navigation: WKNavigation!) {
        if let url = webView.url, isThankYouPage(url) {
            delegate?.didLoadThankYouPage()
            self.checkoutView.isFinished = true
            return
        }
    }
    
    public func webView(_ webView: WKWebView, didFinish navigation: WKNavigation!) {
        checkoutView.hideProgressIndicator()
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
    private let loadingIndicatorOffset: CGFloat = 5
    private let loadingIndicatorTiming: TimeInterval = 0.3
    
    private(set) var containerView: UIView!
    private(set) var webView: WKWebView!
    private(set) var actionButton: CheckoutButton!
    private(set) var progressIndicator: WebPageProgressIndicator!
    private var progressTopConstraint: NSLayoutConstraint!
    
    override init(frame: CGRect) {
        containerView = UIView()
        containerView.translatesAutoresizingMaskIntoConstraints = false
        containerView.layer.masksToBounds = true
        containerView.layer.cornerRadius = cornerRadius
        containerView.backgroundColor = .white
        
        webView = WKWebView()
        webView.translatesAutoresizingMaskIntoConstraints = false
        
        actionButton = CheckoutButton(cornerRadius: cornerRadius)
        actionButton.translatesAutoresizingMaskIntoConstraints = false
        
        progressIndicator = WebPageProgressIndicator(cornerRadius: 3)
        progressIndicator.translatesAutoresizingMaskIntoConstraints = false
        progressIndicator.alpha = 0
        
        super.init(frame: frame)
        
        webView.addObserver(self, forKeyPath: #keyPath(WKWebView.estimatedProgress), options: .new, context: nil)
        backgroundColor = .clear
        
        containerView.addSubview(webView)
        containerView.addSubview(progressIndicator)
        
        addSubview(containerView)
        addSubview(actionButton)
        
        var constraints = [
            containerView.topAnchor.constraint(equalTo: topAnchor, constant: 20),
            containerView.leadingAnchor.constraint(equalTo: leadingAnchor, constant: viewMargin),
            containerView.trailingAnchor.constraint(equalTo: trailingAnchor, constant: -viewMargin),
            
            progressIndicator.trailingAnchor.constraint(equalTo: containerView.trailingAnchor, constant: -5),
            progressIndicator.leadingAnchor.constraint(equalTo: containerView.leadingAnchor, constant: 5),
            progressIndicator.heightAnchor.constraint(equalToConstant: 10),
            
            webView.topAnchor.constraint(lessThanOrEqualTo: progressIndicator.bottomAnchor, constant: -5),
            webView.leadingAnchor.constraint(equalTo: containerView.leadingAnchor),
            webView.trailingAnchor.constraint(equalTo: containerView.trailingAnchor),
            webView.bottomAnchor.constraint(equalTo: containerView.bottomAnchor),
            
            actionButton.leadingAnchor.constraint(equalTo: leadingAnchor, constant: viewMargin),
            actionButton.trailingAnchor.constraint(equalTo: trailingAnchor, constant: -viewMargin),
            actionButton.topAnchor.constraint(equalTo: containerView.bottomAnchor, constant: viewMargin),
            actionButton.bottomAnchor.constraint(equalTo: bottomAnchor, constant: -viewMargin),
            actionButton.heightAnchor.constraint(equalToConstant: buttonHeight),
        ]
        
        progressTopConstraint = progressIndicator.topAnchor.constraint(equalTo: containerView.topAnchor, constant: -5)
        constraints.append(progressTopConstraint)
        
        NSLayoutConstraint.activate(constraints)
    }
    
    deinit {
        webView.removeObserver(self, forKeyPath: #keyPath(WKWebView.estimatedProgress))
    }
    
    func showProgressIndicator() {
        layoutIfNeeded()
        UIView.animate(withDuration: loadingIndicatorTiming) {
            self.progressTopConstraint.constant = self.loadingIndicatorOffset
            self.progressIndicator.alpha = 1
            self.layoutIfNeeded()
        }
    }
    
    func hideProgressIndicator() {
        layoutIfNeeded()
        UIView.animate(withDuration: loadingIndicatorTiming) {
            self.progressTopConstraint.constant = -self.loadingIndicatorOffset
            self.progressIndicator.alpha = 0
            self.layoutIfNeeded()
        }
    }
    
    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
    
    override func observeValue(forKeyPath keyPath: String?, of object: Any?, change: [NSKeyValueChangeKey : Any]?, context: UnsafeMutableRawPointer?) {
        if keyPath == "estimatedProgress" {
            progressIndicator.progress = CGFloat(webView.estimatedProgress)
        } else {
            super.observeValue(forKeyPath: keyPath, of: object, change: change, context: context)
        }
    }
}

private enum CheckoutState {
    case checkingOut
    case finished
    
    // Colors from https://polaris.shopify.com/visuals/colors#color-usage
    var highlightColor: UIColor {
        switch self {
        case .checkingOut: return UIColor(hex: "FEAF9A")!
        case .finished: return UIColor(hex: "BBE5B3")!
        }
    }
    
    var normalColor: UIColor {
        switch self {
        case .checkingOut: return UIColor(hex: "ED6347")!
        case .finished: return UIColor(hex: "50B83C")!
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

// Custom loading indicator that it shown at the top when loading a web page.
private class WebPageProgressIndicator: UIView {
    var progress: CGFloat = 0 {
        didSet {
            DispatchQueue.main.async {
                self.updateProgress()
            }
        }
    }
    
    private var fillView: UIView!
    private var trailingFillConstraint: NSLayoutConstraint!
    
    // Colors from https://polaris.shopify.com/visuals/colors#color-usage
    init(cornerRadius: CGFloat) {
        super.init(frame: CGRect.zero)
        backgroundColor = UIColor(hex: "B4E1FA")!
        layer.masksToBounds = true
        
        fillView = UIView()
        fillView.translatesAutoresizingMaskIntoConstraints = false
        fillView.backgroundColor = UIColor(hex: "007ACE")!
        
        addSubview(fillView)
        
        var constraints = [
            fillView.topAnchor.constraint(equalTo: topAnchor),
            fillView.bottomAnchor.constraint(equalTo: bottomAnchor),
            fillView.leadingAnchor.constraint(equalTo: leadingAnchor)
        ]
        
        trailingFillConstraint = fillView.trailingAnchor.constraint(equalTo: trailingAnchor, constant: 0)
        constraints.append(trailingFillConstraint)
        NSLayoutConstraint.activate(constraints)
        
        layer.cornerRadius = cornerRadius
    }
    
    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
    
    override func traitCollectionDidChange(_ previousTraitCollection: UITraitCollection?) {
        super.traitCollectionDidChange(previousTraitCollection)
        updateProgress()
    }
    
    private func updateProgress() {
        let progressWidth = frame.width * progress
        self.trailingFillConstraint.constant = -(self.frame.width - progressWidth)
        layoutIfNeeded()
    }
}
