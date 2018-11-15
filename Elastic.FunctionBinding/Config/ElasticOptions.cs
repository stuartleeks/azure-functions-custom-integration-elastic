using Microsoft.Azure.WebJobs.Description;

namespace Elastic.FunctionBinding
{
    public class ElasticOptions
    {
        [AppSetting(Default = "ElasticUrl")]
        public string ElasticUrl { get; set; }

        [AppSetting(Default = "ElasticUsername")]
        public string UserName { get; set; }

        [AppSetting(Default = "ElasticPassword")]
        public string Password { get; set; }
    }
}
