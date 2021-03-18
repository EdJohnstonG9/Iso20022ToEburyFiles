using Microsoft.Extensions.Configuration;

namespace EburyMPIsoFiles.Models
{
    public class AppConfig
    {
        public string ConfigurationsFolder { get; set; }
        public string AppPropertiesFileName { get; set; }
        public string PrivacyStatement { get; set; }
        public string TermsOfUseStatement { get; set; }
        public AppConfig(IConfiguration configuration)
        {
            ConfigurationsFolder = configuration.GetSection($"{nameof(AppConfig)}:{nameof(ConfigurationsFolder)}").Value;
            AppPropertiesFileName = configuration.GetSection($"{nameof(AppConfig)}:{nameof(AppPropertiesFileName)}").Value;
            PrivacyStatement = configuration.GetSection($"{nameof(AppConfig)}:{nameof(PrivacyStatement)}").Value;
            TermsOfUseStatement = configuration.GetSection($"{nameof(AppConfig)}:{nameof(TermsOfUseStatement)}").Value;
        }
    }
}
