using EburyMPIsoFiles.Core.Models;
using EburyMPIsoFiles.Models;
using EburyMPIsoFiles.Services;

using EburyMPIsoFilesLibrary.Models.Ebury;
using EburyMPIsoFilesLibrary.Services;

using Microsoft.Win32;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Windows;

namespace EburyMPIsoFiles.ViewModels
{
    public class IsoToMassPaymentsViewModel : BindableBase, INavigationAware
    {
        private readonly IObjectToPropertiesService _propertiesService;
        private string _InputFilePath;
        public string InputFilePath
        {
            get { return _InputFilePath; }
            set
            {
                if (SetProperty(ref _InputFilePath, value))
                {
                    if (!string.IsNullOrEmpty(value) && (new DirectoryInfo(value)).Exists)
                    {
                        UserSettings settings = _propertiesService.GetCurrent<UserSettings>();
                        settings.XmlFilePath = value;
                        _propertiesService.SaveCurrent(settings);
                    }
                    else
                    {
                        MessageBox.Show($"The Path you entered does not exist\nPlease try again\n{value}", "Invalid Path");
                        UserSettings settings = _propertiesService.GetCurrent<UserSettings>();
                        _InputFilePath = settings.XmlFilePath;
                        RaisePropertyChanged(nameof(InputFilePath));
                    }
                }
            }
        }
        private string _OutputFilePath;
        public string OutputFilePath
        {
            get { return _OutputFilePath; }
            set { SetProperty(ref _OutputFilePath, value); }
        }
        //private bool _SameOutputPath;
        //public bool SameOutputPath
        //{
        //    get { return _SameOutputPath; }
        //    set { SetProperty(ref _SameOutputPath, value); }
        //}
        private ObservableCollection<PaymentShortSummaryDisplay> _PaymenSummaryList;
        public ObservableCollection<PaymentShortSummaryDisplay> PaymentSummaryList
        {
            get { return _PaymenSummaryList; }
            set { SetProperty(ref _PaymenSummaryList, value); }
        }
        private ObservableCollection<MassPaymentFileModel> _PaylentList;
        public ObservableCollection<MassPaymentFileModel> PaymentList
        {
            get { return _PaylentList; }
            set { SetProperty(ref _PaylentList, value); }
        }
        private IsoPaymentFile _IsoFile;
        public IsoPaymentFile IsoFile
        {
            get { return _IsoFile; }
            set { SetProperty(ref _IsoFile, value); }
        }
        private EburyMassPaymentsFile _MassPaymentFile;
        public EburyMassPaymentsFile MassPaymentFile
        {
            get { return _MassPaymentFile; }
            set { SetProperty(ref _MassPaymentFile, value); }
        }
        public IsoPaymentFile isoPayments { get; set; }
        public IsoToMassPaymentsViewModel(IObjectToPropertiesService propertiesService)
        {
            _propertiesService = propertiesService;
            UserSettings settings = _propertiesService.GetCurrent<UserSettings>();
            _InputFilePath = settings?.XmlFilePath;
            _OutputFilePath = settings?.SaveFilePath;
        }
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            UserSettings settings = _propertiesService.GetCurrent<UserSettings>();
            InputFilePath = settings?.XmlFilePath;
            OutputFilePath = settings?.SaveFilePath;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            UserSettings settings = _propertiesService.GetCurrent<UserSettings>();
            if (settings == null)
            {
                settings = new UserSettings();
            }
            settings.XmlFilePath = InputFilePath;
        }
        private DelegateCommand _OpenFileCommand;
        public DelegateCommand OpenFileCommand =>
            _OpenFileCommand ?? (_OpenFileCommand = new DelegateCommand(ExecuteOpenFileCommand, CanExecuteOpenFileCommand));

