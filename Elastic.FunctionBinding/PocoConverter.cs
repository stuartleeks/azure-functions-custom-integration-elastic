using Elastic.FunctionBinding;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Elastic.FunctionBinding
{
    // PocoOpenType determines which types PocoConverter is called for 
    // based on the registration in ElasticConfigProvider

    public class PocoOpenType : OpenType
    {
        public override bool IsMatch(Type type, OpenTypeMatchContext context)
        {
            return type != typeof(byte[])
                   && type != typeof(IElasticMessage);
        }
    }
    public class PocoConverter<T> : IConverter<T, IElasticMessage>
        where T : class
    {
        public IElasticMessage Convert(T input)
        {
            switch (input)
            {
                case string stringInput:
                    // check for object or array
                    var value = JsonConvert.DeserializeObject(stringInput);
                    switch (value)
                    {
                        case JObject jObject:
                            return new ElasticMessage<JObject>(jObject);
                        case JArray jArray:
                            return new ElasticMessage<JArray>(jArray);
                        default:
                            throw new NotSupportedException($"PocoConverter doesn't support converting strings with {value.GetType().FullName}");
                    }
                default:
                    return new ElasticMessage<T>(input);
            }
        }

    }
}
