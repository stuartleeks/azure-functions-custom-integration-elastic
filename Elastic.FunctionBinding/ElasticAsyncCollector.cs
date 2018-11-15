using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elastic.FunctionBinding;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Nest;

namespace Elastic.FunctionBinding
{
    public class ElasticAsyncCollector : IAsyncCollector<IElasticMessage>
    {
        private readonly ElasticAttribute _attribute;
        private readonly ElasticClient _client;
        private List<IElasticMessage> _documents = new List<IElasticMessage>();

        public ElasticAsyncCollector(ElasticClient client, ElasticAttribute a)
        {
            _client = client;
            _attribute = a;
        }

        public Task AddAsync(IElasticMessage item, CancellationToken cancellationToken = default(CancellationToken))
        {
            _documents.Add(item);
            //await _client.IndexAsync(item.Value, r => r.Index(_attribute.Index).Type(_attribute.Type));
            // TODO - handle logging on failure, throw exception, ...?

            return Task.CompletedTask;
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var bulk = new BulkDescriptor();
            foreach (var document in _documents)
            {
                document.AddToBulkDescriptor(bulk, _attribute.Index, _attribute.IndexType);
            }
            var response = await _client.BulkAsync(bulk);
            // TODO - handle logging on failure, throw exception, ...?
            if (response.Errors)
            {
                foreach (var item in response.ItemsWithErrors)
                {
                    Console.WriteLine(item.Error.ToString());
                }
            }
        }
    }
}
