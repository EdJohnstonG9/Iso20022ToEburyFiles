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
using System.Linq;
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
            set { SetProperty(ref _InputFilePath, value); }
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
            //InputFilePath = @"G:\Shared drives\MP - High Wycombe - Data\XML FILE EXAMPLES";
            //SameOutputPath = true;
        }
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            InputFilePath = _propertiesService.GetCurrent<UserSettings>()?.XmlFilePath;
            OutputFilePath = _propertiesService.GetCurrent<UserSettings>()?.SaveFilePath;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
        private DelegateCommand _OpenFileCommand;
        public DelegateCommand OpenFileCommand =>
            _OpenFileCommand ?? (_OpenFileCommand = new DelegateCommand(ExecuteOpenFileCommand));

        void ExecuteOpenFileCommand()
        {
            string fileName = "";
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = $"ISO Xml|*.xml|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = InputFilePath;
                if (openFileDialog.ShowDialog() == true)
                {
                    fileName = openFileDialog.FileName;
                    var outFile = readIsoFile(fileName);
                    OutputFilePath = outFile;
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
                MassPaymentFile = new EburyMassPaymentsFile();
                MassPaymentFile.Payments = isoPayments.GetPaymentFileList();
                //var outFile = fileName.Replace(".xml", ".csv");
                var outFile = getOutFileName(fileName, InputFilePath, OutputFilePath);
                MassPaymentFile.WriteMassPaymentsFile(outFile);
                outStr = outFile;
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
        private string getOutFileName(string inFileName, string XmlPath, string OutputPath)
        {
            string output = inFileName;
            XmlPath = XmlPath.Trim();
            OutputPath = _propertiesService.GetCurrent<UserSettings>().SaveFilePath;
            if (inFileName.Contains(XmlPath))
            {
                output = inFileName.Replace(XmlPath, OutputPath);
            }

            output = Regex.Replace(output, @"\.xml", @".csv", RegexOptions.IgnoreCase);

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
            foreach (var fileName in (string[])args.Data.GetData(DataFormats.FileDrop, false))
            {
                if (fileName.ToLower().Contains(".xml"))
                {
                    var outFile = readIsoFile(fileName);
                    OutputFilePath = outFile;
                    showPayments();
                    showSummary(MassPaymentFile);
                }
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

