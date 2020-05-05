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
                .DefaultIndex(defaultIndex);
            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);
        }
    }
}
