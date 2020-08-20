using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace NeverFoundry.Wiki.MvcSample.Services
{
    public static class EmailTemplates
    {
        private static IConfiguration? _Configuration;
        private static IWebHostEnvironment? _Environment;

        public static void Initialize(IConfiguration configuration, IWebHostEnvironment env)
        {
            _Configuration = configuration;
            _Environment = env;
        }

        public static EmailMessage BuildChangeAddressEmail(string callbackUrl)
        {
            var changeEmailTemplate = ReadFile("/Services/Email/Templates/ChangeEmail.template");
            return new EmailMessage
            {
                Subject = "Email Address Change",
                Body = changeEmailTemplate
                    .Replace("{siteUrl}", _Configuration?.GetSection("Site")?.GetValue<string>("SiteURL") ?? "https://localhost")
                    .Replace("{callbackUrl}", callbackUrl),
            };
        }

        public static EmailMessage BuildForgotPasswordEmail(string callbackUrl)
        {
            var forgotPasswordTemplate = ReadFile("/Services/Email/Templates/ForgotPassword.template");
            return new EmailMessage
            {
                Subject = "Password Reset",
                Body = forgotPasswordTemplate
                    .Replace("{siteUrl}", _Configuration?.GetSection("Site")?.GetValue<string>("SiteURL") ?? "https://localhost")
                    .Replace("{callbackUrl}", callbackUrl),
            };
        }

        public static EmailMessage BuildRegistrationConfirmationEmail(string callbackUrl)
        {
            var registrationConfirmationTemplate = ReadFile("/Services/Email/Templates/RegistrationConfirmation.template");
            return new EmailMessage
            {
                Subject = $"Welcome to {_Configuration?.GetSection("Site")?.GetValue<string>("SiteName") ?? "our website"}",
                Body = registrationConfirmationTemplate
                    .Replace("{siteUrl}", _Configuration?.GetSection("Site")?.GetValue<string>("SiteURL") ?? "https://localhost")
                    .Replace("{callbackUrl}", callbackUrl)
            };
        }

        private static string ReadFile(string path)
        {
            if (_Environment is null)
            {
                throw new InvalidOperationException($"{nameof(EmailTemplates)} has not been initialized");
            }

            var fileInfo = _Environment.ContentRootFileProvider.GetFileInfo(path);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"Email template at \"{path}\" was not found");
            }

            using var fs = fileInfo.CreateReadStream();
            using var sr = new StreamReader(fs);
            return sr.ReadToEnd();
        }
    }
}
