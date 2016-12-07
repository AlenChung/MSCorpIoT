using System;
using System.IO;

namespace MSCorp.FirstResponse.AzureSearch.Commands.Helper
{
    public static class JsonFileHelper
    {
        public static string ReadJsonFileToString(string filePath)
        {
            var file = File.ReadAllText(filePath);
            return file.Replace(Environment.NewLine, string.Empty);
        }
    }
}
