//
//  MockPaymentSessionDelegate.swift
//  UnityBuySDK
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

import Foundation
import PassKit
@testable import UnityBuySDKPlugin

@available(iOS 11.0, *)
final class MockPaymentSessionDelegate: NSObject {
    var onSessionDidFinish: ((PaymentSession, PaymentStatus) -> Void)?
    
    var onSessionDidSelectShippingMethod:
        ((PaymentSession, PKShippingMethod, (PKPaymentAuthorizationStatus, [PKPaymentSummaryItem]) -> Void) -> Void)?
    
    var onSessionDidSelectShippingContact:
        ((PaymentSession, PKContact, (PKPaymentAuthorizationStatus, [PKShippingMethod], [PKPaymentSummaryItem]) -> Void) -> Void)?
    
    var onSessionDidAuthorizePayment:
        ((PaymentSession, PKPayment, (PKPaymentAuthorizationStatus) -> Void) -> Void)?

    var onSessionDidSelectShippingContactUpdateRequest: ((PaymentSession, PKContact, (PKPaymentRequestShippingContactUpdate) -> Void) -> Void)?

    var onSessionDidSelectShippingMethodUpdateRequest: ((PaymentSession, PKShippingMethod, (PKPaymentRequestShippingMethodUpdate) -> Void) -> Void)?

    var onSessionDidAuthorizePaymentUpdateRequest: ((PaymentSession, PKPayment, (PKPaymentAuthorizationResult) -> Void) -> Void)?

}

// ----------------------------------
//  MARK: - PaymentSessionDelegate -
//
@available(iOS 11.0, *)
extension MockPaymentSessionDelegate: PaymentSessionDelegate {

    func paymentSessionDidFinish(session: PaymentSession, with status: PaymentStatus) {
        onSessionDidFinish?(session, status)
    }
    
    func paymentSession(_ session: PaymentSession, didSelect shippingMethod: PKShippingMethod, completion: @escaping (PKPaymentAuthorizationStatus, [PKPaymentSummaryItem]) -> Void) {
        onSessionDidSelectShippingMethod?(session, shippingMethod, completion)
    }
    
    func paymentSession(_ session: PaymentSession, didSelectShippingContact shippingContact: PKContact, completion: @escaping (PKPaymentAuthorizationStatus, [PKShippingMethod], [PKPaymentSummaryItem]) -> Void) {
        onSessionDidSelectShippingContact?(session, shippingContact, completion)
    }
    
    func paymentSession(_ session: PaymentSession, didAuthorize payment: PKPayment, completion: @escaping (PKPaymentAuthorizationStatus) -> Void) {
        onSessionDidAuthorizePayment?(session, payment, completion)
    }

    func paymentSession(_ session: PaymentSession, didAuthorize payment: PKPayment, handler completion: @escaping (PKPaymentAuthorizationResult) -> Void) {
        onSessionDidAuthorizePaymentUpdateRequest?(session, payment, completion)
    }

    func paymentSession(_ session: PaymentSession, didSelect shippingMethod: PKShippingMethod, handler completion: @escaping (PKPaymentRequestShippingMethodUpdate) -> Void) {
        onSessionDidSelectShippingMethodUpdateRequest?(session, shippingMethod, completion)
    }

    func paymentSession(_ session: PaymentSession, didSelectShippingContact contact: PKContact, handler completion: @escaping (PKPaymentRequestShippingContactUpdate) -> Void) {
        onSessionDidSelectShippingContactUpdateRequest?(session, contact, completion)
    }
}
