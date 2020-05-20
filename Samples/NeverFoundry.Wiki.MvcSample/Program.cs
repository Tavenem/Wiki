using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NeverFoundry.Wiki.Sample.Logging;
using NeverFoundry.Wiki.Web;
using Npgsql;
using Serilog;
using System;
using System.Diagnostics;

namespace NeverFoundry.Wiki.MVCSample
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Serilog.Debugging.SelfLog.Enable(x => Debug.WriteLine(x));

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            ConfigureDatabases(configuration);

            var logLevel = (Serilog.Events.LogEventLevel)(configuration.GetSection("Serilog")?.GetValue<int>("LogEventLevel") ?? 3);
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.PostgreSql(
                    configuration.GetConnectionString("Logging"),
                    LoggingConfig.TableName,
                    LoggingConfig.ColumnOptions,
                    restrictedToMinimumLevel: logLevel,
                    needAutoCreateTable: true)
                .CreateLogger();

            WikiWebConfig.ContactPageTitle = null;
            WikiWebConfig.ContentsPageTitle = null;
            WikiWebConfig.CopyrightPageTitle = null;
            WikiWebConfig.PolicyPageTitle = null;

            try
            {
                Log.Information("Starting NeverFoundry.Wiki.MVCSample host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "NeverFoundry.Wiki.MVCSample host terminated unexpectedly");
                return 1;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStaticWebAssets();
                    builder.UseStartup<Startup>();
                });

        private static void ConfigureDatabases(IConfigurationRoot configuration)
        {
            var connectionString = configuration.GetConnectionString("Default");

            var auth_db = configuration.GetValue<string>("Database:Names:Auth");
            var auth_un = configuration.GetValue<string>("Database:Logins:Auth");
            var auth_pw = configuration.GetValue<string>("Database:Passwords:Auth");
            var log_db = configuration.GetValue<string>("Database:Names:Logs");
            var logger_un = configuration.GetValue<string>("Database:Logins:Logger");
            var logger_pw = configuration.GetValue<string>("Database:Passwords:Logger");
            var wiki_db = configuration.GetValue<string>("Database:Names:Wiki");
            var wiki_un = configuration.GetValue<string>("Database:Logins:Wiki");
            var wiki_pw = configuration.GetValue<string>("Database:Passwords:Wiki");

            using var con = new NpgsqlConnection(connectionString);

            var authusr_ifexists_cmd = new NpgsqlCommand($"SELECT 1 FROM pg_roles WHERE rolname='{auth_un}';", con);
            var authusr_create_cmd = new NpgsqlCommand($"CREATE ROLE {auth_un} LOGIN PASSWORD '{auth_pw}';", con);
            var logusr_ifexists_cmd = new NpgsqlCommand($"SELECT 1 FROM pg_roles WHERE rolname='{logger_un}';", con);
            var logusr_create_cmd = new NpgsqlCommand($"CREATE ROLE {logger_un} LOGIN PASSWORD '{logger_pw}';", con);
            var wikiusr_ifexists_cmd = new NpgsqlCommand($"SELECT 1 FROM pg_roles WHERE rolname='{wiki_un}';", con);
            var wikiusr_create_cmd = new NpgsqlCommand($"CREATE ROLE {wiki_un} LOGIN PASSWORD '{wiki_pw}';", con);
            var auth_db_ifexists_cmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname='{auth_db}';", con);
            var auth_db_cmd = new NpgsqlCommand(@$"
                    CREATE DATABASE {auth_db}
                    WITH OWNER = {auth_un}
                    ENCODING = 'UTF8'
                    CONNECTION LIMIT = -1;
                    ", con);
            var log_db_ifexists_cmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname='{log_db}';", con);
            var log_db_cmd = new NpgsqlCommand(@$"
                    CREATE DATABASE {log_db}
                    WITH OWNER = {logger_un}
                    ENCODING = 'UTF8'
                    CONNECTION LIMIT = -1;
                    ", con);
            var wiki_db_ifexists_cmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname='{wiki_db}';", con);
            var wiki_db_cmd = new NpgsqlCommand(@$"
                    CREATE DATABASE {wiki_db}
                    WITH OWNER = {wiki_un}
                    ENCODING = 'UTF8'
                    CONNECTION LIMIT = -1;
                    ", con);

            con.Open();

            var result = authusr_ifexists_cmd.ExecuteScalar();
            if (result is null)
            {
                authusr_create_cmd.ExecuteNonQuery();
            }
            result = logusr_ifexists_cmd.ExecuteScalar();
            if (result is null)
            {
                logusr_create_cmd.ExecuteNonQuery();
            }
            result = wikiusr_ifexists_cmd.ExecuteScalar();
            if (result is null)
            {
                wikiusr_create_cmd.ExecuteNonQuery();
            }
            result = auth_db_ifexists_cmd.ExecuteScalar();
            if (result is null)
            {
                auth_db_cmd.ExecuteNonQuery();
            }
            result = log_db_ifexists_cmd.ExecuteScalar();
            if (result is null)
            {
                log_db_cmd.ExecuteNonQuery();
            }
            result = wiki_db_ifexists_cmd.ExecuteScalar();
            if (result is null)
            {
                wiki_db_cmd.ExecuteNonQuery();
            }
        }
    }
}
