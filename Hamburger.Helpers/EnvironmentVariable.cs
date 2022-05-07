using System;

namespace Hamburger.Helpers
{
    public static class EnvironmentVariable
    {
        public static string AspNetCoreEnvironment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        public static string DatabaseConnectionString => Environment.GetEnvironmentVariable("DatabaseConnectionString");
        public static string JwtSecret => Environment.GetEnvironmentVariable("JwtSecret");
        public static string FileStorageApiKey => Environment.GetEnvironmentVariable("FileStorageApiKey");
        public static string LoggingServiceApiKey => Environment.GetEnvironmentVariable("LoggingServiceApiKey");
    }
}
