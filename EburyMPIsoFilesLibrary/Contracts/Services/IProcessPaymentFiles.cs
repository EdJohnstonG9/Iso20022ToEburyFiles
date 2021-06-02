using EburyMPIsoFilesLibrary.Models;
using System.Collections.Generic;

namespace EburyMPIsoFilesLibrary.Services
{
    public interface IProcessPaymentFiles
    {
        List<string> InputFileNameList { get; }
        string InputFilePath { get; }
        InputTypeModel InputType { get; }
        List<EburyMassPaymentsFile> MassPaymentsOutput { get; }
        string OutputFilePath { get; }

        string OutputFileName(string inputFileName);
        string ReadInputFiles(List<string> inputFileNames, string inputFilePath, string outputFilePath);
    }
}