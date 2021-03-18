using System;
using System.Diagnostics;
using System.Reflection;

namespace EburyMPIsoFiles.Services
{
    public class ApplicationInfoService : IApplicationInfoService
    {
        public ApplicationInfoService()
        {
        }

        public Version GetVersion()
        {
            Version version;
            try
            {
                version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception ex)
            {
                // Set the app version in EburyMassFilesWPF > Properties > Package > PackageVersion
                //string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                //version = FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
                version = Assembly.GetExecutingAssembly().GetName().Version;
            }

            return version;
        }
    }
}
