//
//  Checkout.TitleView.swift
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

class TitleView: UIView {
    private let titleLabel: UILabel
    private let lockImageView: UIImageView
    
    override init(frame: CGRect) {
        titleLabel = UILabel()
        titleLabel.translatesAutoresizingMaskIntoConstraints = false
        titleLabel.text = "checkout.shopify.com"
        titleLabel.textColor = .darkGray
        titleLabel.font = UIFont.systemFont(ofSize: 16)
        titleLabel.sizeToFit()
        
        // TODO: Use actual lock image
        lockImageView = UIImageView(frame: CGRect(x: 0, y: 0, width: 25, height: 25))
        lockImageView.translatesAutoresizingMaskIntoConstraints = false
        lockImageView.backgroundColor = .red

        super.init(frame: frame)
        
        addSubview(titleLabel)
        addSubview(lockImageView)
        
        NSLayoutConstraint.activate([
            titleLabel.centerXAnchor.constraint(equalTo: centerXAnchor),
            titleLabel.centerYAnchor.constraint(equalTo: centerYAnchor),
            
            lockImageView.trailingAnchor.constraint(equalTo: titleLabel.leadingAnchor, constant: -5),
            lockImageView.centerYAnchor.constraint(equalTo: titleLabel.centerYAnchor),
            lockImageView.heightAnchor.constraint(equalToConstant: 25),
            lockImageView.widthAnchor.constraint(equalToConstant: 25)
        ])
    }
    
    required init?(coder aDecoder: NSCoder) {
        fatalError("init(coder:) has not been implemented")
    }
}
    
}
