using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;

namespace Elastic.FunctionBinding
{
    public static class ElasticWebJobsBuilderExtensions
    {
        public static IWebJobsBuilder AddElastic(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.AddExtension<ElasticConfigProvider>()
                    .ConfigureOptions<ElasticOptions>(ApplyConfiguration);

            return builder;
        }

        private static void ApplyConfiguration(IConfigurationSection config, ElasticOptions options)
        {
            if (config == null)
            {
                return;
            }

            config.Bind(options);
        }
    }
}
