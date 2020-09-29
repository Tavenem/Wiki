using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Nest.JsonNetSerializer;
using System;

namespace NeverFoundry.Wiki.Samples.Complete.Services
{
    public static class ElasticsearchExtension
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration.GetValue<string>("Elasticsearch:Url");
            var defaultIndex = configuration.GetValue<string>("Elasticsearch:Index");

            var settings = new ConnectionSettings(
                    new SingleNodeConnectionPool(new Uri(url)),
                    sourceSerializer: JsonNetSerializer.Default)
                .DefaultIndex(defaultIndex)
                .DefaultFieldNameInferrer(x => x)
                .DefaultMappingFor<WikiLink>(m => m)
                .DefaultMappingFor<Article>(m =>
                    m.IndexName(defaultIndex)
                    .Ignore(i => i.Timestamp))
                .DefaultMappingFor<Category>(m =>
                    m.IndexName(defaultIndex)
                    .Ignore(i => i.Timestamp))
                .DefaultMappingFor<WikiFile>(m =>
                    m.IndexName(defaultIndex)
                    .Ignore(i => i.Timestamp));

            var client = new ElasticClient(settings);

            var result = client.Indices.Exists(defaultIndex);
            if (!result.Exists)
            {
                var response = client.Indices.Create(defaultIndex, i =>
                    i.Map<Article>(m =>
                        m.AutoMap<Article>()
                        .Properties(p =>
                            p.Nested<WikiLink>(n =>
                                n.Name(nn => nn.WikiLinks)
                                .AutoMap())
                            .Nested<Transclusion>(n =>
                                n.Name(nn => nn.Transclusions)
                                .AutoMap())
                            .Keyword(k => k.Name(n => n.AllowedEditors))
                            .Keyword(k => k.Name(n => n.AllowedViewers))
                            .Keyword(k => k.Name(n => n.Categories)))
                        .AutoMap<Category>()
                        .AutoMap<WikiFile>()));
                if (!response.ApiCall.Success)
                {
                    throw response.ApiCall.OriginalException;
                }
            }

            services.AddSingleton<IElasticClient>(client);
        }
    }
}
