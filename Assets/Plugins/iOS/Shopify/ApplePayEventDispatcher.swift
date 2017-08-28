//
//  ApplePayEventDispatcher.swift
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

class ApplePayEventDispatcher: NSObject {
    
    typealias AppleEventCompletion = (_ response: ApplePayEventResponse?) -> Void
    
    let receiver: String
    
    // ----------------------------------
    //  MARK: - Init -
    //
    init(receiver: String) {
        self.receiver = receiver
        super.init()
    }
}

extension ApplePayEventDispatcher {
    fileprivate enum Method: String {
        case updateSummaryItemsForShippingIdentifier = "UpdateSummaryItemsForShippingIdentifier"
        case updateSummaryItemsForShippingContact    = "UpdateSummaryItemsForShippingContact"
        case fetchCheckoutStatusForToken             = "FetchApplePayCheckoutStatusForToken"
        case didFinishCheckoutSession                = "DidFinishCheckoutSession"
    }
    
    fileprivate func call(method: Method, value: String, completionHandler: AppleEventCompletion? = nil) {
        
        let message = UnityMessage(content: value, object: receiver, method: method.rawValue)
        
        if let completionHandler = completionHandler {
            
            MessageCenter.send(message) { response in
                if let response = response {
                    completionHandler(ApplePayEventResponse.deserialize(response))
                } else {
                    completionHandler(nil)
                }
            }
        } else {
            MessageCenter.send(message)
        }
    }
}

//  ----------------------------------
//  MARK: - PaymentSessionDelegate -
//
extension ApplePayEventDispatcher: PaymentSessionDelegate {
    
    func paymentSessionDidFinish(session: PaymentSession, with status: PaymentStatus) {
        call(method: .didFinishCheckoutSession, value: status.description)
    }
    
    func paymentSession(_ session: PaymentSession, didAuthorize payment: PKPayment, completion: @escaping (PKPaymentAuthorizationStatus) -> Void) {
        
        let paymentString = try! payment.serializedString()
        
        call(method: .fetchCheckoutStatusForToken, value: paymentString) { response in
            guard let response = response, let authStatus = response.authorizationStatus else {
                completion(.failure)
                return
            }
            
            completion(authStatus)
        }
    }
    
    func paymentSession(_ session: PaymentSession, didSelect shippingMethod: PKShippingMethod, completion: @escaping (PKPaymentAuthorizationStatus, [PKPaymentSummaryItem]) -> Void) {
        
        guard let identifier = shippingMethod.identifier else {
            completion(.failure, [])
            return
        }
        
        call(method: .updateSummaryItemsForShippingIdentifier, value: identifier) { response in
            guard
                let response     = response,
                let authStatus   = response.authorizationStatus,
                let summaryItems = response.summaryItems
            else {
                    completion(.failure, [])
                    return
            }
            completion(authStatus, summaryItems)
        }
    }
    
    func paymentSession(_ session: PaymentSession, didSelectShippingContact shippingContact: PKContact, completion: @escaping (PKPaymentAuthorizationStatus, [PKShippingMethod], [PKPaymentSummaryItem]) -> Void) {
        
        guard let contactString = try? shippingContact.serializedString() else {
            completion(.failure, [], [])
            return
        }
        
        call(method: .updateSummaryItemsForShippingContact, value: contactString) { response in
            guard
                let response        = response,
                let authStatus      = response.authorizationStatus,
                let summaryItems    = response.summaryItems,
                let shippingMethods = response.shippingMethods
            else {
                    completion(.failure, [], [])
                    return
            }
            completion(authStatus, shippingMethods, summaryItems)
        }
    }
}

@available(iOS 11.0, *)
extension ApplePayEventDispatcher  {
    func paymentSession(_ session: PaymentSession, didAuthorize payment: PKPayment, handler completion: @escaping (PKPaymentAuthorizationResult) -> Void) {
        let paymentString = try! payment.serializedString()
        
        call(method: .fetchCheckoutStatusForToken, value: paymentString) { response in
            guard
                let response   = response,
                let authStatus = response.authorizationStatus,
                let errors     = response.paymentErrors
            else {
                completion(PKPaymentAuthorizationResult(status: .failure, errors: nil))
                return
            }
            
            // Since response.authorizationStatus still supports iOS 11 and below
            // we can receive a deprecated auth status, in which should be a .failure
            var authStatusToReturn = authStatus
            
            if (errors.count > 0) {
                authStatusToReturn = .failure
            }
            
            completion(PKPaymentAuthorizationResult(status: authStatusToReturn, errors: errors))
        }
    }
    
    func paymentSession(_ session: PaymentSession, didSelect shippingMethod: PKShippingMethod, handler completion: @escaping (PKPaymentRequestShippingMethodUpdate) -> Void) {
        
        let failureUpdate = PKPaymentRequestShippingMethodUpdate(paymentSummaryItems: [])
        failureUpdate.status = .failure
        
        guard let identifier = shippingMethod.identifier else {
            completion(failureUpdate)
            return
        }
        
        call(method: .updateSummaryItemsForShippingIdentifier, value: identifier) { response in
            guard
                let response     = response,
                let authStatus   = response.authorizationStatus,
                let summaryItems = response.summaryItems
            else {
                completion(failureUpdate)
                return
            }
            
            let updateToReturn = PKPaymentRequestShippingMethodUpdate(paymentSummaryItems: summaryItems)
            updateToReturn.status = authStatus
            
            completion(updateToReturn)
        }
    }
    
    func paymentSession(_ session: PaymentSession, didSelectShippingContact contact: PKContact, handler completion: @escaping (PKPaymentRequestShippingContactUpdate) -> Void) {
        
        let failureUpdate = PKPaymentRequestShippingContactUpdate(errors: nil, paymentSummaryItems: [], shippingMethods: [])
        failureUpdate.status = .failure
        
        guard let contactString = try? contact.serializedString() else {
            completion(failureUpdate)
            return
        }
        
        call(method: .updateSummaryItemsForShippingContact, value: contactString) { response in
            guard
                let response        = response,
                let authStatus      = response.authorizationStatus,
                let summaryItems    = response.summaryItems,
                let shippingMethods = response.shippingMethods,
                let errors          = response.paymentErrors
            else {
                completion(failureUpdate)
                return
            }
            
            // Since response.authorizationStatus still supports iOS 11 and below
            // we can receive a deprecated auth status, in which should be a .failure
            var authStatusToReturn = authStatus
            
            if (errors.count > 0) {
                authStatusToReturn = .failure
            }
            
            let updateToReturn = PKPaymentRequestShippingContactUpdate(errors: errors, paymentSummaryItems: summaryItems, shippingMethods: shippingMethods)
            updateToReturn.status = authStatusToReturn
            
            completion(updateToReturn)
        }
    }
}
