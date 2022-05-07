using Hamburger.Helpers.Extensions;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Hamburger.Helpers
{
    public static class AppSettings
    {
        private static IConfigurationRoot _configuration;

        public static IConfigurationRoot Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    var appsettingsFileName = EnvironmentVariable.AspNetCoreEnvironment.IsNullOrWhiteSpace() ?
                        "appsettings.json" :
                        $"appsettings.{EnvironmentVariable.AspNetCoreEnvironment}.json";

                    _configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile(appsettingsFileName)
                        .Build();
                }
                return _configuration;
            }
        }

        public static class Database
        {
            public static string ConnectionString => Configuration["Database:ConnectionString"].IsNullOrEmpty() ? EnvironmentVariable.DatabaseConnectionString : Configuration["Database:ConnectionString"];
        }

        public static class LoggingService
        {
            public static string BaseUrl => Configuration["LoggingService:BaseUrl"];
            public static string DefaultCollectionName => Configuration["LoggingService:DefaultCollectionName"];
            public static string ApiKey => Configuration["LoggingService:ApiKey"].IsNullOrEmpty() ? EnvironmentVariable.LoggingServiceApiKey : Configuration["LoggingService:ApiKey"];
        }

        public static class Jwt
        {
            public static string Issuer => Configuration["Jwt:Issuer"];
            public static string Audience => Configuration["Jwt:Audience"];
            public static int Expiration => int.Parse(Configuration["Jwt:Expiration"]);
            public static int RefreshTokenExpiration => int.Parse(Configuration["Jwt:RefreshTokenExpiration"]);
            public static string Secret => Configuration["Jwt:Secret"].IsNullOrEmpty() ? EnvironmentVariable.JwtSecret : Configuration["Jwt:Secret"];
        }

        public static class FileStorage
        {
            public static string BaseUrl => Configuration["FileStorage:BaseUrl"];
            public static string ApiKey => Configuration["FileStorage:ApiKey"].IsNullOrEmpty() ? EnvironmentVariable.FileStorageApiKey : Configuration["FileStorage:ApiKey"];
        }
    }
}
