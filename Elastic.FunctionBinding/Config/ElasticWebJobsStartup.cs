using Elastic.FunctionBinding;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(ElasticWebJobsStartup))]

namespace Elastic.FunctionBinding
{
    public class ElasticWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddElastic();
        }
    }
}
