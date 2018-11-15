using Elastic.FunctionBinding;
using Microsoft.Azure.WebJobs.Hosting;
using Nest;
using Newtonsoft.Json.Linq;

namespace Elastic.FunctionBinding
{
    public interface IElasticMessage // thin wrapper around the object to index so that we can register a mapper
    {
        void AddToBulkDescriptor(BulkDescriptor bulkDescriptor, string index, string indexType);
        object Value { get; }
    }
    public class ElasticMessage<T> : IElasticMessage
        where T : class
    {

        public ElasticMessage(T value)
        {
            Value = value;
        }
        public T Value { get; }

        object IElasticMessage.Value { get => Value; }

        public void AddToBulkDescriptor(BulkDescriptor bulkDescriptor, string index, string indexType)
        {
            void IndexItem<TItem>(TItem item)
                where TItem : class
            {
                // TODO only call Type if indexType is set
                bulkDescriptor.Index<TItem>(indexDescriptor =>
                {
                    indexDescriptor.Index(index);
                    if (!string.IsNullOrEmpty(indexType))
                    {
                        // Allow index type inference by TypeName (as per .NET behaviour in NEST)
                        indexDescriptor.Type(indexType);
                    }
                    if (item is JObject jObject)
                    {
                        var idProperty = jObject.Property("_id");
                        if (idProperty != null)
                        {
                            // Set the Id for the operation
                            indexDescriptor.Id(idProperty.Value.ToString()); // TODO - should handle NEST.Id supported types, not just string!
                            idProperty.Remove(); // remove from the object before submitting to avoid NEST errors
                        }
                    }
                    indexDescriptor.Document(item);
                    return indexDescriptor;
                });
            }

            // Handle JArray as a special case to allow multiple records to be written in a function in JavaScript
            if (Value is JArray jArray)
            {
                foreach (var obj in jArray)
                {
                    IndexItem(obj);
                }
            }
            else
            {
                IndexItem(Value);
            }
        }
    }

}
