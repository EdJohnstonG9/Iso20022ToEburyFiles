using EburyMPIsoFilesLibrary.Models.ApplyFinancials;

using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using Xunit;

using static System.Net.Mime.MediaTypeNames;

namespace EburyMPIsoFilesLibraryTests
{
    public class ApplyFixture : IDisposable
    {
        public ApplyConfiguration _applyConfig;
        public ApplyFixture()
        {
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(appLocation)
                .AddUserSecrets<ApplyFixture>().Build();

            ApplyConfiguration applyConfig = new ApplyConfiguration();

            //var applyConfig = new ApplyConfiguration();
            //applyConfig.BaseUrl = configuration.GetSection("ApplyConfiguration_BaseUrl").Value;
            //applyConfig.Credentials.UserName = configuration.GetSection("ApplyConfiguration_Credentials_UserName").Value;
            //applyConfig.Credentials.Password = configuration.GetSection("ApplyConfiguration_Credentials_Password").Value;
            applyConfig.BaseUrl = configuration.GetValue<string>("ApplyConfiguration_BaseUrl");
            applyConfig.Credentials.UserName = configuration.GetValue<string>("ApplyConfiguration_Credentials_UserName");
            applyConfig.Credentials.Password = configuration.GetValue<string>("ApplyConfiguration_Credentials_Password");

            Console.WriteLine($"applyConfig.BaseUrl: {applyConfig.BaseUrl}");
            Console.WriteLine($"applyConfig.Credentials.UserName: {applyConfig.Credentials.UserName}");
            Console.WriteLine($"applyConfig.Credentials.Password: {applyConfig.Credentials.Password}");
            _applyConfig = applyConfig;
        }
        public void Dispose()
        {
            _applyConfig = null;
        }
    }
}
