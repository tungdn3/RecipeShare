using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;

namespace Search.API.Repositories;

public static class ElasticSearchInitializer
{
    public static void EnsureIndexCreated(ElasticsearchClient client, string indexName)
    {
        var response = client.Indices.ExistsAsync(indexName).Result;
        if (response.ApiCallDetails.HttpStatusCode != 200 && response.ApiCallDetails.HttpStatusCode != 404)
        {
            throw new InvalidOperationException($"Failed to check if index '{indexName}' exists. Debug info: {response.DebugInformation}");
        }

        if (!response.Exists)
        {
            client.Indices.CreateAsync(indexName).Wait();

            client.Indices.PutMappingAsync(new PutMappingRequest(indexName)
            {
                Properties = new Properties(new Dictionary<PropertyName, IProperty>
                {
                    { "userId", new KeywordProperty{ Index = false } },
                    { "imageFileName", new KeywordProperty{ Index = false } },
                    { "description", new TextProperty{ } },
                    { "ingredients", new TextProperty{ } },
                    { "instructions", new TextProperty{ } },
                }),
            });
        }
    }
}
