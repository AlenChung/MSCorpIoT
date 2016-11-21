using System.Configuration;

namespace MSCorp.FirstResponse.PowerBIDataLoader
{
    public static class Configuration
    {
        public static string ClientId => ConfigurationManager.AppSettings["Auth.ClientId"];
        public static string RedirectUrl => ConfigurationManager.AppSettings["Auth.RedirectUrl"];
        public static string ResourceUri => ConfigurationManager.AppSettings["Auth.ResourceUri"];
        public static string RestApiUrl => ConfigurationManager.AppSettings["PowerBI.RestApiUrl"];
        public static int RefreshInSeconds => int.Parse(ConfigurationManager.AppSettings["Data.RefreshInSeconds"]);
    }
}