namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System;
    using Shopify.Unity.GraphQL;

    public class DefaultCheckoutQueries {
        public void Create(MutationQuery query, List<CheckoutLineItemInput> lineItems) {
            query.checkoutCreate(
                buildQuery: checkoutCreate => checkoutCreate
                .checkout(checkout => {
                    Checkout(checkout);
                    CheckoutLineItems(checkout);
                })
                .checkoutUserErrors(userErrors => userErrors
                    .code()
                    .field()
                    .message()
                ),
                input : new CheckoutCreateInput(
                    allowPartialAddresses: true,
                    lineItems: lineItems
                )
            );
        }

        public void CheckoutCompleteWithTokenizedPaymentV2(MutationQuery query, string checkoutId, TokenizedPaymentInputV2 tokenizedPaymentInputV2) {
            query.checkoutCompleteWithTokenizedPaymentV2(
                buildQuery: checkoutCompleteWithTokenizedPaymentV2 => checkoutCompleteWithTokenizedPaymentV2
                .checkout(checkout => Checkout(checkout))
                .payment(payment => Payment(payment))
                .userErrors(userErrors => userErrors
                    .field()
                    .message()
                ),
                checkoutId : checkoutId,
                payment : tokenizedPaymentInputV2
            );
        }

        public void Poll(QueryRootQuery query, string checkoutId) {
            query.node(
                buildQuery: node => node
                .onCheckout(checkout => Checkout(checkout)),
                id : checkoutId
            );
        }

        public void Completed(QueryRootQuery query, string checkoutId) {
            query.node(
                buildQuery: node => node
                .onCheckout(checkout => checkout.completedAt()),
                id : checkoutId
            );
        }

        public void PaymentPoll(QueryRootQuery query, string paymentId) {
            query.node(
                buildQuery: node => node
                .onPayment(payment => Payment(payment)),
                id : paymentId
            );
        }

        public void AvailableShippingRatesPoll(QueryRootQuery query, string checkoutId) {
            query.node(
                buildQuery: node => node
                .onCheckout(checkout => Checkout(checkout)
                    .availableShippingRates(availableShippingRates => AvailableShippingRates(availableShippingRates))
                ),
                id : checkoutId
            );
        }

        public void LineItemsRemove(MutationQuery query, string checkoutId, List<string> lineItemIds) {
            query.checkoutLineItemsRemove(
                buildQuery: lineItemRemove => lineItemRemove
                .userErrors(userErrors => userErrors
                    .field()
                    .message()
                ),
                checkoutId : checkoutId,
                lineItemIds : lineItemIds
            );
        }

        public void LineItemsAdd(MutationQuery query, string checkoutId, List<CheckoutLineItemInput> lineItems) {
            query.checkoutLineItemsAdd(
                buildQuery: lineItemAdd => lineItemAdd
                .checkout(checkout => {
                    Checkout(checkout);
                    CheckoutLineItems(checkout);
                })
                .userErrors(userErrors => userErrors
                    .field()
                    .message()
                ),
                checkoutId : checkoutId,
                lineItems : lineItems
            );
        }

        public void CheckoutLineItemsPage(QueryRootQuery query, string checkoutId, int first = 250, string after = null) {
            query.node(
                buildQuery: node => node.onCheckout(checkout => CheckoutLineItems(checkout, first, after)),
                id : checkoutId
            );
        }

        public void ShippingAddressUpdate(MutationQuery query, string checkoutId, MailingAddressInput shippingAddress) {
            query.checkoutShippingAddressUpdateV2(
                buildQuery: shippingAddressUpdate => shippingAddressUpdate
                .checkout(checkout => Checkout(checkout))
                .userErrors(userErrors => userErrors
                    .field()
                    .message()
                ),
                shippingAddress : shippingAddress,
                checkoutId : checkoutId
            );
        }

        public void EmailUpdate(MutationQuery query, string checkoutId, String email) {
            query.checkoutEmailUpdateV2(
                buildQuery: emailUpdateV2 => emailUpdateV2
                .checkout(checkout => checkout
                    .email())
                .userErrors(userErrors => userErrors
                    .field()
                    .message()
                ),
                checkoutId : checkoutId,
                email : email
            );
        }

        public void ShippingLineUpdate(MutationQuery query, string checkoutId, String shippingRateHandle) {
            query.checkoutShippingLineUpdate(
                buildQuery: emailUpdate => emailUpdate
                .checkout(checkout => Checkout(checkout)
                    .shippingLine(shippingLine => ShippingRate(shippingLine)))
                .userErrors(userErrors => userErrors
                    .field()
                    .message()
                ),
                checkoutId : checkoutId,
                shippingRateHandle : shippingRateHandle
            );
        }

        private CheckoutQuery Checkout(CheckoutQuery checkout) {
            MoneyV2Delegate moneyV2Query = (queryBuilder) => {
                queryBuilder.amount().currencyCode();
            };

            return checkout
                .id()
                .webUrl()
                .currencyCode()
                .requiresShipping()
                .subtotalPriceV2(moneyV2Query)
                .totalTaxV2(moneyV2Query)
                .totalPriceV2(moneyV2Query)
                .ready();
        }

        private PaymentQuery Payment(PaymentQuery payment) {
            return payment
                .checkout(checkout => Checkout(checkout)
                    .completedAt()
                )
                .errorMessage()
                .id()
                .ready();
        }

        private ShippingRateQuery ShippingRate(ShippingRateQuery shippingRate) {
            return shippingRate
                .handle()
                .title()
                .price();
        }

        private CheckoutQuery CheckoutLineItems(CheckoutQuery checkout, int first = 250, string after = null) {
            return checkout.lineItems(
                buildQuery: lineItems => lineItems
                .edges(edge => edge
                    .node(node => node
                        .id()
                        .variant(variant => variant
                            .id()
                        )
                    )
                    .cursor()
                )
                .pageInfo(pageInfo => pageInfo
                    .hasNextPage()
                ),
                first : first,
                after : after
            );
        }

        private AvailableShippingRatesQuery AvailableShippingRates(AvailableShippingRatesQuery availableShippingRates) {
            return availableShippingRates
                .shippingRates(
                    buildQuery: shippingRate => ShippingRate(shippingRate)
                )
                .ready();
        }
    }
}
