namespace Shopify.Tests
{
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.Unity.GraphQL;

    [TestFixture]
    public class TestDefaultShopQueries {
        [Test]
        public void TestPaymentSettings() {
            QueryRootQuery query = new QueryRootQuery();
            DefaultQueries.shop.PaymentSettings(query);

            Assert.AreEqual(
                "{shop {paymentSettings {countryCode currencyCode acceptedCardBrands supportedDigitalWallets }}}",
                query.ToString()
            );
        }

        [Test]
        public void TestName() {
            QueryRootQuery query = new QueryRootQuery();
            DefaultQueries.shop.Name(query);

            Assert.AreEqual(
                "{shop {name }}",
                query.ToString()
            );
        }
    }
}
