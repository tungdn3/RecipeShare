using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Search.API.Models;
using System.Net;

namespace Search.API.Services;

public class SearchService
{
    private readonly ILogger<SearchService> _logger;
    private readonly ElasticsearchClient _client;

    public SearchService(ILogger<SearchService> logger, ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<IReadOnlyCollection<Recipe>> Search(RecipeSearchRequest request)
    {
        var requestDescriptor = new SearchRequestDescriptor<Recipe>()
        .From((request.Page - 1) * request.PageSize)
        .Size(request.PageSize)
        .Query(q => q
            .Bool(b =>
            {
                if (!string.IsNullOrEmpty(request.Query))
                {
                    b.Must(m =>
                    {
                        m.MultiMatch(m =>
                        {
                            m.Query(request.Query);
                            m.Fields(new Field[]
                            {
                                new("title"),
                                new("ingredients"),
                                new("description"),
                                new("instructions"),
                            });
                        });
                    });
                }

                if (request.CategoryId != null)
                {
                    b.Filter(f =>
                    {
                        f.Term(t =>
                        {
                            t.Field(r => r.CategoryId);
                            t.Value(request.CategoryId.Value);
                        });
                    });
                }
            })
        );

        SearchResponse<Recipe> response = await _client.SearchAsync(requestDescriptor);

        if (!response.IsValidResponse)
        {
            _logger.LogError("Failed to search recipes with request '{RecipeSearchRequest}'. Debug info: '{DebugInfo}'",
                JsonConvert.SerializeObject(request), response.DebugInformation);
            
            return new List<Recipe>();
        }

        return response.Documents;
    }

    public async Task<List<string>> Complete(string query, int top = 10)
    {
        var request = new SearchRequestDescriptor<Recipe>()
            .From(0)
            .Size(top)
            .Query(q => q
                .Match(m =>
                {
                    m.Field(x => x.Title);
                    m.Query(query);
                })
            );

        SearchResponse<Recipe> response = await _client.SearchAsync(request);

        if (!response.IsValidResponse)
        {
            _logger.LogError("Failed to generate completions for Query '{Query}'. Debug info: '{DebugInfo}'.", query, response.DebugInformation);
            return new List<string>();
        }

        List<string> titles = response.Documents.Select(x => x.Title).ToList();
        return titles;
    }
}
