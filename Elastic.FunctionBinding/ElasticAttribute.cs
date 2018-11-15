using System;
using Microsoft.Azure.WebJobs.Description;

namespace Elastic.FunctionBinding
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public sealed class ElasticAttribute : Attribute
    {
        // TODO - look at config more. Can we group under a top level tag? How do we handle multiple 

        [AppSetting(Default = "ElasticUrl")]
        public string ElasticUrl { get; set; }

        [AppSetting(Default = "ElasticUsername")]
        public string UserName { get; set; }

        [AppSetting(Default = "ElasticPassword")]
        public string Password { get; set; }

        /// <summary>
        /// The index name to submit the document to
        /// </summary>
        public string Index { get; set; }
        /// <summary>
        /// The type name to use when indexing the document
        /// </summary>
        public string IndexType { get; set; } // "Type" clashes with function.json "type" property so using IndexType
    }
}
