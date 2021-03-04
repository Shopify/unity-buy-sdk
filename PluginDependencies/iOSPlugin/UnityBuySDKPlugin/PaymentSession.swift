//
//  PaymentSession.swift
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
import PassKit

enum PaymentStatus: Int, CustomStringConvertible {
    case failed
    case cancelled
    case success
    
    var description: String {
        switch self {
        case .cancelled:
            return "Cancelled"
        case .failed:
            return "Failed"
        case .success:
            return "Success"
        }
    }
}

protocol PaymentSessionDelegate : class {
    func paymentSessionDidFinish(session: PaymentSession, with status: PaymentStatus)
    
    func paymentSession(_ session: PaymentSession, didSelect shippingMethod: PKShippingMethod, completion: @escaping (PKPaymentAuthorizationStatus, [PKPaymentSummaryItem]) -> Void)
    
    func paymentSession(_ session: PaymentSession, didSelectShippingContact shippingContact: PKContact, completion: @escaping (PKPaymentAuthorizationStatus, [PKShippingMethod], [PKPaymentSummaryItem]) -> Void)
    
    func paymentSession(_ session: PaymentSession, didAuthorize payment: PKPayment, completion: @escaping (PKPaymentAuthorizationStatus) -> Void)
    
    @available(iOS 11.0, *)
    func paymentSession(_ session: PaymentSession, didAuthorize payment: PKPayment, handler completion: @escaping (PKPaymentAuthorizationResult) -> Void)
    
    @available(iOS 11.0, *)
    func paymentSession(_ session: PaymentSession, didSelect shippingMethod: PKShippingMethod, handler completion: @escaping (PKPaymentRequestShippingMethodUpdate) -> Void)
    
    @available(iOS 11.0, *)
    func paymentSession(_ session: PaymentSession, didSelectShippingContact contact: PKContact, handler completion: @escaping (PKPaymentRequestShippingContactUpdate) -> Void)
}
    

public class PaymentSession : NSObject {
    
    let request: PKPaymentRequest
    var controller: PKPaymentAuthorizationController
    
    /// Whether Apple is in the process of authenticating the payment request
    var isAuthenticating: Bool = false
    
    /// Whether Apple and Shopify has authenticated the payment request
    var hasAuthenticated: Bool = false
    
    weak var delegate: PaymentSessionDelegate?
    
    // ----------------------------------
    //  MARK: - Init -
    //
    init?(
        merchantId: String,
        countryCode: String,
        currencyCode: String,
        requiringShippingAddressFields: Bool,
        supportedNetworks: [PKPaymentNetwork],
        summaryItems: [PKPaymentSummaryItem],
        shippingMethods:[PKShippingMethod]?,
        controllerType: PKPaymentAuthorizationController.Type = PKPaymentAuthorizationController.self)
    {
        request = PKPaymentRequest()
        request.countryCode                   = countryCode
        request.currencyCode                  = currencyCode
        request.merchantIdentifier            = merchantId
        request.requiredBillingAddressFields  = .all
        request.merchantCapabilities          = PaymentSession.capabilities
        request.supportedNetworks             = supportedNetworks
        request.requiredShippingAddressFields = requiringShippingAddressFields ?
            .all : PKAddressField(rawValue: PKAddressField.email.rawValue | PKAddressField.phone.rawValue)
        
        request.paymentSummaryItems = summaryItems
        request.shippingMethods     = shippingMethods
    
        self.controller = controllerType.init(paymentRequest: request)
        
        super.init();
        
        self.controller.delegate = self
    }
    
    /// Presents the PKAuthorizationController with the current shipping methods
    /// and summary items in this session.
    /// UnityAppController is set to remain active until the authorization controller
    /// is dismissed, to enable communication between managed and unmanaged functions
    func presentAuthorizationController()  {
        UnityBuyAppControllerSetShouldResign(false)

        controller.present(completion: nil)
    }
}

// ----------------------------------
//  MARK: - Static functions -
//
extension PaymentSession {
    
    static let capabilities: PKMerchantCapability = [.capability3DS, .capabilityDebit, .capabilityCredit]
    
