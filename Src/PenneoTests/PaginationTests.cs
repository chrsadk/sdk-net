using System;
using System.Linq;
using NUnit.Framework;
using Penneo;
using Penneo.Connector;
using Penneo.Util;

namespace PenneoTests
{
    [TestFixture]
    public class PaginationTests
    {
        [Test]
        public void ParseLinkResponseHeaderTest()
        {
            var queryResult = new QueryResult<CaseFile>();
            const string linkHeader = "Link=<https://app.penneo.com/api/v1/casefiles?page=17&per_page=10>; rel=\"next\",<https://app.penneo.com/api/v1/casefiles?page=1&per_page=10>; rel=\"first\",<https://app.penneo.com/api/v1/casefiles?page=15&per_page=10>; rel=\"prev\"";
            PaginationUtil.ParseRepsonseHeadersForPagination(linkHeader, queryResult);
            Assert.AreEqual(17, queryResult.NextPage);
            Assert.AreEqual(15, queryResult.PrevPage);
            Assert.AreEqual(1, queryResult.FirstPage);
        }

        [Test]
        public void PageNotZeroTest()
        {
            ApiConnector.SetFactory(null);
            Assert.That(() => ((ApiConnector)ApiConnector.Instance).PrepareRequest(string.Empty, page: 0, perPage: 10), 
                Throws.TypeOf<NotSupportedException>());
            
        }

        [Test]
        public void PerPageNotZeroTest()
        {
            ApiConnector.SetFactory(null);
            Assert.That(() => ((ApiConnector)ApiConnector.Instance).PrepareRequest(string.Empty, page: 5, perPage: 0),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void PageNotLessThanZeroTest()
        {
            ApiConnector.SetFactory(null);
            Assert.That(() => ((ApiConnector)ApiConnector.Instance).PrepareRequest(string.Empty, page: -2, perPage: 10),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void PerPageNotLessThanZeroTest()
        {
            ApiConnector.SetFactory(null);
            Assert.That(() => ((ApiConnector)ApiConnector.Instance).PrepareRequest(string.Empty, page: 5, perPage: -5),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void PaginateRequestParametersTest()
        {
            ApiConnector.SetFactory(null);

            var connector = (ApiConnector) ApiConnector.Instance;
            var request = connector.PrepareRequest(string.Empty, page: 5, perPage: 10);

            var paginationHeader = request.Parameters.FirstOrDefault(x => x.Name.Equals("x-paginate"));
            Assert.IsTrue(paginationHeader != null && paginationHeader.Value.ToString().Equals("true", StringComparison.OrdinalIgnoreCase));

            var pageParameter = request.Parameters.FirstOrDefault(x => x.Name.Equals("page"));
            Assert.IsTrue(pageParameter != null && pageParameter.Value.ToString() == "5");

            var perPageParameter = request.Parameters.FirstOrDefault(x => x.Name.Equals("per_page"));
            Assert.IsTrue(perPageParameter != null && perPageParameter.Value.ToString() == "10");
        }

        [Test]
        public void NoPaginateRequestParametersTest()
        {
            ApiConnector.SetFactory(null);

            var connector = (ApiConnector)ApiConnector.Instance;
            var request = connector.PrepareRequest(string.Empty);

            var paginationHeader = request.Parameters.FirstOrDefault(x => x.Name.Equals("x-paginate"));
            Assert.IsTrue(paginationHeader == null);

            var pageParameter = request.Parameters.FirstOrDefault(x => x.Name.Equals("page"));
            Assert.IsTrue(pageParameter == null);

            var perPageParameter = request.Parameters.FirstOrDefault(x => x.Name.Equals("per_page"));
            Assert.IsTrue(perPageParameter == null);
        }
    }
}