        private bool CanExecuteOpenFileCommand()
        {
            if (String.IsNullOrEmpty(_propertiesService.GetCurrent<UserSettings>()?.XmlFilePath))
            {
                var result = MessageBox.Show("Please set the default File Path for your XML files", "Set File Path", MessageBoxButton.OK);
                return false;
            }
            else
                return true;
        }

        void ExecuteOpenFileCommand()
        {
            //string fileName = "";
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = $"ISO Xml|*.xml|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = InputFilePath;
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == true)
                {
                    MassPaymentFile = new EburyMassPaymentsFile();
                    //fileName = openFileDialog.FileName;
                    var fileNames = openFileDialog.FileNames;
                    List<string> outFiles = new List<string>();
                    foreach (var fileName in fileNames)
                    {
                        outFiles.Add(readIsoFile(fileName));
                    }
                    OutputFilePath = writeIsoFile(fileNames);
                    showPayments();
                    showSummary(MassPaymentFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Could not process ISO file", MessageBoxButton.OK);
            }
        }
        private string readIsoFile(string fileName)
        {
            string outStr;
            try
            {
                isoPayments = new IsoPaymentFile();
                isoPayments.ReadPaymentsFile(fileName);
                MassPaymentFile.Payments.AddRange(isoPayments.GetPaymentFileList());
                outStr = fileName.Replace(".xml", ".csv");
                //outStr = writeIsoFile(fileName);
            }
            catch (Exception ex)
            {
                outStr = "File Create Failed";
                MassPaymentFile = null;
                var content = $"Could not process the file: {fileName}";
                MessageBox.Show(content + $"\n{ex.Message}", "File Creation Failed", MessageBoxButton.OK);
                throw new ApplicationException(content, ex);
                //, "Issue Reading File", MessageBoxButton.OK);
            }
            return outStr;
        }

        private string writeIsoFile(string[] fileNames)
        {
            string outStr;
            var outFile = getOutFileName(fileNames, InputFilePath, OutputFilePath);
            MassPaymentFile.WriteMassPaymentsFile(outFile);
            outStr = outFile;
            return outStr;
        }

        private string getOutFileName(string[] inFileNames, string XmlPath, string OutputPath)
        {
            string output = multiFileName(inFileNames);

            XmlPath = XmlPath.Trim();
            OutputPath = _propertiesService.GetCurrent<UserSettings>()?.SaveFilePath;
            if (string.IsNullOrEmpty(OutputPath))
            {
                OutputPath = XmlPath;
            }

            if (output.Contains(XmlPath))
            {
                output = output.Replace(XmlPath, OutputPath);
            }

            output = Regex.Replace(output, @"\.xml", @".csv", RegexOptions.IgnoreCase);

            return output;
        }

        public string multiFileName(string[] files)
        {
            if (files == null || files.Length == 0)
                throw new ArgumentNullException($"{nameof(files)} must contain 1 or more names");

            if(files.Length == 1)
            {
                return files[0];
            }

            var fi = new FileInfo(files.First());
            var ext = fi.Extension;
            var path = fi.DirectoryName;
            string common = fi.Name;
            foreach (var file in files)
            {
                var fiName = new FileInfo(file).Name;
                common = string.Concat(common.TakeWhile((c, i) => c == fiName[i]));
                Debug.Print(common);
            }

            string output = fi.Name;
            if (output != common)
            {
                output = $"{path}\\{common}Multi-{DateTimeOffset.Now:yyMMdd-hhmmss}{ext}";
            }
            else
            {
                output = fi.FullName;
            }
            return output;
        }

        private void showPayments()
        {
            PaymentList = new ObservableCollection<MassPaymentFileModel>(MassPaymentFile.Payments);
        }
        private void showSummary(EburyMassPaymentsFile mpFile)
        {
            PaymentSummaryList = new ObservableCollection<PaymentShortSummaryDisplay>(PaymentShortSummaryDisplay.GetShortSummary(mpFile.Payments));
        }
        //private DelegateCommand _SamePathChangedCommand;
        //public DelegateCommand SamePathChangedCommand =>
        //    _SamePathChangedCommand ?? (_SamePathChangedCommand = new DelegateCommand(ExecuteSamePathChangedCommand));

