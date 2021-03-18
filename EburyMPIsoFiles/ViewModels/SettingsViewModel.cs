using EburyMPIsoFiles.Models;
using EburyMPIsoFiles.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace EburyMPIsoFiles.ViewModels
{
    public class SettingsViewModel : BindableBase, INavigationAware
    {
        private readonly AppConfig _appConfig;
        private readonly ISystemService _systemService;
        private readonly IApplicationInfoService _applicationInfoService;
        private readonly IObjectToPropertiesService _objectToPropertiesService;
        private UserSettings _userSettings;

        private string _versionDescription;
        public string VersionDescription
        {
            get { return _versionDescription; }
            set { SetProperty(ref _versionDescription, value); }
        }


        public SettingsViewModel(AppConfig appConfig, ISystemService systemService, IApplicationInfoService applicationInfoService, IObjectToPropertiesService objectToPropertiesService)
        {
            _appConfig = appConfig;
            _systemService = systemService;
            _applicationInfoService = applicationInfoService;
            _objectToPropertiesService = objectToPropertiesService;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            VersionDescription = $"IsoToEburyFile - {_applicationInfoService.GetVersion()}";
            _userSettings = _objectToPropertiesService.GetCurrent<UserSettings>();
            if (_userSettings == null)
                _userSettings = new UserSettings();
            XmlFilePath = _userSettings.XmlFilePath;
            if (string.IsNullOrEmpty(XmlFilePath))
            {
                XmlFilePath = @"G:\Shared drives\MP - High Wycombe - Data";
                ExecuteSaveUserSettingsCommand(null);
            }
            OutputFilePath = _userSettings.SaveFilePath;
            if (string.IsNullOrEmpty(OutputFilePath))
            {
                OutputFilePath = XmlFilePath;
                ExecuteSaveUserSettingsCommand(null);
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        private ICommand _webLinkCommand;
        public ICommand WebLinkCommand => _webLinkCommand ?? (_webLinkCommand = new DelegateCommand(OnWebLink));
        private void OnWebLink()
            => _systemService.OpenInWebBrowser(_appConfig.PrivacyStatement);

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;

        #region ApplicationSettings

        private string _OutputFilePath;
        public string OutputFilePath
        {
            get { return _OutputFilePath; }
            set { SetProperty(ref _OutputFilePath, value); }
        }

        private string _xmlFilePath;
        public string XmlFilePath
        {
            get { return _xmlFilePath; }
            set { SetProperty(ref _xmlFilePath, value); }
        }
        private DelegateCommand<EventArgs> _saveUserSettingsCommand;
        public DelegateCommand<EventArgs> SaveUserSettingsCommand =>
            _saveUserSettingsCommand ?? (_saveUserSettingsCommand = new DelegateCommand<EventArgs>(ExecuteSaveUserSettingsCommand)
            .ObservesProperty(() => XmlFilePath)
            .ObservesProperty(() => OutputFilePath));

        void ExecuteSaveUserSettingsCommand(EventArgs args)
        {
            _userSettings.XmlFilePath = XmlFilePath.Trim();
            _userSettings.SaveFilePath = OutputFilePath?.Trim();
            _objectToPropertiesService.SaveCurrent(_userSettings);
        }

        #endregion
    }
}
