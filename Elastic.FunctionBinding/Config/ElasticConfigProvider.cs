using System;
using Elastic.FunctionBinding;
using Elasticsearch.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json.Linq;

namespace Elastic.FunctionBinding
{
    [Extension("Elastic")]
    public class ElasticConfigProvider : IExtensionConfigProvider
    {
        private readonly ElasticOptions _options;
        private readonly INameResolver _nameResolver;
        private readonly ILogger _logger;

        public ElasticConfigProvider(
            IOptions<ElasticOptions> options,
            INameResolver nameResolver,
            ILoggerFactory loggerFactory
            )
        {
            _options = options.Value;
            _nameResolver = nameResolver;
            _logger = loggerFactory.CreateLogger("Elastic");
        }
        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // TODO - use nameresolver to resolve Elastic URL etc?

            context.AddOpenConverter<PocoOpenType, IElasticMessage>(typeof(PocoConverter<>));

            var elasticAttributeBinding = context.AddBindingRule<ElasticAttribute>();
            elasticAttributeBinding.AddValidator(ElasticAttributeValidator);
            elasticAttributeBinding.BindToCollector<IElasticMessage>(CreateElasticAsyncCollector);
        }

        private void ElasticAttributeValidator(ElasticAttribute attribute, Type type)
        {
            // TODO validate connection details exist or throw exception
            // TODO validate that Index is set (also IndexType?)
        }



        private ElasticAsyncCollector CreateElasticAsyncCollector(ElasticAttribute attribute)
        {
            var elasticUrl = string.IsNullOrEmpty(attribute.ElasticUrl) ? _options.ElasticUrl : attribute.ElasticUrl;
            var userName = string.IsNullOrEmpty(attribute.UserName) ? _options.UserName : attribute.UserName;
            var password = string.IsNullOrEmpty(attribute.Password) ? _options.Password : attribute.Password;

            var connectionPool = new SingleNodeConnectionPool(new Uri(elasticUrl));
            var settings = new ConnectionSettings(connectionPool, sourceSerializer: JsonNetSerializer.Default)
             .BasicAuthentication(userName, password); // TODO - look at how to support different auth types

            var client = new ElasticClient(settings); // TODO - should this be cached?

            return new ElasticAsyncCollector(client, attribute);
        }
    }
}
