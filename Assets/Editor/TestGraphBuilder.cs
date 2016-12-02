namespace Shopify.Tests
{
    using NUnit.Framework;
    using APISchema;

    [TestFixture]
    public class TestGraphQLGenerator {
        [Test]
        public void queryRootBuildsQuery() {
            QueryRootQuery query = Root.buildQuery();
            query.shop(s =>
                s.name()
            );

            Assert.AreEqual(query.ToString(), "{shop{name}}");
        }
    }    
}
