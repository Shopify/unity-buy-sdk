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

@objc enum PaymentStatus: Int {
    case Failed
    case Cancelled
    
    func toString() -> String {
        switch self {
        case .Cancelled:
            return "Cancelled"
        case .Failed:
            return "Failed"
        }
    }
}

@objc protocol PaymentSessionDelegate : class {
    func paymentSessionDidFinish(session: PaymentSession,with status: PaymentStatus)
    func paymentSession(_ session: PaymentSession, didSelect shippingMethod: PKShippingMethod)
    func paymentSession(_ session: PaymentSession, didSelectShippingContact shippingContact: PKContact)
    func paymentSession(_ session: PaymentSession, didAuthorize payment: PKPayment)
}

@objc class PaymentSession: NSObject {

    /// Semaphore that blocks the PKAuthorizationController till summary items
    /// or shipping methods are updated based on the user's input
    let updateSemaphore = DispatchSemaphore(value: 0)
    
    let request: PKPaymentRequest
    
    /// The last status of the authorization received by passing the token data
    /// to the payment server
    var lastAuthStatus: PKPaymentAuthorizationStatus?
    
    var summaryItems: [PKPaymentSummaryItem]
    var shippingMethods: [PKShippingMethod]
    weak var delegate: PaymentSessionDelegate?
    
    // ----------------------------------
    //  MARK: - Init -
    //
    init(withMerchantIdentifier merchantId: String, countryCode: String, currencyCode: String, requiringShippingAddressFields: Bool)
    {
        summaryItems    = [PKPaymentSummaryItem]()
        shippingMethods = [PKShippingMethod]()
        
        request = PKPaymentRequest.init()
        request.countryCode                   = countryCode
        request.currencyCode                  = currencyCode
        request.merchantIdentifier            = merchantId
        request.requiredBillingAddressFields  = .all
        request.merchantCapabilities          = .capability3DS
        request.supportedNetworks             = PaymentSession.supportedNetworks
        request.requiredShippingAddressFields = requiringShippingAddressFields ?
            .all : PKAddressField.init(rawValue: PKAddressField.email.rawValue | PKAddressField.phone.rawValue)
        
        super.init()
    }
    
    func addSummaryItem(withLabel label : String, amount: NSDecimalNumber) {
        let newItem = PKPaymentSummaryItem.init(label: label, amount: amount)
        summaryItems.append(newItem)
    }
    
    func removeAllSummaryItems() {
        summaryItems.removeAll()
    }
    
    func addShippingMethod(withLabel label : String, amount: NSDecimalNumber, identifier: String, detail: String) {
        let newMethod        = PKShippingMethod.init(label: label, amount: amount)
        newMethod.identifier = identifier
        newMethod.detail     = detail
        
        shippingMethods.append(newMethod)
    }
    
    func removeAllShippingMethods() {
        shippingMethods.removeAll()
    }
    
    /// Assigns the shipping methods and summary items in this session to the request
    func populateRequest() {
        request.shippingMethods     = shippingMethods;
        request.paymentSummaryItems = summaryItems;
    }
    
    /// Presents the PKAuthorizationController with the current shipping methods
    /// and summary items in this session.
    /// UnityAppController is set to remain active until the authorization controller
    /// is dismissed, to enable communication between managed and unmanaged functions
    func presentAuthorizationController()  {
        populateRequest();
        let authViewController = PKPaymentAuthorizationViewController.init(paymentRequest: request)
        let unityController    = UIApplication.shared.delegate as! UnityBuyAppController
        let topController      = unityController.topMostController()!

        authViewController.delegate = self
        unityController.shouldResignActive = false
        topController.present(authViewController, animated: true)
    }
    
    /// Signals the update semaphore that items updates have finalized
    func didFinishUpdate() {
        updateSemaphore.signal()
    }
    
    /// Signal the update semaphore with the status returned from sending 
    /// the token data
    func didAuthorize(withStatus status: PKPaymentAuthorizationStatus) {
        self.lastAuthStatus = status;
        updateSemaphore.signal()
    }
}

// ----------------------------------
//  MARK: - Static functions -
//
extension PaymentSession {
    
    static let supportedNetworks: [PKPaymentNetwork] = [.amex, .masterCard, .visa, .discover]
    
    static func canMakePayments() -> Bool {
        return PKPaymentAuthorizationViewController.canMakePayments() &&
            PKPaymentAuthorizationViewController.canMakePayments(usingNetworks: PaymentSession.supportedNetworks)
    }
    
    static func canShowSetup() -> Bool {
        return PKPaymentAuthorizationViewController.canMakePayments() &&
            !PKPaymentAuthorizationViewController.canMakePayments(usingNetworks: PaymentSession.supportedNetworks)
    }
    
    static func showSetup() {
        PKPassLibrary.init().openPaymentSetup()
    }
}

// ----------------------------------
//  MARK: - PKPaymentAuthorizationViewControllerDelegate -
//
extension PaymentSession: PKPaymentAuthorizationViewControllerDelegate {
    
    func paymentAuthorizationViewControllerDidFinish(_ controller: PKPaymentAuthorizationViewController) {
        
        let paymentStatus: PaymentStatus
        let unityController = UIApplication.shared.delegate as! UnityBuyAppController
        unityController.shouldResignActive = true
        
        if lastAuthStatus == .failure {
            paymentStatus = .Failed
        }
        else {
            paymentStatus = .Cancelled
        }
        
        controller.dismiss(animated: true) {
            self.delegate?.paymentSessionDidFinish(session: self, with: paymentStatus)
        }
    }
    
    func paymentAuthorizationViewController(_ controller: PKPaymentAuthorizationViewController, didSelect shippingMethod: PKShippingMethod, completion: @escaping (PKPaymentAuthorizationStatus, [PKPaymentSummaryItem]) -> Void) {

        delegate?.paymentSession(self, didSelect: shippingMethod)
        
        DispatchQueue.global(qos: .userInitiated).async {
            self.updateSemaphore.wait()
            completion(.success, self.summaryItems)
        }
    }

    func paymentAuthorizationViewController(_ controller: PKPaymentAuthorizationViewController, didSelect paymentMethod: PKPaymentMethod, completion: @escaping ([PKPaymentSummaryItem]) -> Void) {
        completion(summaryItems)
    }
    
    func paymentAuthorizationViewController(_ controller: PKPaymentAuthorizationViewController, didSelectShippingContact contact: PKContact, completion: @escaping (PKPaymentAuthorizationStatus, [PKShippingMethod], [PKPaymentSummaryItem]) -> Void) {
        
        delegate?.paymentSession(self, didSelectShippingContact: contact)
        
        DispatchQueue.global(qos: .userInitiated).async {
            self.updateSemaphore.wait()
            completion(.success, self.shippingMethods, self.summaryItems);
        }
    }
    
    func paymentAuthorizationViewController(_ controller: PKPaymentAuthorizationViewController, didAuthorizePayment payment: PKPayment, completion: @escaping (PKPaymentAuthorizationStatus) -> Void) {
        
        delegate?.paymentSession(self, didAuthorize: payment)
        
        DispatchQueue.global(qos: .userInitiated).async {
            self.updateSemaphore.wait()
            completion(self.lastAuthStatus!);
        }
    }
}
