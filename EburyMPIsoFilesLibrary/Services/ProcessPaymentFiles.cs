using EburyMPIsoFilesLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EburyMPIsoFilesLibrary.Services
{
    public class ProcessPaymentFiles : IProcessPaymentFiles
    {
        public InputTypeModel InputType { get; }
        public List<string> InputFileNameList { get; private set; }
        public string InputFilePath { get; private set; }
        public string OutputFilePath { get; private set; }
        public List<EburyMassPaymentsFile> MassPaymentsOutput { get; private set; }

        public ProcessPaymentFiles(InputTypeModel inputType)
        {
            InputType = inputType;
        }

        public string ReadInputFiles(List<string> inputFileNames, string inputFilePath, string outputFilePath)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
            InputFileNameList = inputFileNames;
            string output = "";

            MassPaymentsOutput = new List<EburyMassPaymentsFile>();
            foreach (string file in InputFileNameList)
            {
                try
                {
                    MassPaymentsOutput.Add(readInputFile(file));
                    output = output.Trim() + $"\nSuccess:\t{file}";
                }
                catch (Exception ex)
                {
                    output = output.Trim() + $"\nFAILED:\t{file}\n{ex.Message}";
                }
            }
            return output;
        }

        private EburyMassPaymentsFile readInputFile(string fileName)
        {
            EburyMassPaymentsFile output;
            try
            {
                output = getInputData(fileName, InputType);

                var outFile = OutputFileName(fileName);
                output.WriteMassPaymentsFile(outFile);
            }
            catch (Exception ex)
            {
                //outStr = "File Create Failed";
                output = null;
                throw new ApplicationException($"Could not open the file: {fileName}\n{ex.Message}");
            }
            return output;
        }
        private EburyMassPaymentsFile getInputData(string fileName, InputTypeModel fileModel)
        {
            EburyMassPaymentsFile output = new EburyMassPaymentsFile();

            switch (fileModel.InputType)
            {
                case InputTypeModel.InputFileType.ISOPain113:
                    var pain113 = new Pain113PaymentFile();
                    pain113.ReadPaymentsFile(fileName);
                    output.Payments = pain113.MassPaymentFileList();
                    break;
                case InputTypeModel.InputFileType.ISOPain133:
                    var pain133 = new Pain133PaymentFile();
                    pain133.ReadPaymentsFile(fileName);
                    output.Payments = pain133.MassPaymentFileList();
                    break;
                case InputTypeModel.InputFileType.AirswiftText:
                    var airswift = new AirswiftPaymentFile();
                    airswift.ReadPaymentsFile(fileName);
                    output.Payments = airswift.MassPaymentFileList();
                    break;
                case InputTypeModel.InputFileType.BacsXls:
                    var Bacs = new BacsPaymentFile();
                    Bacs.ReadPaymentsFile(fileName);
                    output.Payments = Bacs.MassPaymentFileList();
                    break;
                default:
                    break;
            }
            return output;
        }
        public string OutputFileName(string inputFileName)
        {
            string output = inputFileName;
            InputFilePath = InputFilePath.Trim();
            if (inputFileName.Contains(InputFilePath))
            {
                output = inputFileName.Replace(InputFilePath, OutputFilePath);
            }

            output = Regex.Replace(output, InputType.InputExt, InputType.OuputExt, RegexOptions.IgnoreCase);

            return output;
        }
    }
}
