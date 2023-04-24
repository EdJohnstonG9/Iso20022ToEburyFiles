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
            var configuration = new ConfigurationBuilder()
                .SetBasePath(appLocation)
                .AddUserSecrets<ApplyFixture>()
                .Build();

            ApplyConfiguration applyConfig = new ApplyConfiguration();

            //var applyConfig = new ApplyConfiguration();
            applyConfig.BaseUrl = configuration.GetSection("ApplyConfiguration:BaseUrl").Value;
            applyConfig.Credentials.UserName = configuration.GetSection("ApplyConfiguration:Credentials:UserName").Value;
            applyConfig.Credentials.Password = configuration.GetSection("ApplyConfiguration:Credentials:Password").Value;

            _applyConfig= applyConfig;
        }
        public void Dispose()
        {
            _applyConfig = null;
        }
    }
}
