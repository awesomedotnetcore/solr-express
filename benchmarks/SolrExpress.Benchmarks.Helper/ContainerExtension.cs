﻿using SimpleInjector;
using SolrExpress.Builder;
using SolrExpress.Search;
using SolrExpress.Search.Behaviour;
using SolrExpress.Search.Query;
using SolrExpress.Search.Result;
using SolrExpress.Update;
using System;

namespace SolrExpress.Benchmarks.Helper
{
    /// <summary>
    /// Configure SolrExpress DI using SimpleInjector engine
    /// </summary>
    public static class ContainerExtension
    {
        public static Container AddSolrExpressFake<TDocument>(this Container container, Action<SolrExpressBuilder<TDocument>> builder)
            where TDocument : Document
        {
            var solrExpressServiceProvider = (ISolrExpressServiceProvider<TDocument>)new SolrExpressServiceProvider<TDocument>();
            var solrExpressBuilder = new SolrExpressBuilder<TDocument>(solrExpressServiceProvider);

            builder.Invoke(solrExpressBuilder);

            container.Register<ISolrExpressServiceProvider<TDocument>>(() => solrExpressServiceProvider, Lifestyle.Singleton);
            container.Register<DocumentCollection<TDocument>>(Lifestyle.Singleton);

            solrExpressServiceProvider
                .AddSingleton(solrExpressBuilder.Options)
                .AddTransient(solrExpressServiceProvider);

            var solrConnection = new FakeSolrConnection<TDocument>();
            var expressionBuilder = new ExpressionBuilder<TDocument>(solrExpressBuilder.Options, solrConnection);
            expressionBuilder.LoadDocument();

            solrExpressServiceProvider
                .AddTransient(expressionBuilder)
                .AddTransient<DocumentSearch<TDocument>>()
                .AddTransient<DocumentUpdate<TDocument>>()
                .AddTransient<SearchResultBuilder<TDocument>>()
                .AddTransient<SearchQuery<TDocument>>()
                .AddTransient<ISolrConnection>(solrConnection)
                .AddTransient<IDocumentResult<TDocument>, DocumentResult<TDocument>>()
                .AddTransient<IInformationResult<TDocument>, InformationResult<TDocument>>()
                .AddTransient<IChangeDynamicFieldBehaviour<TDocument>, ChangeDynamicFieldBehaviour<TDocument>>();

            return container;
        }
    }
}
