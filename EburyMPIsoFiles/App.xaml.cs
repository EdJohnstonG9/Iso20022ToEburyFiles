using EburyMPIsoFiles.Constants;
using EburyMPIsoFiles.Core.Contracts.Services;
using EburyMPIsoFiles.Core.Services;
using EburyMPIsoFiles.Models;
using EburyMPIsoFiles.Services;
using EburyMPIsoFiles.ViewModels;
using EburyMPIsoFiles.Views;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EburyMPIsoFiles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private string[] _startUpArgs;

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }
        protected override async void OnInitialized()
        {
            var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
            persistAndRestoreService.RestoreData();

            base.OnInitialized();
            await Task.CompletedTask;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _startUpArgs = e.Args;
            //ShutdownMode = ShutdownMode.OnMainWindowClose; 
            base.OnStartup(e);
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            // Core Services
            containerRegistry.Register<IFileService, FileService>();

            // App Services
            containerRegistry.Register<IApplicationInfoService, ApplicationInfoService>();
            containerRegistry.Register<ISystemService, SystemService>();
            containerRegistry.Register<IPersistAndRestoreService, PersistAndRestoreService>();

            //Views
            containerRegistry.RegisterForNavigation<IsoToMassPayments, IsoToMassPaymentsViewModel>(PageKeys.IsoToMassPayments);
            containerRegistry.RegisterForNavigation<Settings, SettingsViewModel>(PageKeys.Settings);

            // Configuration
            var configuration = BuildConfiguration();
            var appConfig = new AppConfig(configuration);

            // Register configurations to IoC
            containerRegistry.RegisterInstance<IConfiguration>(configuration);
            containerRegistry.RegisterInstance<AppConfig>(appConfig);
            containerRegistry.Register<IObjectToPropertiesService, ObjectToPropertiesService>();

        }

        private IConfiguration BuildConfiguration()
        {
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var output = new ConfigurationBuilder()
                .SetBasePath(appLocation)
                .AddJsonFile("appsettings.json")
                .Build();
            return output;
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
            persistAndRestoreService.PersistData();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, nameof(sender), MessageBoxButton.OK);
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
        }
    }
}
