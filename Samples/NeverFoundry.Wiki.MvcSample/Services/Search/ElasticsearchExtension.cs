using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;

namespace NeverFoundry.Wiki.Sample.Services
{
    public static class ElasticsearchExtension
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration.GetValue<string>("Elasticsearch:Url");
            var defaultIndex = configuration.GetValue<string>("Elasticsearch:Index");

            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex)
                .DefaultFieldNameInferrer(x => x)
                .DefaultMappingFor<WikiLink>(m =>
                    m.Ignore(i => i.FullTitle))
                .DefaultMappingFor<Article>(m =>
                    m.Ignore(i => i.FullTitle)
                    .Ignore(i => i.Timestamp))
                .DefaultMappingFor<Category>(m =>
                    m.Ignore(i => i.FullTitle))
                .DefaultMappingFor<WikiFile>(m =>
                    m.Ignore(i => i.FullTitle));

            var client = new ElasticClient(settings);

            var response = client.Indices.Create(defaultIndex, i =>
                i.Map<Article>(m =>
                    m.AutoMap<Article>()
                    .Properties(p =>
                        p.Nested<WikiLink>(n =>
                            n.Name(nn => nn.WikiLinks)
                            .AutoMap()))
                    .AutoMap<Category>()
                    .AutoMap<WikiFile>()));
            if (!response.ApiCall.Success)
            {
                throw response.ApiCall.OriginalException;
            }

            services.AddSingleton<IElasticClient>(client);
        }
    }
}
