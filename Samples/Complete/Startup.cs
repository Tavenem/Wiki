using Fido2NetLib;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;
using NeverFoundry.DataStorage;
using NeverFoundry.DataStorage.Marten;
using NeverFoundry.Wiki.Mvc;
using NeverFoundry.Wiki.Samples.Complete.Data;
using NeverFoundry.Wiki.Samples.Complete.Logging;
using NeverFoundry.Wiki.Samples.Complete.Providers;
using NeverFoundry.Wiki.Samples.Complete.Services;
using NeverFoundry.Wiki.Web;
using System.Reflection;
using System.Security.Claims;

namespace NeverFoundry.Wiki.Samples.Complete
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("Auth"),
                    a => a.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name)));

            services.Configure<IdentityOptions>(options =>
                options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);

            services.AddIdentity<WikiUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<Fido2TokenProvider>("FIDO2");

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Error/401";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(WikiClaims.Claim_SiteAdmin, WikiPolicies.IsSiteAdminPolicy());
                options.AddPolicy(WikiClaims.Claim_WikiAdmin, WikiPolicies.IsWikiAdminPolicy());
            });

            services.Configure<Fido2Configuration>(Configuration.GetSection("fido2"));
            services.AddScoped<Fido2Storage>();
            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddSingleton<MartenLogger>();
            services.AddSingleton<IDataStore>(provider => new MartenDataStore(DocumentStore.For(config =>
            {
                config.UseDefaultSerialization(collectionStorage: CollectionStorage.AsArray, nonPublicMembersStorage: NonPublicMembersStorage.NonPublicSetters);
                config.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                config.Connection(Configuration.GetConnectionString("Wiki"));
                config.Logger(provider.GetRequiredService<MartenLogger>());
                config.Schema.For<Article>()
                    .AddSubClass<Category>()
                    .AddSubClass<WikiFile>();
                config.Schema.For<Revision>();
                config.Schema.For<MissingPage>();
                config.Schema.For<Message>();
                config.Schema.For<WikiGroup>();
            })));

            services.AddElasticsearch(Configuration);

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSignalR(options => options.EnableDetailedErrors = true);

            services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration")?.Get<EmailConfiguration>() ?? new EmailConfiguration());
            services.AddTransient<IEmailService, EmailService>();

            WikiWebConfig.ContactPageTitle = null;
            WikiWebConfig.ContentsPageTitle = null;
            WikiWebConfig.CopyrightPageTitle = null;
            WikiWebConfig.PolicyPageTitle = null;
            WikiWebConfig.MaxFileSize = 100000000; // 100 MB
            services.AddWiki(
                typeof(WikiUserManager<WikiUser>),
                typeof(WikiGroupManager<WikiUser>),
                provider =>
                {
                    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                    return new WikiOptions
                    {
                        DataStore = provider.GetRequiredService<IDataStore>(),
                        CompactLayoutPath = "/Pages/Shared/_Layout.cshtml",
                        LoginPath = "/Account/Login",
                        MainLayoutPath = "/Pages/Shared/_MainLayout.cshtml",
                        TenorAPIKey = "ZB1P1TN5PVFQ",
                    };
                },
                searchClientType: typeof(ElasticSearchClient));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            EmailTemplates.Initialize(Configuration, env);

            Seed.InitializeDatabasesAsync(app).GetAwaiter().GetResult();

            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapWiki();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
