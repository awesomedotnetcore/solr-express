﻿using SolrExpress.Search;
using SolrExpress.Search.Parameter;
using System.Collections.Generic;

namespace SolrExpress.Solr4.Search.Parameter
{
    public sealed class QueryFieldParameter<TDocument> : IQueryFieldParameter<TDocument>, ISearchItemExecution<List<string>>
        where TDocument : Document
    {
        private string _result;
        
        public string Expression { get; set; }

        public void AddResultInContainer(List<string> container)
        {
            container.Add(this._result);
        }

        public void Execute()
        {
            this._result = $"qf={this.Expression}";
        }
    }
}