        //void ExecuteSamePathChangedCommand()
        //{
        //    if (SameOutputPath)
        //        OutputFilePath = InputFilePath;
        //}

        private DelegateCommand _OpenOutputFileCommand;
        public DelegateCommand OpenOutputFileCommand =>
            _OpenOutputFileCommand ?? (_OpenOutputFileCommand = new DelegateCommand(ExecuteOpenOutputFileCommand, CanExecuteOpenOutputFileCommand)
            .ObservesProperty(() => OutputFilePath));

        bool CanExecuteOpenOutputFileCommand()
        {
            var output = false;
            if (!string.IsNullOrEmpty(OutputFilePath))
            {
                var fi = new System.IO.FileInfo(OutputFilePath);
                output = fi.Exists;
            }
            return output;
        }

        void ExecuteOpenOutputFileCommand()
        {
            Process xlProcess = new Process();
            try
            {
                if (CanExecuteOpenOutputFileCommand())
                    System.Diagnostics.Process.Start(OutputFilePath);
                else
                    throw new ApplicationException("File does not exist");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open file: {OutputFilePath}\n{ex.Message}", "CSV Review Error", MessageBoxButton.OK);
            }
        }

        private DelegateCommand<object> _DropFileDropCommand;
        public DelegateCommand<object> DropFileDropCommand =>
            _DropFileDropCommand ?? (_DropFileDropCommand = new DelegateCommand<object>(ExecuteDropFileDropCommand, CanExecuteDropFileDropCommand));

        void ExecuteDropFileDropCommand(object e)
        {
            DragEventArgs args = (DragEventArgs)e;
            MassPaymentFile = new EburyMassPaymentsFile();
            List<string> outFiles = new List<string>();
            string[] fileNames = (string[])args.Data.GetData(DataFormats.FileDrop, false);
            foreach (var fileName in fileNames)
            {
                if (fileName.ToLower().Contains(".xml"))
                {
                    outFiles.Add(readIsoFile(fileName));
                }
                OutputFilePath = writeIsoFile(fileNames);
                showPayments();
                showSummary(MassPaymentFile);

            }
        }

        bool CanExecuteDropFileDropCommand(object e)
        {
            DragEventArgs args = (DragEventArgs)e;
            return getDragEffect(args) == DragDropEffects.Copy;
        }

        private DelegateCommand<object> _DropFileEnterCommand;
        public DelegateCommand<object> DropFileEnterCommand =>
            _DropFileEnterCommand ?? (_DropFileEnterCommand = new DelegateCommand<object>(ExecuteDropFileEnterCommand));

        void ExecuteDropFileEnterCommand(object e)
        {
            DragEventArgs args = (DragEventArgs)e;
            args.Effects = getDragEffect(args);
            args.Handled = true;
        }

        private DragDropEffects getDragEffect(DragEventArgs args)
        {
            if (args.Data.GetDataPresent(DataFormats.FileDrop))
            {
                args.Effects = DragDropEffects.None;
                foreach (var fname in (string[])args.Data.GetData(DataFormats.FileDrop, false))
                {
                    if (fname.ToLower().Contains(".xml") && args.Effects != DragDropEffects.Copy)
                        args.Effects = DragDropEffects.Copy;
                }
            }
            else
            {
                args.Effects = DragDropEffects.None;
            }
            return args.Effects;
        }

        private DelegateCommand _SaveEburyFileCommand;
        public DelegateCommand SaveEburyFileCommand =>
            _SaveEburyFileCommand ?? (_SaveEburyFileCommand = new DelegateCommand(ExecuteSaveEburyFileCommand)
            .ObservesCanExecute(() => IsoFile != null));

        void ExecuteSaveEburyFileCommand()
        {

        }

    }
}

