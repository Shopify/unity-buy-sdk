//
//  Checkout.WebContentView.swift
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

// View contents of the checkout view controller.
class WebContentView: UIView {
    var isFinished = false {
        didSet {
            actionButton.checkoutState = isFinished ? .finished : .checkingOut
        }
    }
    
    let webView: WKWebView
    
    private let cornerRadius: CGFloat = 3
    private let viewMargin: CGFloat = 10
    private let buttonHeight: CGFloat = 46
    private let loadingIndicatorOffset: CGFloat = 5
    private let loadingIndicatorTiming: TimeInterval = 0.3
    
    private let containerView: UIView
    private let actionButton: Button
    private let progressIndicator: WebPageProgressIndicator
    private var progressTopConstraint: NSLayoutConstraint!
    
    private var kvoContext = 0
    
    override init(frame: CGRect) {
        containerView = UIView()
        containerView.translatesAutoresizingMaskIntoConstraints = false
        containerView.layer.masksToBounds = true
        containerView.layer.cornerRadius = cornerRadius
        containerView.backgroundColor = .white
        
        webView = WKWebView()
        webView.translatesAutoresizingMaskIntoConstraints = false
        
        actionButton = Button(cornerRadius: cornerRadius)
        actionButton.translatesAutoresizingMaskIntoConstraints = false
        
        progressIndicator = WebPageProgressIndicator(cornerRadius: 3)
        progressIndicator.translatesAutoresizingMaskIntoConstraints = false
        progressIndicator.alpha = 0
        
        super.init(frame: frame)
        
        webView.addObserver(self, forKeyPath: #keyPath(WKWebView.estimatedProgress), options: .new, context: &kvoContext)
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
    
    func setActionButton(target: Any?, selector: Selector) {
        actionButton.addTarget(target, action: selector, for: .touchUpInside)
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
        if keyPath == "estimatedProgress" && context == &kvoContext {
            progressIndicator.progress = CGFloat(webView.estimatedProgress)
        } else {
            super.observeValue(forKeyPath: keyPath, of: object, change: change, context: context)
        }
    }
}

}
