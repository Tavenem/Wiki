using Fido2NetLib;
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
using NeverFoundry.Wiki.Mvc;
using NeverFoundry.Wiki.Mvc.Authorization;
using NeverFoundry.Wiki.Sample.Data;
using NeverFoundry.Wiki.Sample.Logging;
using NeverFoundry.Wiki.Sample.Providers;
using NeverFoundry.Wiki.Sample.Services;
using System.Reflection;

namespace NeverFoundry.Wiki.MVCSample
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var connectionString = Configuration.GetConnectionString("Auth");

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(connectionString, a => a.MigrationsAssembly(migrationsAssembly)));
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
                options.AddPolicy(Constants.Claim_SiteAdmin, Policies.IsSiteAdminPolicy());
                options.AddPolicy(Constants.Claim_WikiAdmin, Policies.IsWikiAdminPolicy());
            });

            services.Configure<Fido2Configuration>(Configuration.GetSection("fido2"));
            services.AddScoped<Fido2Storage>();
            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddSingleton<MartenLogger>();
            services.AddSingleton(provider => Seed.GetDocumentStore(
                Configuration.GetConnectionString("Wiki"),
                provider.GetRequiredService<MartenLogger>()));

            services.AddElasticsearch(Configuration);

            services.AddControllersWithViews()
                .AddNewtonsoftJson();
            services.AddRazorPages();

            services.AddSignalR();

            services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration")?.Get<EmailConfiguration>() ?? new EmailConfiguration());
            services.AddTransient<IEmailService, EmailService>();

            WikiConfig.ServerUrl = "https://localhost:5001/";
            Web.WikiWebConfig.MaxFileSize = 100000000; // 100 MB
            services.AddWiki(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return new WikiOptions
                {
                    CompactLayoutPath = "/Pages/Shared/_Layout.cshtml",
                    MainLayoutPath = "/Pages/Shared/_MainLayout.cshtml",
                    LoginPath = "/Account/Login",
                    SearchClient = new ElasticSearchClient(
                        provider.GetRequiredService<IElasticClient>(),
                        loggerFactory.CreateLogger<ElasticSearchClient>(),
                        provider.GetRequiredService<UserManager<WikiUser>>()),
                };
            });
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
