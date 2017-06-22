namespace Shopify.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.Unity.GraphQL;

    [TestFixture]
    public class TestDefaultQueries {
        [Test]
        public void TestCheckoutCreate() {
            MutationQuery query = new MutationQuery();
            List<CheckoutLineItemInput> lineItems = new List<CheckoutLineItemInput>();

            DefaultQueries.checkout.Create(query, lineItems);

            Assert.AreEqual(
                "mutation{checkoutCreate (input:{lineItems:[]}){checkout {id webUrl ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}userErrors {field message }}}",
                query.ToString()
            );
        }

        [Test]
        public void TestCheckoutPoll() {
            QueryRootQuery query = new QueryRootQuery();
            string checkoutId = "an-id";
            
            DefaultQueries.checkout.Poll(query, checkoutId);

            Assert.AreEqual(
                "{node (id:\"an-id\"){__typename ...on Checkout{id webUrl ready }}}",
                query.ToString()
            );
        }

        [Test]
        public void TestCheckoutLineItemsAdd() {
            MutationQuery query = new MutationQuery();
            string checkoutId = "an-id";
            List<CheckoutLineItemInput> lineItems = new List<CheckoutLineItemInput>();

            DefaultQueries.checkout.LineItemsAdd(query, checkoutId, lineItems);

            Assert.AreEqual(
                "mutation{checkoutLineItemsAdd (lineItems:[],checkoutId:\"an-id\"){checkout {id webUrl ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}userErrors {field message }}}",
                query.ToString()
            );
        }

        [Test]
        public void TestCheckoutLineItemsRemove() {
            MutationQuery query = new MutationQuery();
            string checkoutId = "an-id";
            List<string> lineItemIds = new List<string>();

            DefaultQueries.checkout.LineItemsRemove(query, checkoutId, lineItemIds);

            Assert.AreEqual(
                "mutation{checkoutLineItemsRemove (checkoutId:\"an-id\",lineItemIds:[]){userErrors {field message }}}",
                query.ToString()
            );
        }

        [Test]
        public void TestCheckoutLineItemsPage() {
            QueryRootQuery query = new QueryRootQuery();
            string checkoutId = "an-id";

            DefaultQueries.checkout.CheckoutLineItemsPage(query, checkoutId, 210, "after-something");

            Assert.AreEqual(
                "{node (id:\"an-id\"){__typename ...on Checkout{lineItems (first:210,after:\"after-something\"){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}}}",
                query.ToString()
            );
        }

        [Test]
        public void TestShippingAddressUpdate() {
            MutationQuery query = new MutationQuery();
            string checkoutId = "an-id";

            var addressInput = new MailingAddressInput("123 Test Street", "456", "Toronto", "Shopify", "Canada", "First", "Last", "1234567890", "Ontario", "A1B2C3");
            DefaultQueries.checkout.ShippingAddressUpdate(query, checkoutId, addressInput);

            Assert.AreEqual(
                "mutation{checkoutLineItemsRemove (checkoutId:\"an-id\",lineItemIds:[]){userErrors {field message }}}",
                query.ToString()
            );
        }

        [Test]
        public void TestEmailUpdate() {
            MutationQuery query = new MutationQuery();
            string checkoutId = "an-id";

            DefaultQueries.checkout.EmailUpdate(query, checkoutId, "test@shopify.com");

            Assert.AreEqual(
                "mutation{checkoutLineItemsRemove (checkoutId:\"an-id\",lineItemIds:[]){userErrors {field message }}}",
                query.ToString()
            );
        }

        [Test]
        public void TestShippingLineUpdate() {
            MutationQuery query = new MutationQuery();
            string checkoutId = "an-id";

            DefaultQueries.checkout.ShippingLineUpdate(query, checkoutId, "handle");

            Assert.AreEqual(
                "mutation{checkoutLineItemsRemove (checkoutId:\"an-id\",lineItemIds:[]){userErrors {field message }}}",
                query.ToString()
            );
        }
    }
}
