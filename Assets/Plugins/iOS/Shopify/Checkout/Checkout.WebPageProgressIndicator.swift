//
//  Checkout.WebPageProgressIndicator.swift
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

extension Checkout {

// Custom loading indicator that it shown at the top when loading a web page.
class WebPageProgressIndicator: UIView {
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

}