    static func canMakePayments(usingNetworks networks: [PKPaymentNetwork]) -> Bool {
        return PKPaymentAuthorizationViewController.canMakePayments() &&
            PKPaymentAuthorizationViewController.canMakePayments(usingNetworks: networks, capabilities: capabilities)
    }
    
    static func canShowSetup(forNetworks networks: [PKPaymentNetwork]) -> Bool {
        return PKPaymentAuthorizationViewController.canMakePayments() &&
            !PKPaymentAuthorizationViewController.canMakePayments(usingNetworks: networks, capabilities: capabilities)
    }
    
    static func showSetup() {
        PKPassLibrary().openPaymentSetup()
    }
}

extension PaymentSession: PKPaymentAuthorizationControllerDelegate {
    public func paymentAuthorizationControllerDidFinish(_ controller: PKPaymentAuthorizationController) {
        let paymentStatus: PaymentStatus
        
        if (isAuthenticating) {
            paymentStatus = PaymentStatus.failed
        } else if (hasAuthenticated) {
            paymentStatus = PaymentStatus.success
        } else {
            paymentStatus = PaymentStatus.cancelled
        }
        
        
        self.controller.dismiss {
            self.delegate?.paymentSessionDidFinish(session: self, with: paymentStatus)
        }
    }
    
    public func paymentAuthorizationControllerWillAuthorizePayment(_ controller: PKPaymentAuthorizationController) {
        isAuthenticating = true
    }
    
    @available(iOS, introduced: 10.0, deprecated: 11.0)
    public func paymentAuthorizationController(_ controller: PKPaymentAuthorizationController, didSelectShippingMethod shippingMethod: PKShippingMethod, completion: @escaping (PKPaymentAuthorizationStatus, [PKPaymentSummaryItem]) -> Void) {
        delegate?.paymentSession(self, didSelect: shippingMethod, completion: completion)
    }
    
    @available(iOS, introduced: 10.0, deprecated: 11.0)
    public func paymentAuthorizationController(_ controller: PKPaymentAuthorizationController, didSelectShippingContact contact: PKContact, completion: @escaping (PKPaymentAuthorizationStatus, [PKShippingMethod], [PKPaymentSummaryItem]) -> Void) {
        delegate?.paymentSession(self, didSelectShippingContact: contact, completion: completion)
    }
    
    @available(iOS, introduced: 10.0, deprecated: 11.0)
    public func paymentAuthorizationController(_ controller: PKPaymentAuthorizationController, didAuthorizePayment payment: PKPayment, completion: @escaping (PKPaymentAuthorizationStatus) -> Swift.Void) {
        isAuthenticating = false
        
        delegate?.paymentSession(self, didAuthorize: payment, completion: { status in
            
            if (status == PKPaymentAuthorizationStatus.success) {
                self.hasAuthenticated = true
            } else {
                self.hasAuthenticated = false
            }
            
            completion(status)
        })
    }
    
    @available(iOS 11.0, *)
    public func paymentAuthorizationController(_ controller: PKPaymentAuthorizationController, didAuthorizePayment payment: PKPayment, handler completion: @escaping (PKPaymentAuthorizationResult) -> Void) {
        isAuthenticating = false
        
        delegate?.paymentSession(self, didAuthorize: payment, handler: { result in
            
            if (result.status == PKPaymentAuthorizationStatus.success) {
                self.hasAuthenticated = true
            } else {
                self.hasAuthenticated = false
            }
            
            completion(result)
        })
    }
    
    @available(iOS 11.0, *)
    public func paymentAuthorizationController(_ controller: PKPaymentAuthorizationController, didSelectShippingMethod shippingMethod: PKShippingMethod, handler completion: @escaping (PKPaymentRequestShippingMethodUpdate) -> Swift.Void) {
        delegate?.paymentSession(self, didSelect: shippingMethod, handler: completion)
    }
    
    @available(iOS 11.0, *)
    public func paymentAuthorizationController(_ controller: PKPaymentAuthorizationController, didSelectShippingContact contact: PKContact, handler completion: @escaping (PKPaymentRequestShippingContactUpdate) -> Swift.Void) {
        delegate?.paymentSession(self, didSelectShippingContact: contact, handler: completion)
    }
}
