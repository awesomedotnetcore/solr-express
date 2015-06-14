﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrExpress.Core.ParameterValue;
using SolrExpress.Core.Query;
using SolrExpress.Solr5.Builder;
using SolrExpress.Solr5.Parameter;
using System.Collections.Generic;

namespace SolrExpress.Solr5.IntegrationTests
{
    [TestClass]
    public class IntegrationTests
    {
        /// <summary>
        /// Where   Creating a SOLR context, only creting provider and solr query classes
        /// When    Invoking the method "Execute"
        /// What    Create a communication between software and SOLR
        /// </summary>
        [TestMethod]
        public void IntegrationTest001()
        {
            // Arrange
            var provider = new Provider("http://localhost:8983/solr/techproducts");
            var solrQuery = new SolrQueryable<TechProductDocument>(provider);

            // Act / Assert
            solrQuery.Execute();
        }

        /// <summary>
        /// Where   Creating a SOLR context, using parameter "Query"
        /// When    Invoking the method "Execute"
        /// What    Create a communication between software and SOLR
        /// </summary>
        [TestMethod]
        public void IntegrationTest002()
        {
            // Arrange
            var provider = new Provider("http://localhost:8983/solr/techproducts");
            var solrQuery = new SolrQueryable<TechProductDocument>(provider);
            SolrQueryResult result;
            List<TechProductDocument> data;

            // Act
            solrQuery.Parameter(new QueryParameter(new QueryAll()));
            result = solrQuery.Execute();
            data = result.Get(new DocumentBuilder<TechProductDocument>()).Data;

            // Assert
            Assert.AreEqual(10, data.Count);
            Assert.AreEqual("GB18030TEST", data[0].Id);
        }

        /// <summary>
        /// Where   Creating a SOLR context, using parameter "Query" and "Filter" (twice)
        /// When    Invoking the method "Execute"
        /// What    Create a communication between software and SOLR
        /// </summary>
        [TestMethod]
        public void IntegrationTest003()
        {
            // Arrange
            var provider = new Provider("http://localhost:8983/solr/techproducts");
            var solrQuery = new SolrQueryable<TechProductDocument>(provider);
            SolrQueryResult result;
            List<TechProductDocument> data;

            // Act
            solrQuery.Parameter(new QueryParameter(new QueryAll()));
            solrQuery.Parameter(new FilterParameter(new SingleValue<TechProductDocument>(q => q.InStock, "true")));
            solrQuery.Parameter(new FilterParameter(new SingleValue<TechProductDocument>(q => q.ManufacturerId, "corsair")));
            result = solrQuery.Execute();
            data = result.Get(new DocumentBuilder<TechProductDocument>()).Data;

            // Assert
            Assert.AreEqual(3, data.Count);
            Assert.AreEqual("TWINX2048-3200PRO", data[0].Id);
        }

        /// <summary>
        /// Where   Creating a SOLR context, using parameter "Query" and "Facet.Field" (twice)
        /// When    Invoking the method "Execute"
        /// What    Create a communication between software and SOLR
        /// </summary>
        [TestMethod]
        public void IntegrationTest004()
        {
            // Arrange
            var provider = new Provider("http://localhost:8983/solr/techproducts");
            var solrQuery = new SolrQueryable<TechProductDocument>(provider);
            SolrQueryResult result;
            List<FacetKeyValue<string>> data;

            // Act
            solrQuery.Parameter(new QueryParameter(new QueryAll()));
            solrQuery.Parameter(new FacetFieldParameter<TechProductDocument>(q => q.ManufacturerId));
            solrQuery.Parameter(new FacetFieldParameter<TechProductDocument>(q => q.InStock));
            result = solrQuery.Execute();
            data = result.Get(new FacetFieldResultBuilder()).Data;

            // Assert
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual("ManufacturerId", data[0].Name);
            Assert.AreEqual("InStock", data[1].Name);
        }

        /// <summary>
        /// Where   Creating a SOLR context, using parameter "Query" and "Facet.Query" (twice)
        /// When    Invoking the method "Execute"
        /// What    Create a communication between software and SOLR
        /// </summary>
        [TestMethod]
        public void IntegrationTest005()
        {
            // Arrange
            var provider = new Provider("http://localhost:8983/solr/techproducts");
            var solrQuery = new SolrQueryable<TechProductDocument>(provider);
            SolrQueryResult result;
            Dictionary<string, long> data;

            // Act
            solrQuery.Parameter(new QueryParameter(new QueryAll()));
            solrQuery.Parameter(new FacetQueryParameter("Facet1", new RangeValue<TechProductDocument, decimal>(q => q.Popularity, from: 10)));
            solrQuery.Parameter(new FacetQueryParameter("Facet2", new RangeValue<TechProductDocument, decimal>(q => q.Popularity, to: 10)));
            result = solrQuery.Execute();
            data = result.Get(new FacetQueryResultBuilder()).Data;

            // Assert
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual(2, data["Facet1"]);
            Assert.AreEqual(15, data["Facet2"]);
        }

        /// <summary>
        /// Where   Creating a SOLR context, using parameter "Query" and "Facet.Range" (twice)
        /// When    Invoking the method "Execute"
        /// What    Create a communication between software and SOLR
        /// </summary>
        [TestMethod]
        public void IntegrationTest006()
        {
            // Arrange
            var provider = new Provider("http://localhost:8983/solr/techproducts");
            var solrQuery = new SolrQueryable<TechProductDocument>(provider);
            SolrQueryResult result;
            List<FacetKeyValue<FacetRange>> data;

            // Act
            solrQuery.Parameter(new QueryParameter(new QueryAll()));
            solrQuery.Parameter(new FacetRangeParameter<TechProductDocument>(q => q.Popularity, "Facet1", "1", "1", "10"));
            solrQuery.Parameter(new FacetRangeParameter<TechProductDocument>(q => q.Price, "Facet2", "10", "10", "1000"));
            result = solrQuery.Execute();
            data = result.Get(new FacetRangeResultBuilder()).Data;

            // Assert
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual("Facet1", data[0].Name);
            Assert.AreEqual("Facet2", data[1].Name);
        }

        /// <summary>
        /// Where   Creating a SOLR context, using parameter "Query" and result Statistic
        /// When    Invoking the method "Execute"
        /// What    Create a communication between software and SOLR
        /// </summary>
        [TestMethod]
        public void IntegrationTest007()
        {
            // Arrange
            var provider = new Provider("http://localhost:8983/solr/techproducts");
            var solrQuery = new SolrQueryable<TechProductDocument>(provider);
            SolrQueryResult result;
            StatisticResultBuilder data;

            // Act
            solrQuery.Parameter(new QueryParameter(new QueryAll()));
            result = solrQuery.Execute();
            data = result.Get(new StatisticResultBuilder());

            // Assert
            Assert.AreEqual(32, data.DocumentCount);
            Assert.IsFalse(data.IsEmpty);
        }
    }
}